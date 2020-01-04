using FamilyBudget.v3.Models.Repository.Interfaces;
using System.Linq;
using System.Collections.Generic;
using FamilyBudget.v3.App_DataModel;
using System;

namespace FamilyBudget.v3.Controllers.Services
{
    public interface IExpenditureSuggestionService
    {
        List<string> GetTopNSuggestions(int topN = 10);
    }

    public class ExpenditureSuggestionService : SuggestionServiceBase, IExpenditureSuggestionService
    {
        private readonly IExpenditureRepository _expenditureRepository;
        private readonly IAccountRepository _accountRepository;

        public ExpenditureSuggestionService(IExpenditureRepository expenditureRepository, IAccountRepository accountRepository)
        {
            _expenditureRepository = expenditureRepository;
            _accountRepository = accountRepository;
        }

        public List<string> GetTopNSuggestions(int topN = 10)
        {
            return new List<string>
            {
                "АШАН",
                "Привоз",
                "Бензин",
                "Кофе",
                "Еда",
                $"Налоги за * квартал {DateTime.Now.Year}",
                "Квартплата, Коммуналка + Кап.ремонт",
                "Инвестирование"
            };
        }
    }
}