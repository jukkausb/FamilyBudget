using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using FamilyBudget.v3.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Tinkoff.Trading.OpenApi.Models;
using static Tinkoff.Trading.OpenApi.Models.PortfolioCurrencies;
using System;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Models.Repository.Interfaces;
using static FamilyBudget.v3.App_CodeBase.Tinkoff.Models.PortfolioCurrenciesExtended;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public interface ITinkoffInvestmentDataProvider
    {
        Task<List<InvestmentAccount>> GetInvestmentAccounts();
    }

    public class TinkoffInvestmentDataProvider : ITinkoffInvestmentDataProvider
    {
        private readonly IExpenditureRepository _expenditureRepository;

        public TinkoffInvestmentDataProvider(IExpenditureRepository expenditureRepository)
        {
            _expenditureRepository = expenditureRepository;
        }

        private string GetToken()
        {
            string tokenFileName = System.Web.Configuration.WebConfigurationManager.AppSettings["TinkoffTokenFileName"];
            ITinkoffOpenApiTokenReader tinkoffOpenApiTokenReader = new TinkoffOpenApiTokenReader();
            string token = tinkoffOpenApiTokenReader.ReadToken(tokenFileName);
            return token;
        }

        private async Task<List<TinkoffPortfolioPosition>> GetPortfolioPositions(string brokerAccountId)
        {
            string token = GetToken();
            ITinkoffPortfolioDataRetriever tinkoffPortfolioDataRetriever = new TinkoffPortfolioDataRetriever(token);
            List<TinkoffPortfolioPosition> positions = await tinkoffPortfolioDataRetriever.GetTinkoffPortfolioPositions(brokerAccountId);
            return positions;
        }

        private async Task<List<PortfolioCurrencyExtended>> GetPortfolioCurrencies(string brokerAccountId)
        {
            string token = GetToken();
            ITinkoffPortfolioDataRetriever tinkoffPortfolioDataRetriever = new TinkoffPortfolioDataRetriever(token);
            List<PortfolioCurrencyExtended> positions = await tinkoffPortfolioDataRetriever.GetTinkoffPortfolioCurrencies(brokerAccountId);
            return positions;
        }

        public async Task<List<InvestmentAccount>> GetInvestmentAccounts()
        {
            string token = GetToken();

            decimal investmentsToIIS = BusinessHelper.GetIISExpenditures(_expenditureRepository).Sum(e => e.Summa);
            decimal investmentsBrokerAccount = BusinessHelper.GetBrokerAccountExpenditures(_expenditureRepository).Sum(e => e.Summa);
            decimal totalInvestments = investmentsToIIS + investmentsBrokerAccount;

            ITinkoffUserAccountDataRetriever tinkoffUserAccountDataRetriever = new TinkoffUserAccountDataRetriever(token);
            List<TinkoffBrokerAccount> accounts = await tinkoffUserAccountDataRetriever.GetTinkoffUserAccounts();

            List<InvestmentAccount> investmentAccounts = new List<InvestmentAccount>();

            foreach (var account in accounts)
            {
                InvestmentAccount investmentAccount = new InvestmentAccount()
                {
                    Id = account.BrokerAccountId,
                    Currency = Constants.CURRENCY_RUB // Инвестирую только в рублях (это Крым, детка)
                };

                if (account.BrokerAccountType == BrokerAccountType.TinkoffIis)
                {
                    investmentAccount.Name = "ИИС";
                    investmentAccount.TotalInvested = investmentsToIIS;
                    investmentAccount.IsActive = false;
                }
                else
                {
                    investmentAccount.Name = "Брокерский счет";
                    investmentAccount.TotalInvested = investmentsBrokerAccount;
                    investmentAccount.IsActive = true;
                }

                var portfolioPositions = await GetPortfolioPositions(account.BrokerAccountId);
                var accountCashRub = (await GetPortfolioCurrencies(account.BrokerAccountId)).
                    Where(c => c.Currency.ToString().ToUpper() == Constants.CURRENCY_RUB).
                    FirstOrDefault();

                var groups = portfolioPositions.GroupBy(p => p.Type).ToList();

                investmentAccount.TotalBalance = portfolioPositions.Sum(p => p.CurrentTotalInPortfolio) + accountCashRub.Balance;
                investmentAccount.TotalDelta = portfolioPositions.Sum(p => p.CurrentDelta);
                investmentAccount.TotalDeltaPercent = Math.Round(Math.Abs(investmentAccount.TotalDelta / investmentAccount.TotalInvested * 100), 2).ToString("N2");

                foreach (var group in groups)
                {
                    if (group.Key == InstrumentType.Bond)
                    {
                        investmentAccount.Bonds = group.OrderBy(p => p.Name).ToList();
                    }
                    if (group.Key == InstrumentType.Currency)
                    {
                        investmentAccount.Currencies = group.OrderBy(p => p.Name).ToList();
                        investmentAccount.Currencies.Add(new TinkoffPortfolioPosition
                        {
                            Name = Constants.CURRENCY_NAME_RUB,
                            Isin = Constants.CURRENCY_RUB,
                            CurrentTotalInPortfolio = accountCashRub.Balance
                        });
                    }
                    if (group.Key == InstrumentType.Etf)
                    {
                        investmentAccount.Etfs = group.OrderBy(p => p.Name).ToList();
                    }
                    if (group.Key == InstrumentType.Stock)
                    {
                        investmentAccount.Stocks = group.OrderBy(p => p.Name).ToList();
                    }
                }

                investmentAccounts.Add(investmentAccount);
            }

            return investmentAccounts;
        }
    }
}