using System.Collections.Generic;
using FamilyBudget.v3.Models.Base;
using FamilyBudget.v3.Models.Home;

namespace FamilyBudget.v3.Models
{
    public class DashboardModel : BaseModel
    {
        public MoneyModel Wealth { get; set; }
        public List<AccountRateView> AccountRateViews { get; set; }
        public int AverageLastMonthCount { get; set; }
        public MoneyModel AverageIncomePerMonth { get; set; }
        public MoneyModel AverageExpenditurePerMonth { get; set; }
        public MoneyModel AverageProfitPerMonth { get; set; }
        public MoneyModel AllIISExpenditureTotal { get; set; }
        public MoneyModel AllBrokerAccountExpenditureTotal { get; set; }
    }
}