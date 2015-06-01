﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using FamilyBudget.Www.App_CodeBase;
using FamilyBudget.Www.App_CodeBase.Csv;
using FamilyBudget.Www.App_CodeBase.Widgets;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.App_Utils;
using FamilyBudget.Www.Models;
using FamilyBudget.Www.Models.Home;
using FamilyBudget.Www.Models.Widgets;

namespace FamilyBudget.Www.Controllers
{
    public class HomeController : BaseController
    {
        #region Widget services

        private List<Widget> GenerateWidgetDefinitions()
        {
            return new List<Widget>
            {
                new Widget
                {
                    Id = "widget_account_balance",
                    Title = "Состояние счетов",
                    Url = "/Home/Widget_AccountBalance"
                },
                new Widget
                {
                    Id = "widget_account_balance_circle",
                    Title = "Состояние счетов в сравнении по главной валюте",
                    Url = "/Home/Widget_AccountBalanceCircle",
                    Callback = "AccountEquivalentsCircleCallback"
                },
                new Widget
                {
                    Id = "widget_expenditure_income_compare",
                    Title = "Соотношение доходов и расходов",
                    Url = "/Home/Widget_ExpenditureIncomeCompare",
                    Callback = "ExpenditureIncomeCompareCallback"
                },
                new Widget
                {
                    Id = "widget_expenditure_by_category",
                    Title = "Расходы по категориям за период",
                    Url = "/Home/Widget_ExpenditureByCategory",
                    Callback = "ExpenditureByCategoryCallback"
                }
            };
        }

        private List<ExpenditureIncomeItem> GetWidget_ExpenditureIncomeCompareWidgetData(int accountId,
            DateTime startDate, DateTime endDate)
        {
            var incomesPerMonth =
                (from income in DbModelFamilyBudgetEntities.Income
                    where income.AccountID == accountId
                    where income.Date >= startDate.Date && income.Date <= endDate.Date
                    group income by new {income.Date.Year, income.Date.Month}
                    into g
                    select new
                    {
                        Period = new Period {Year = g.Key.Year, Month = g.Key.Month},
                        IncomeTotal = g.Sum(i => i.Summa)
                    }).ToList();

            var expendituresPerMonth =
                (from expenditure in DbModelFamilyBudgetEntities.Expenditure
                    where expenditure.AccountID == accountId
                    where expenditure.Date >= startDate.Date && expenditure.Date <= endDate.Date
                    group expenditure by new {expenditure.Date.Month, expenditure.Date.Year}
                    into g
                    select new
                    {
                        Period = new Period {Year = g.Key.Year, Month = g.Key.Month},
                        ExpenditureTotal = g.Sum(i => i.Summa)
                    }).ToList();

            List<ExpenditureIncomeItem> resultsPerMonth = incomesPerMonth.Outer().Join(expendituresPerMonth.Outer(),
                k => new {k.Period.Year, k.Period.Month},
                k => new {k.Period.Year, k.Period.Month},
                (i, e) =>
                    new ExpenditureIncomeItem
                    {
                        Year = i != null ? i.Period.Year : e.Period.Year,
                        Month = i != null ? i.Period.Month : e.Period.Month,
                        Period =
                            i != null
                                ? string.Format("{0}-{1}", i.Period.Year, i.Period.Month)
                                : string.Format("{0}-{1}", e.Period.Year, e.Period.Month),
                        Expenditure = e != null ? e.ExpenditureTotal.ToString(Thread.CurrentThread.CurrentCulture) : "0",
                        Income = i != null ? i.IncomeTotal.ToString(Thread.CurrentThread.CurrentCulture) : "0"
                    }
                ).OrderBy(r => r.Year).ThenBy(r => r.Month).ToList();

            return resultsPerMonth;
        }

        private List<ExpenditureByCategoryItem> GetWidget_ExpenditureByCategoryWidgetData(int accountId,
            DateTime startDate, DateTime endDate)
        {
            List<ExpenditureByCategoryItem> expendituresPerMonth =
                (from expenditure in DbModelFamilyBudgetEntities.Expenditure
                    where expenditure.AccountID == accountId
                    where expenditure.Date >= startDate.Date && expenditure.Date <= endDate.Date
                    group expenditure by expenditure.ExpenditureCategory.Name
                    into g
                    select g).ToList().Select(g =>
                        new ExpenditureByCategoryItem
                        {
                            Category = g.Key,
                            Total = g.Sum(e => e.Summa).ToString(CultureInfo.CurrentCulture),
                        }).ToList();

            return expendituresPerMonth;
        }

        private decimal CalculateWealth(string mainCurrencyCode)
        {
            decimal wealthValue = 0;
            List<Account> accounts = DbModelFamilyBudgetEntities.Account.ToList();
            accounts.ForEach(a =>
            {
                if (a.Currency.Code != mainCurrencyCode)
                {
                    decimal rate = CurrencyProvider.GetSellCurrencyRate(a.Currency.Code, mainCurrencyCode);
                    Logger.Info(string.Format("Exchange rate ({0}-{1}): {2}", a.Currency.Code, mainCurrencyCode, rate));
                    wealthValue += a.Balance*rate;
                }
                else
                {
                    wealthValue += a.Balance;
                }
            }
                );
            return wealthValue;
        }

        #endregion

