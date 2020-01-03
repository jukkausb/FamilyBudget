using FamilyBudget.v3.Models.Base;

namespace FamilyBudget.v3.Models.Report
{
    public class ReportsModel : BaseModel
    {
        public ReportIncomesModel ReportIncomesModel { get; set; }
        public ReportExpendituresModel ReportExpendituresModel { get; set; }

        public ReportExpendituresByCategoryModel ReportExpendituresByCategoryModel { get; set; }
    }
}