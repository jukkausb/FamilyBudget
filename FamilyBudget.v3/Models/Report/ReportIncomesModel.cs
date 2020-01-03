using System;
using FamilyBudget.v3.Models.Base;

namespace FamilyBudget.v3.Models.Report
{
    public class ReportIncomesModel : BaseModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}