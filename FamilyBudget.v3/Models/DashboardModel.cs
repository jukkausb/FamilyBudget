using System.Collections.Generic;
using FamilyBudget.v3.Models.Base;
using FamilyBudget.v3.Models.Home;

namespace FamilyBudget.v3.Models
{
    public class DashboardModel : BaseModel
    {
        /// <summary>
        /// Total capital
        /// </summary>
        public MoneyModel Capital { get; set; }
        /// <summary>
        /// Cash on accounts
        /// </summary>
        public MoneyModel Cash { get; set; }
        /// <summary>
        /// Total results of investment
        /// </summary>
        public MoneyModel Investment { get; set; }
        public List<AccountRateView> AccountRateViews { get; set; }
        public int AverageLastMonthCount { get; set; }
        public MoneyModel AverageIncomePerMonth { get; set; }
        public MoneyModel AverageExpenditurePerMonth { get; set; }
        public MoneyModel AverageProfitPerMonth { get; set; }
        public MoneyModel AllIISExpenditureTotal { get; set; }
        public MoneyModel AllBrokerAccountExpenditureTotal { get; set; }
    }
}