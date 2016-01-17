using FamilyBudget.Www.App_DataModel;
using System.Collections.Generic;

namespace FamilyBudget.Www.App_CodeBase
{
    public class Prosperity
    {
        // Period
        public int Month { get; set; }
        public int Year { get; set; }

        // Все доходы до текщего периода включительно
        public List<Income> Incomes { get; set; }

        // Все затраты до текщего периода включительно
        public List<Expenditure> Expenditures { get; set; }

        // Стоимость унции золота как эквивалента благосостояния (в рублях) в данном периоде
        public decimal GoldOunce { get; set; }

        // Общее накопление с учетом всех счетов в данный период времени
        public decimal Wealth { get; set; }

        // Общее благосостояние с учетом всех счетов в данный период времени и стоимости золота (как эквивалента)
        public decimal ProsperityValue { get; set; }
    }
}