        public ActionResult GetWidgetDefinitions()
        {
            return Json(GenerateWidgetDefinitions(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Widget_AccountBalance()
        {
            var model = new AccountBalanceWidgetModel
            {
                WidgetClientId = "widget_account_balance",
                Wealth = new WealthModel()
            };

            Account mainAccount = DbModelFamilyBudgetEntities.Account.FirstOrDefault(a => a.IsMain);
            if (mainAccount != null)
            {
                string mainCurrencyCode = mainAccount.Currency.Code;
                model.MainCurrency = mainCurrencyCode;
                List<Account> accounts = GetAccountsData();
                List<AccountRateView> accountRateViews = (from account in accounts
                    let accountCurrencyRate = CurrencyProvider.GetCurrencyRate(account.Currency.Code, mainCurrencyCode)
                    select new AccountRateView
                    {
                        Account = account,
                        RateView =
                            !account.Currency.Code.Equals(mainCurrencyCode, StringComparison.InvariantCultureIgnoreCase)
                                ? new CurrencyRateView
                                {
                                    Rate = accountCurrencyRate,
                                    MainCurrency = mainCurrencyCode,
                                    OriginCurrency = account.Currency.Code,
                                    Equivalent =
                                        accountCurrencyRate != null ? accountCurrencyRate.SellRate*account.Balance : 0
                                }
                                : null
                    }).ToList();

                model.Accounts = accountRateViews;
                model.Wealth.Currency = mainCurrencyCode;
                model.Wealth.Value = CalculateWealth(mainCurrencyCode);
            }

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Widget_ExpenditureIncomeCompare()
        {
            DateTime now = DateTime.Now;
            var model = new ExpenditureIncomeCompareWidgetModel
            {
                Accounts = GetAccountsForDropDownExtended(),
                StartDate = now.AddYears(-1),
                EndDate = now,
                WidgetClientId = "widget_expenditure_income_compare"
            };

            List<Account> accounts = GetAccountsData();
            if (accounts != null && accounts.Count > 0)
            {
                Account mainAccount = accounts.Find(a => a.IsMain);
                if (mainAccount != null)
                {
                    model.SelectedAccountId = mainAccount.ID.ToString(CultureInfo.CurrentCulture);
                }
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Widget_ExpenditureIncomeCompareData(int accountId, DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
            {
                DateTime now = DateTime.Now;
                startDate = new DateTime(now.Year, 1, 1);
            }

            if (!endDate.HasValue)
            {
                endDate = DateTime.Now;
            }

            return Json(GetWidget_ExpenditureIncomeCompareWidgetData(accountId, startDate.Value, endDate.Value));
        }

        [HttpPost]
        public ActionResult Widget_ExpenditureByCategory()
        {
            DateTime now = DateTime.Now;
            var model = new ExpenditureByCategoryWidgetModel
            {
                Accounts = GetAccountsForDropDownExtended(),
                StartDate = now.AddYears(-1),
                EndDate = now,
                WidgetClientId = "widget_expenditure_by_category"
            };

            List<Account> accounts = GetAccountsData();
            if (accounts != null && accounts.Count > 0)
            {
                Account mainAccount = accounts.Find(a => a.IsMain);
                if (mainAccount != null)
                {
                    model.SelectedAccountId = mainAccount.ID.ToString(CultureInfo.CurrentCulture);
                }
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Widget_ExpenditureByCategoryData(int accountId, DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
            {
                DateTime now = DateTime.Now;
                startDate = new DateTime(now.Year, 1, 1);
            }

            if (!endDate.HasValue)
            {
                endDate = DateTime.Now;
            }

            return Json(GetWidget_ExpenditureByCategoryWidgetData(accountId, startDate.Value, endDate.Value));
        }

        [HttpPost]
        public ActionResult Widget_AccountBalanceCircle()
        {
            return PartialView(new AccountBalanceCircleWidgetModel
            {
                WidgetClientId = "widget_account_balance_circle"
            });
        }

        [HttpPost]
        public ActionResult Widget_AccountBalanceCircleData()
        {
            List<Account> accounts = GetAccountsData();
            Account mainAccount = DbModelFamilyBudgetEntities.Account.FirstOrDefault(a => a.IsMain);
            IEnumerable<AccountCircleEquivalentView> accountEquivalentViews = null;
            if (mainAccount != null)
            {
                string mainCurrencyCode = mainAccount.Currency.Code;
                accountEquivalentViews = (from account in accounts
                    let accountCurrencyRate = CurrencyProvider.GetCurrencyRate(account.Currency.Code, mainCurrencyCode)
                    let isMainAccount =
                        account.Currency.Code.Equals(mainCurrencyCode, StringComparison.InvariantCultureIgnoreCase)
                    select new AccountCircleEquivalentView
                    {
                        Value =
                            (!isMainAccount
                                ? accountCurrencyRate != null
                                    ? accountCurrencyRate.SellRate*account.Balance
                                    : account.Balance
                                : account.Balance),
                        Label = account.DisplayName
                    }).Where(a => a.Value > 0).ToList();
            }

            return Json(accountEquivalentViews);
        }

        public ActionResult Index()
        {
            var model = new DashboardModel
            {
                Widgets = GenerateWidgetDefinitions()
            };
            return View(model);
        }
    }
}