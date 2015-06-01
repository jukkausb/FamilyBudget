using FamilyBudget.Www.Models.Base;

namespace FamilyBudget.Www.Models.Report
{
    public class ReportsModel : BaseModel
    {
        public ReportIncomesModel ReportIncomesModel { get; set; }
        public ReportExpendituresModel ReportExpendituresModel { get; set; }

        public ReportExpendituresByCategoryModel ReportExpendituresByCategoryModel { get; set; }
    }
}