using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.Models.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FamilyBudget.v3.App_Helpers
{
    public static class BusinessHelper
    {
        /// <summary>
        /// Не включает доходы от обмена валют и др.
        /// </summary>
        public static IQueryable<Income> GetRealIncomes(IIncomeRepository incomeRepository)
        {
            return incomeRepository.GetAll().Where(e =>
                e.IncomeCategory.ID != 6 /* Обмен валют */
                );
        }

        /// <summary>
        /// Не включает расходы на ИИС или др. которые не считаются расходами
        /// </summary>
        public static IQueryable<Expenditure> GetRealExpenditures(IExpenditureRepository expenditureRepository)
        {
            return expenditureRepository.GetAll().Where(e =>
                e.ExpenditureCategory.ID != 58 && /* ИИС */
                e.ExpenditureCategory.ID != 59 && /* Брокерский счет*/
                e.ExpenditureCategory.ID != 60 /* Обмен валют */
                );
        }

        /// <summary>
        /// Расходы на ИИС
        /// </summary>
        public static IQueryable<Expenditure> GetIISExpenditures(IExpenditureRepository expenditureRepository)
        {
            return expenditureRepository.GetAll().Where(e =>
                e.ExpenditureCategory.ID == 58
                );
        }

        /// <summary>
        /// Расходы на Брокерский счет
        /// </summary>
        public static IQueryable<Expenditure> GetBrokerAccountExpenditures(IExpenditureRepository expenditureRepository)
        {
            return expenditureRepository.GetAll().Where(e =>
                e.ExpenditureCategory.ID == 59
                );
        }

        public static DeltaType GetDeltaType(double deltaValue)
        {
            if (deltaValue > 0)
            {
                return DeltaType.Positive;
            }

            if (deltaValue < 0)
            {
                return DeltaType.Negative;
            }

            return DeltaType.Neutral;
        }
    }
}