using FamilyBudget.v3.App_CodeBase;
using FamilyBudget.v3.App_CodeBase.Widgets;
using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.App_Utils;
using FamilyBudget.v3.Models;
using FamilyBudget.v3.Models.Repository.Interfaces;
using FamilyBudget.v3.Models.Widgets;
using System;
using System.Collections.Generic;
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
        private readonly ITrendCalculator _trendCalculator;

        public ChartsController(IAccountRepository accountRepository, IIncomeRepository incomeRepository, IExpenditureRepository expenditureRepository,
            ICurrencyProvider currencyProvider, ITrendCalculator trendCalculator)
        {
            _accountRepository = accountRepository;
            _incomeRepository = incomeRepository;
            _expenditureRepository = expenditureRepository;
            _currencyProvider = currencyProvider;
            _trendCalculator = trendCalculator;
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
                //new Widget
                //{
                //    Id = "widget_expenditure_by_category_current_month",
                //    Title = "Расходы по категориям в текущем месяце",
                //    Url = "/Charts/Widget_ExpenditureByCategoryCurrentMonth",
                //    Callback = "ExpenditureByCategoryCurrentMonthCallback"
                //},
                new Widget
                {
                    Id = "widget_expenditure_by_category",
                    Title = "Расходы по категориям за период",
                    Url = "/Charts/Widget_ExpenditureByCategory",
                    Callback = "ExpenditureByCategoryCallback"
                }
                //new Widget
                //{
                //    Id = "widget_expenditure_income_compare",
                //    Title = "Соотношение доходов и расходов",
                //    Url = "/Charts/Widget_ExpenditureIncomeCompare",
                //    Callback = "ExpenditureIncomeCompareCallback"
                //}
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

        private dynamic GetWidget_ExpenditureByCategoryWidgetData(string mainCurrencyCode,
            DateTime startDate, DateTime endDate)
        {
            List<Account> accounts = _accountRepository.GetAll().ToList();
            List<ExpenditureByCategoryItem> expendituresPerMonth = new List<ExpenditureByCategoryItem>();

            foreach (var account in accounts)
            {
                decimal rate = _currencyProvider.GetSellCurrencyRate(account.Currency.Code, mainCurrencyCode);
                if (account.Currency.Code == mainCurrencyCode)
                {
                    rate = 1;
                }

                expendituresPerMonth.AddRange(
                (from expenditure in _expenditureRepository.GetAll()
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

        [HttpPost]
        public ActionResult Widget_ExpenditureByCategoryCurrentMonth()
        {
            var model = new ExpenditureByCategoryWidgetModel
            {
                WidgetClientId = "widget_expenditure_by_category_current_month"
            };

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Widget_ExpenditureByCategoryCurrentMonthData()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

            return Json(GetWidget_ExpenditureByCategoryWidgetData(GetMainCurrencyCode(), startDate, endDate));
        }

        private string GetMainCurrencyCode()
        {
            var accounts = _accountRepository.GetAll().ToList();
            Account mainAccount = accounts.FirstOrDefault(a => a.IsMain);
            return mainAccount.Currency.Code;
        }
    }
}