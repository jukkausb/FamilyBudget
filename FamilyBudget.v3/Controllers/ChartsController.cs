using FamilyBudget.v3.App_CodeBase;
using FamilyBudget.v3.App_CodeBase.Widgets;
using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Controllers.Services;
using FamilyBudget.v3.Models;
using FamilyBudget.v3.Models.Home;
using FamilyBudget.v3.Models.Repository.Interfaces;
using FamilyBudget.v3.Models.Widgets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace FamilyBudget.v3.Controllers
{
    public class ChartsController : BaseController
    {
        private readonly ICurrencyProvider _currencyProvider;
        private readonly IAccountRepository _accountRepository;
        private readonly IIncomeRepository _incomeRepository;
        private readonly IExpenditureRepository _expenditureRepository;
        private readonly ICalculationService _calculationService;

        public ChartsController(IAccountRepository accountRepository, IIncomeRepository incomeRepository,
            IExpenditureRepository expenditureRepository, ICurrencyProvider currencyProvider,
            ICalculationService calculationService)
        {
            _accountRepository = accountRepository;
            _incomeRepository = incomeRepository;
            _expenditureRepository = expenditureRepository;
            _currencyProvider = currencyProvider;
            _calculationService = calculationService;
        }

        public ActionResult Index()
        {
            ChartsModel model = new ChartsModel();

            model.Widgets = GenerateWidgetDefinitions();

            return View(model);
        }

        #region Widget services

        private List<Widget> GenerateWidgetDefinitions()
        {
            return new List<Widget>
            {
                new Widget
                {
                    Id = "widget_account_balance_circle",
                    Title = "Баланс по валютам",
                    Url = "/Charts/Widget_AccountBalanceCircle",
                    Callback = "AccountBalanceCircleCallback",
                    ColumnCssClass = "col-lg-6"
                },
                new Widget
                {
                    Id = "widget_balance_and_investment_circle",
                    Title = "Распределение денежных средств",
                    Url = "/Charts/Widget_BalanceAndInvestmentCircle",
                    Callback = "BalanceAndInvestmentCircleCallback",
                    ColumnCssClass = "col-lg-6"
                },
                new Widget
                {
                    Id = "widget_expenditure_by_category",
                    Title = "Расходы по категориям за период",
                    Url = "/Charts/Widget_ExpenditureByCategory",
                    Callback = "ExpenditureByCategoryCallback",
                    ColumnCssClass = "col-lg-12"
                },
                new Widget
                {
                    Id = "widget_expenditure_income_compare",
                    Title = "Соотношение доходов и расходов",
                    Url = "/Charts/Widget_ExpenditureIncomeCompare",
                    Callback = "ExpenditureIncomeCompareCallback",
                    ColumnCssClass = "col-lg-12"
                }
            };
        }

        private List<AccountRateView> GetWidget_AccountBalanceCircleData()
        {
            return _calculationService.GetAccountBalanceWithRatesViews();
        }

        private List<AccountRateView> GetWidget_BalanceAndInvestmentCircleData()
        {
            return _calculationService.GetBalanceAndInvestmentWithRatesViews();
        }

        private List<ExpenditureIncomeItem> GetWidget_ExpenditureIncomeCompareWidgetData(
            DateTime startDate, DateTime endDate)
        {
            string mainCurrencyCode = GetMainCurrencyCode();
            List<Account> accounts = _accountRepository.GetAll().ToList();
            List<ExpenditureIncomeItem> result = new List<ExpenditureIncomeItem>();

            foreach (var account in accounts)
            {
                decimal rate = (decimal)_currencyProvider.GetSellCurrencyRate(account.Currency.Code, mainCurrencyCode);
                if (account.Currency.Code == mainCurrencyCode)
                {
                    rate = 1;
                }

                var incomesPerMonth =
                (from income in BusinessHelper.GetRealIncomes(_incomeRepository)
                 let accountCurrencyCode = income.Account.Currency.Code
                 where income.AccountID == account.ID
                 where income.Date >= startDate.Date && income.Date <= endDate.Date
                 group income by new { income.Date.Year, income.Date.Month } into g
                 select new
                 {
                     g.Key.Year,
                     g.Key.Month,
                     Period = g.Key.Year + "-" + g.Key.Month,
                     IncomeTotal = g.Sum(i => i.Summa * rate)
                 }).ToList();

                var expendituresPerMonth =
                    (from expenditure in BusinessHelper.GetRealExpenditures(_expenditureRepository)
                     let accountCurrencyCode = expenditure.Account.Currency.Code
                     where expenditure.AccountID == account.ID
                     where expenditure.Date >= startDate.Date && expenditure.Date <= endDate.Date
                     group expenditure by new { expenditure.Date.Month, expenditure.Date.Year } into g
                     select new
                     {
                         g.Key.Year,
                         g.Key.Month,
                         Period = g.Key.Year + "-" + g.Key.Month,
                         ExpenditureTotal = g.Sum(i => i.Summa * rate)
                     }).ToList();

                var aggregatedData = incomesPerMonth.Outer().Join(expendituresPerMonth.Outer(),
                    k => new { k.Period },
                    k => new { k.Period },
                    (i, e) =>
                        new ExpenditureIncomeItem
                        {
                            Year = (i?.Year ?? e?.Year) ?? 0,
                            Month = (i?.Month ?? e?.Month) ?? 0,
                            Period = (i?.Period ?? e?.Period) ?? "",
                            ExpenditureValue = e != null ? e.ExpenditureTotal : 0,
                            IncomeValue = i != null ? i.IncomeTotal : 0
                        }
                    ).OrderBy(r => r.Period).ToList();

                foreach (var aggregatedDataItem in aggregatedData)
                {
                    var item = result.FirstOrDefault(i => i.Period == aggregatedDataItem.Period);
                    if (item == null)
                    {
                        item = new ExpenditureIncomeItem
                        {
                            Year = aggregatedDataItem.Year,
                            Month = aggregatedDataItem.Month,
                            ExpenditureValue = aggregatedDataItem.ExpenditureValue,
                            IncomeValue = aggregatedDataItem.IncomeValue,
                            Period = aggregatedDataItem.Period
                        };
                        result.Add(item);
                    }
                    else
                    {
                        item.ExpenditureValue += aggregatedDataItem.ExpenditureValue;
                        item.IncomeValue += aggregatedDataItem.IncomeValue;
                    }
                }
            }

            return result.OrderBy(r => r.Year).ThenBy(r => r.Month).ToList();
        }

        private dynamic GetWidget_ExpenditureByCategoryWidgetData(string mainCurrencyCode,
            DateTime startDate, DateTime endDate)
        {
            List<Account> accounts = _accountRepository.GetAll().ToList();
            List<ExpenditureByCategoryItem> expendituresPerMonth = new List<ExpenditureByCategoryItem>();

            foreach (var account in accounts)
            {
                decimal rate = (decimal)_currencyProvider.GetSellCurrencyRate(account.Currency.Code, mainCurrencyCode);
                if (account.Currency.Code == mainCurrencyCode)
                {
                    rate = 1;
                }

                expendituresPerMonth.AddRange(
                (from expenditure in BusinessHelper.GetRealExpenditures(_expenditureRepository)
                 where expenditure.AccountID == account.ID
                 where expenditure.Date >= startDate.Date && expenditure.Date <= endDate.Date
                 group expenditure by expenditure.ExpenditureCategory.Name
                     into g
                 select g).ToList().Select(g =>
                    new ExpenditureByCategoryItem
                    {
                        Category = g.Key,
                        Total = g.Sum(e => e.Summa * rate),
                    }).ToList());
            }

            expendituresPerMonth.GroupBy(ec => ec.Category).Select(g =>
                new ExpenditureByCategoryItem
                {
                    Category = g.Key,
                    Total = g.Sum(e => e.Total)
                }).ToList();

            return expendituresPerMonth.Select(a => new
            {
                Category = a.Category,
                Total = Math.Round(a.Total).ToString("G")
            }).ToList();
        }

        #endregion

        public ActionResult GetWidgetDefinitions()
        {
            return Json(GenerateWidgetDefinitions(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Widget_AccountBalanceCircle()
        {
            var model = new AccountBalanceCircleWidgetModel
            {
                WidgetClientId = "widget_account_balance_circle"
            };

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Widget_BalanceAndInvestmentCircle()
        {
            var model = new BalanceAndInvestmentCircleWidgetModel
            {
                WidgetClientId = "widget_balance_and_investment_circle"
            };

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Widget_AccountBalanceCircleData()
        {
            return Json(GetWidget_AccountBalanceCircleData());
        }

        [HttpPost]
        public ActionResult Widget_BalanceAndInvestmentCircleData()
        {
            return Json(GetWidget_BalanceAndInvestmentCircleData());
        }

        [HttpPost]
        public ActionResult Widget_ExpenditureIncomeCompare()
        {
            DateTime now = DateTime.Now;
            var model = new ExpenditureIncomeCompareWidgetModel
            {
                StartDate = now.AddYears(-1),
                EndDate = now,
                WidgetClientId = "widget_expenditure_income_compare"
            };

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Widget_ExpenditureIncomeCompareData(DateTime? startDate, DateTime? endDate)
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

            return Json(GetWidget_ExpenditureIncomeCompareWidgetData(startDate.Value, endDate.Value));
        }

        [HttpPost]
        public ActionResult Widget_ExpenditureByCategory()
        {
            DateTime now = DateTime.Now;
            var model = new ExpenditureByCategoryWidgetModel
            {
                StartDate = now.AddYears(-1),
                EndDate = now,
                WidgetClientId = "widget_expenditure_by_category"
            };

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Widget_ExpenditureByCategoryData(DateTime? startDate, DateTime? endDate)
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

            return Json(GetWidget_ExpenditureByCategoryWidgetData(GetMainCurrencyCode(), startDate.Value, endDate.Value));
        }

        private string GetMainCurrencyCode()
        {
            var accounts = _accountRepository.GetAll().ToList();
            Account mainAccount = accounts.FirstOrDefault(a => a.IsMain);
            return mainAccount.Currency.Code;
        }
    }
}