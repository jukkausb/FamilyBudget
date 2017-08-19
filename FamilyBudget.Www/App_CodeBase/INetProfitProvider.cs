using FamilyBudget.Www.App_DataModel;
using System.Collections.Generic;

namespace FamilyBudget.Www.App_CodeBase
{
    public interface INetProfitProvider
    {
        decimal CalculateNetProfit(List<Account> accounts, List<Income> allIncomesInMonth, List<Expenditure> allExpenditresInMonth, string mainCurrencyCode);
    }
}
