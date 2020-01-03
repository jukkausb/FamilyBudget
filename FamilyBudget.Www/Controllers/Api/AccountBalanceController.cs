using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FamilyBudget.Www.App_CodeBase;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.App_Helpers;
using FamilyBudget.Www.App_Utils;
using FamilyBudget.Www.Models;
using FamilyBudget.Www.Models.Home;
using FamilyBudget.Www.Models.Repository.Interfaces;
using FamilyBudget.Www.Models.Spa;
using FamilyBudget.Www.Models.Widgets;

namespace FamilyBudget.Www.Controllers.Api
{
    public class AccountBalanceController : ApiController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IExpenditureRepository _expenditureRepository;
        private readonly IIncomeRepository _incomeRepository;
        private readonly ICurrencyProvider _currencyProvider;

        public AccountBalanceController(IAccountRepository accountRepository, ICurrencyProvider currencyProvider, IExpenditureRepository expenditureRepository, IIncomeRepository incomeRepository)
        {
            _accountRepository = accountRepository;
            _currencyProvider = currencyProvider;
            _expenditureRepository = expenditureRepository;
            _incomeRepository = incomeRepository;
        }

        public HttpResponseMessage Get()
        {
            var model = new AccountBalanceWidgetModel
            {
                WidgetClientId = "widget_account_balance",
                Wealth = new WealthModel()
            };

            Account mainAccount = _accountRepository.GetAll().FirstOrDefault(a => a.IsMain);
            if (mainAccount != null)
            {
                string mainCurrencyCode = mainAccount.Currency.Code;
                model.MainCurrency = mainCurrencyCode.ToCurrencySymbol();
                List<Account> accounts = _accountRepository.GetAll().ToList();
                List<AccountRateView> accountRateViews = (from account in accounts
                                                          let accountCurrencyRate =
                                                              account.Currency.Code != mainCurrencyCode
                                                                  ? _currencyProvider.GetSellCurrencyRate(account.Currency.Code, mainCurrencyCode) : 1

                                                          select new AccountRateView
                                                          {
                                                              Account = account,
                                                              AccountModel = new AccountModel()
                                                              {
                                                                  Balance = account.Balance,
                                                                  BalanceReal = GetRealAccountBalance(account),
                                                                  CurrencySymbol = account.Currency.Code.ToCurrencySymbol(),
                                                                  DisplayName = account.DisplayName
                                                              },
                                                              RateView =
                                                                  !account.Currency.Code.Equals(mainCurrencyCode, StringComparison.InvariantCultureIgnoreCase)
                                                                      ? new CurrencyRateView
                                                                      {
                                                                          SellRate = accountCurrencyRate,
                                                                          MainCurrency = mainCurrencyCode.ToCurrencySymbol(),
                                                                          OriginCurrency = account.Currency.Code.ToCurrencySymbol(),
                                                                          Equivalent = accountCurrencyRate * account.Balance
                                                                      }
                                                                      : new CurrencyRateView()
                                                          }).ToList();

                model.Accounts = accountRateViews;
                model.Wealth.Currency = mainCurrencyCode.ToCurrencySymbol();
                model.Wealth.Value = CalculateWealth(mainCurrencyCode);
            }

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        private decimal GetRealAccountBalance(Account account)
        {
            if (account == null)
            {
                return 0;
            }

            decimal incomeValue = 0;
            decimal expenditureValue = 0;

            bool anyIncome = _incomeRepository.GetAll().Where(i => i.AccountID == account.ID).Any();
            if (anyIncome)
            {
                incomeValue = _incomeRepository.GetAll().Where(i => i.AccountID == account.ID).Sum(i => i.Summa);
            }

            bool anyExpenditure = _expenditureRepository.GetAll().Where(i => i.AccountID == account.ID).Any();
            if (anyExpenditure)
            {
                expenditureValue = _expenditureRepository.GetAll().Where(i => i.AccountID == account.ID).Sum(i => i.Summa);
            }

            return incomeValue - expenditureValue;
        }

        private decimal CalculateWealth(string mainCurrencyCode)
        {
            decimal wealthValue = 0;
            List<Account> accounts = _accountRepository.GetAll().ToList();
            accounts.ForEach(a =>
            {
                if (a.Currency.Code != mainCurrencyCode)
                {
                    decimal rate = _currencyProvider.GetSellCurrencyRate(a.Currency.Code, mainCurrencyCode);
                    Logger.Info(string.Format("Exchange rate ({0}-{1}): {2}", a.Currency.Code, mainCurrencyCode, rate));
                    wealthValue += a.Balance * rate;
                }
                else
                {
                    wealthValue += a.Balance;
                }
            }
                );
            return wealthValue;
        }
    }
}