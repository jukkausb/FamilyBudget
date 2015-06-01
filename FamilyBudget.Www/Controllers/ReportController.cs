using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FamilyBudget.Www.App_DataModel;

namespace FamilyBudget.Www.Controllers
{
    public class ReportController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PerformIncomesReport(DateTime startDate, DateTime endDate)
        {
            var incomesPerMonth =
                (from income in DbModelFamilyBudgetEntities.Income
                 where income.Date >= startDate && income.Date <= endDate
                 group income by new { income.Date.Month, income.Date.Year }
                     into g
                     select new
                     {
                         g.Key.Month,
                         g.Key.Year,
                         Total = g.Sum(i => i.Summa)
                     }).OrderBy(i => i.Year).ThenBy(i => i.Month).ToList();

            var data = new ArrayList { new ArrayList { "Период", "Доход" } };

            foreach (var incomePerMonth in incomesPerMonth)
            {
                data.Add(new ArrayList
                {
                    string.Format("{0}-{1}", incomePerMonth.Year, incomePerMonth.Month),
                    incomePerMonth.Total
                });
            }

            return Json(data);
        }

        public ActionResult PerformExpendituresReport(DateTime startDate, DateTime endDate)
        {
            var expendituresPerMonth =
                (from expenditure in DbModelFamilyBudgetEntities.Expenditure
                 where expenditure.Date >= startDate && expenditure.Date <= endDate
                 group expenditure by new { expenditure.Date.Month, expenditure.Date.Year }
                     into g
                     select new
                     {
                         g.Key.Month,
                         g.Key.Year,
                         Total = g.Sum(i => i.Summa)
                     }).OrderBy(i => i.Year).ThenBy(i => i.Month).ToList();

            var data = new ArrayList { new ArrayList { "Период", "Расход" } };

            foreach (var expenditurePerMonth in expendituresPerMonth)
            {
                data.Add(new ArrayList
                {
                    string.Format("{0}-{1}", expenditurePerMonth.Year, expenditurePerMonth.Month),
                    expenditurePerMonth.Total
                });
            }

            return Json(data);
        }

        public ActionResult PerformExpendituresByCategoryReport(DateTime startDate, DateTime endDate)
        {
            List<ExpenditureCategory> categories = DbModelFamilyBudgetEntities.ExpenditureCategory.ToList();

            var data = new ArrayList();
            var titles = new ArrayList { "Период" };
            foreach (ExpenditureCategory category in categories)
            {
                titles.Add(category.Name.Trim());
            }
            data.Add(titles);

            var titles2 = new ArrayList { "2011" };
            foreach (ExpenditureCategory category in categories)
            {
                titles2.Add(100 * categories.IndexOf(category) + 50);
            }
            data.Add(titles2);

            var titles3 = new ArrayList { "2012" };
            foreach (ExpenditureCategory category in categories)
            {
                titles3.Add(200 * categories.IndexOf(category) + 100);
            }
            data.Add(titles3);

            return Json(data);
        }
    }
}