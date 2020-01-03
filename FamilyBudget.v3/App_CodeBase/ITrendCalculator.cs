using FamilyBudget.v3.Controllers;
using System.Collections.Generic;

namespace FamilyBudget.v3.App_CodeBase
{
    public interface ITrendCalculator
    {
        List<TrendLineMonthDefinition> CalculateTrend(List<TrendLineMonthDefinition> data);
    }
}
