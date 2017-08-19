using FamilyBudget.Www.Controllers;
using System.Collections.Generic;

namespace FamilyBudget.Www.App_CodeBase
{
    public interface ITrendCalculator
    {
        List<TrendLineMonthDefinition> CalculateTrend(List<TrendLineMonthDefinition> data);
    }
}
