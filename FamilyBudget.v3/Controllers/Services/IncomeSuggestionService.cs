using System.Collections.Generic;
using System.Globalization;
using System;

namespace FamilyBudget.v3.Controllers.Services
{
    public interface IIncomeSuggestionService
    {
        List<string> GetTopNSuggestions(int topN = 10);
    }

    public class IncomeSuggestionService : IIncomeSuggestionService
    {
        public List<string> GetTopNSuggestions(int topN = 10)
        {
            DateTime lastMonth = DateTime.Now.AddMonths(-1);

            string previousMonthName = lastMonth.ToString("MMMM", CultureInfo.CurrentCulture).ToLower();
            string previousMonthYear = lastMonth.ToString("yyyy", CultureInfo.CurrentCulture);

            return new List<string>
            {
                $"ЗП за {previousMonthName} {previousMonthYear} (1 часть)",
                $"ЗП за {previousMonthName} {previousMonthYear} (остаток)",
                "Жена-тренер",
                "Обмен валюты",
                "Подарок"
            };
        }
    }
}