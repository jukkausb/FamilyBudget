using FamilyBudget.Www.App_CodeBase;
using FamilyBudget.Www.App_CodeBase.Widgets;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.App_Helpers;
using FamilyBudget.Www.App_Utils;
using FamilyBudget.Www.Models;
using FamilyBudget.Www.Models.Home;
using FamilyBudget.Www.Models.Repository.Interfaces;
using FamilyBudget.Www.Models.Widgets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace FamilyBudget.Www.Controllers
{
    public class HomeController : BaseController
    {
        private readonly DateTime _startDate = new DateTime(2014, 5, 1);

        private readonly ICurrencyProvider _currencyProvider;
        private readonly IAccountRepository _accountRepository;
        private readonly IIncomeRepository _incomeRepository;
        private readonly IExpenditureRepository _expenditureRepository;
        private readonly IProsperityProvider _prosperityProvider;
        private readonly INetProfitProvider _netProfitProvider;
        private readonly ITrendCalculator _trendCalculator;

        public HomeController(IAccountRepository accountRepository, IIncomeRepository incomeRepository, IExpenditureRepository expenditureRepository,
            ICurrencyProvider currencyProvider, IProsperityProvider prosperityProvider, INetProfitProvider netProfitProvider, ITrendCalculator trendCalculator)
        {
            _accountRepository = accountRepository;
            _incomeRepository = incomeRepository;
            _expenditureRepository = expenditureRepository;
            _currencyProvider = currencyProvider;
            _prosperityProvider = prosperityProvider;
            _netProfitProvider = netProfitProvider;
            _trendCalculator = trendCalculator;
        }

        protected List<Account> GetAccountsData()
        {
            return _accountRepository.GetAll().ToList();
        }

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
                    Id = "widget_expenditure_by_category_current_month",
                    Title = "Расходы по категориям в текущем месяце",
                    Url = "/Home/Widget_ExpenditureByCategoryCurrentMonth",
                    Callback = "ExpenditureByCategoryCurrentMonthCallback"
                },
                new Widget
                {
                    Id = "widget_expenditure_by_category",
                    Title = "Расходы по категориям за период",
                    Url = "/Home/Widget_ExpenditureByCategory",
                    Callback = "ExpenditureByCategoryCallback"
                },
                //new Widget
                //{
                //    Id = "widget_account_balance_circle",
                //    Title = "Состояние счетов в сравнении по главной валюте",
                //    Url = "/Home/Widget_AccountBalanceCircle",
                //    Callback = "AccountEquivalentsCircleCallback"
                //},
                new Widget
                {
                    Id = "widget_expenditure_income_compare",
                    Title = "Соотношение доходов и расходов",
                    Url = "/Home/Widget_ExpenditureIncomeCompare",
                    Callback = "ExpenditureIncomeCompareCallback"
                },

                new Widget
                {
                    Id = "widget_wealth_dynamic",
                    Title = "Динамика накопления",
                    Url = "/Home/Widget_WealthDynamic",
                    Callback = "WealthDynamicCallback"
                },

                //new Widget
                //{
                //    Id = "widget_prosperity_dynamic",
                //    Title = "Динамика благосостояния",
                //    Url = "/Home/Widget_ProsperityDynamic",
                //    Callback = "ProsperityDynamicCallback"
                //},

                new Widget
                {
                    Id = "widget_netprofit_dynamic",
                    Title = "Динамика чистой прибыли",
                    Url = "/Home/Widget_NetProfitDynamic",
                    Callback = "NetProfitDynamicCallback"
                }
            };
        }

        private List<ExpenditureIncomeItem> GetWidget_ExpenditureIncomeCompareWidgetData(int accountId,
            DateTime startDate, DateTime endDate)
        {
            var incomesPerMonth =
                (from income in _incomeRepository.GetAll()
                 where income.AccountID == accountId
                 where income.Date >= startDate.Date && income.Date <= endDate.Date
                 group income by new { income.Date.Year, income.Date.Month }
                     into g
                 select new
                 {
                     Period = new Period { Year = g.Key.Year, Month = g.Key.Month },
                     IncomeTotal = g.Sum(i => i.Summa)
                 }).ToList();

            var expendituresPerMonth =
                (from expenditure in _expenditureRepository.GetAll()
                 where expenditure.AccountID == accountId
                 where expenditure.Date >= startDate.Date && expenditure.Date <= endDate.Date
                 group expenditure by new { expenditure.Date.Month, expenditure.Date.Year }
                     into g
                 select new
                 {
                     Period = new Period { Year = g.Key.Year, Month = g.Key.Month },
                     ExpenditureTotal = g.Sum(i => i.Summa)
                 }).ToList();

            List<ExpenditureIncomeItem> resultsPerMonth = incomesPerMonth.Outer().Join(expendituresPerMonth.Outer(),
                k => new { k.Period.Year, k.Period.Month },
                k => new { k.Period.Year, k.Period.Month },
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
                (from expenditure in _expenditureRepository.GetAll()
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

            Account mainAccount = _accountRepository.GetAll().FirstOrDefault(a => a.IsMain);
            if (mainAccount != null)
            {
                string mainCurrencyCode = mainAccount.Currency.Code;
                model.MainCurrency = mainCurrencyCode.ToCurrencySymbol();
                List<Account> accounts = GetAccountsData();
                List<AccountRateView> accountRateViews = (from account in accounts
                                                          let accountCurrencyRate =
                                                              account.Currency.Code != mainCurrencyCode
                                                                  ? _currencyProvider.GetSellCurrencyRate(account.Currency.Code, mainCurrencyCode) : 1

                                                          select new AccountRateView
                                                          {
                                                              Account = account,
                                                              RateView =
                                                                  !account.Currency.Code.Equals(mainCurrencyCode, StringComparison.InvariantCultureIgnoreCase)
                                                                      ? new CurrencyRateView
                                                                      {
                                                                          SellRate = accountCurrencyRate,
                                                                          MainCurrency = mainCurrencyCode.ToCurrencySymbol(),
                                                                          OriginCurrency = account.Currency.Code.ToCurrencySymbol(),
                                                                          Equivalent = accountCurrencyRate * account.Balance
                                                                      }
                                                                      : null
                                                          }).ToList();

                model.Accounts = accountRateViews;
                model.Wealth.Currency = mainCurrencyCode.ToCurrencySymbol();
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
        public ActionResult Widget_ExpenditureByCategoryCurrentMonth()
        {
            var model = new ExpenditureByCategoryWidgetModel
            {
                Accounts = GetAccountsForDropDownExtended(),

                WidgetClientId = "widget_expenditure_by_category_current_month"
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
        public ActionResult Widget_ExpenditureByCategoryCurrentMonthData(int accountId)
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

            return Json(GetWidget_ExpenditureByCategoryWidgetData(accountId, startDate, endDate));
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
            Account mainAccount = _accountRepository.GetAll().FirstOrDefault(a => a.IsMain);
            IEnumerable<AccountCircleEquivalentView> accountEquivalentViews = null;
            if (mainAccount != null)
            {
                string mainCurrencyCode = mainAccount.Currency.Code;
                accountEquivalentViews = (from account in accounts
                                          let accountCurrencyRate = _currencyProvider.GetSellCurrencyRate(account.Currency.Code, mainCurrencyCode)
                                          let isMainAccount = account.Currency.Code.Equals(mainCurrencyCode, StringComparison.InvariantCultureIgnoreCase)
                                          let value = !isMainAccount ? accountCurrencyRate * account.Balance : account.Balance
                                          where value > 0
                                          select new AccountCircleEquivalentView
                                          {
                                              Value = string.Format("{0} ({1})", value.ToString("F"), account.Currency.Code),
                                              Label = account.DisplayName
                                          }).ToList();
            }

            return Json(accountEquivalentViews);
        }

        [HttpPost]
        public ActionResult Widget_WealthDynamic()
        {
            return PartialView(new WealthDynamicModel
            {
                WidgetClientId = "widget_wealth_dynamic"
            });
        }

        [HttpPost]
        public ActionResult Widget_WealthDynamicData()
        {
            var monthRange = EachDay(_startDate, DateTime.Now.Date).Select(d => new MonthDefinition
            {
                Month = d.Month,
                Year = d.Year
            }).Distinct(new MonthDefinitionComparer()).ToList();

            var prosperityList = GetProsperityDynamic(monthRange).ToList();

            var prosperityWithTrend = prosperityList.Select(p => new TrendLineMonthDefinition()
            {
                Month = p.Month,
                Year = p.Year,
                Value = p.Wealth
            }).ToList();

            prosperityWithTrend = _trendCalculator.CalculateTrend(prosperityWithTrend);

            return Json(prosperityWithTrend.Select(p =>
                new LabelAndValueWithTrend
                {
                    Label = string.Format("{1}-{0}", p.Month, p.Year),
                    Value = p.Value.ToString("F"),
                    TrendValue = p.TrendValue.ToString("F"),
                }));
        }

        [HttpPost]
        public ActionResult Widget_ProsperityDynamic()
        {
            return PartialView(new ProsperityDynamicModel
            {
                WidgetClientId = "widget_prosperity_dynamic"
            });
        }

        [HttpPost]
        public ActionResult Widget_ProsperityDynamicData()
        {
            var monthRange = EachDay(_startDate, DateTime.Now.Date).Select(d => new MonthDefinition
            {
                Month = d.Month,
                Year = d.Year
            }).Distinct(new MonthDefinitionComparer()).ToList();

            var prosperityList = GetProsperityDynamic(monthRange).ToList();

            return Json(prosperityList.Select(p =>
                new LabelAndValue
                {
                    Label = string.Format("{1}-{0}", p.Month, p.Year),
                    Value = p.ProsperityValue.ToString("F")
                }
                ));
        }

        [HttpPost]
        public ActionResult Widget_NetProfitDynamic()
        {
            return PartialView(new NetProfitDynamicModel
            {
                WidgetClientId = "widget_netprofit_dynamic"
            });
        }

        [HttpPost]
        public ActionResult Widget_NetProfitDynamicData()
        {
            var monthRange = EachDay(_startDate, DateTime.Now.Date).Select(d => new MonthDefinition
            {
                Month = d.Month,
                Year = d.Year
            }).Distinct(new MonthDefinitionComparer()).ToList();

            var wealthWithTrend = monthRange.Select(p => new TrendLineMonthDefinition()
            {
                Month = p.Month,
                Year = p.Year,
                Value = GetMonthNetProfit(new MonthDefinition() { Month = p.Month, Year = p.Year })
            }).ToList();

            wealthWithTrend = _trendCalculator.CalculateTrend(wealthWithTrend);

            return Json(wealthWithTrend.Select(m =>
                new LabelAndValueWithTrend
                {
                    Label = string.Format("{1}-{0}", m.Month, m.Year),
                    Value = m.Value.ToString("F"),
                    TrendValue = m.TrendValue.ToString("F"),
                }
                ));
        }

        private decimal GetMonthNetProfit(MonthDefinition m)
        {
            List<Account> accounts = _accountRepository.GetAll().ToList();
            Account mainAccount = accounts.FirstOrDefault(a => a.IsMain);
            string mainCurrencyCode = mainAccount?.Currency.Code ?? "RUB";

            var allIncomesInMonth = _incomeRepository.GetAll().Where(i => i.Date.Month == m.Month && i.Date.Year == m.Year).ToList();
            var allExpenditresInMonth = _expenditureRepository.GetAll().Where(i => i.Date.Month == m.Month && i.Date.Year == m.Year).ToList();
            return _netProfitProvider.CalculateNetProfit(accounts, allIncomesInMonth, allExpenditresInMonth, mainCurrencyCode);
        }

        public ActionResult Index()
        {
            var model = new DashboardModel
            {
                Widgets = GenerateWidgetDefinitions(),
            };

            return View(model);
        }

        protected List<ExtendedSelectListItem> GetAccountsForDropDownExtended()
        {
            List<ExtendedSelectListItem> accounts = _accountRepository.GetAll().ToList().Select(c => new ExtendedSelectListItem
            {
                IsBold = c.IsMain,
                Value = c.ID.ToString(CultureInfo.InvariantCulture),
                Text = c.DisplayName,
                Selected = c.IsMain,
                HtmlAttributes = new { data_currency = c.Currency.Code }
            }).ToList();

            accounts.Insert(0, new ExtendedSelectListItem { Text = " - Выберите счет - ", Value = "" });
            return accounts;
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        private IEnumerable<Prosperity> GetProsperityDynamic(IEnumerable<MonthDefinition> monthRange)
        {
            return monthRange.Select(m => _prosperityProvider.GetProsperity(m.Month, m.Year));
        }
    }

    public class MonthDefinition
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class TrendLineMonthDefinition : MonthDefinition
    {
        public decimal Value { get; set; }
        public decimal TrendValue { get; set; }
    }

    class MonthDefinitionComparer : IEqualityComparer<MonthDefinition>
    {
        public bool Equals(MonthDefinition b1, MonthDefinition b2)
        {
            if (b2 == null || b1 == null)
                return false;

            return b1.Month == b2.Month && b1.Year == b2.Year;
        }

        public int GetHashCode(MonthDefinition bx)
        {
            int hCode = bx.Year ^ bx.Month;
            return hCode.GetHashCode();
        }
    }
}