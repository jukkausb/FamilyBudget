using FamilyBudget.Www.Models.Repository.Interfaces;
using System.Linq;
using System.Collections.Generic;
using FamilyBudget.Www.App_DataModel;

namespace FamilyBudget.Www.Controllers.Services
{
    public interface IIncomeSuggestionService
    {
        List<string> GetTopNSuggestions(int topN = 10);
    }

    public class IncomeSuggestionService : SuggestionServiceBase, IIncomeSuggestionService
    {
        private readonly IIncomeRepository _incomeRepository;
        private readonly IAccountRepository _accountRepository;

        public IncomeSuggestionService(IIncomeRepository incomeRepository, IAccountRepository accountRepository)
        {
            _incomeRepository = incomeRepository;
            _accountRepository = accountRepository;
        }

        public List<string> GetTopNSuggestions(int topN = 10)
        {
            Account mainAccount = _accountRepository.GetAll().FirstOrDefault(a => a.IsMain);

            var allIncomes = _incomeRepository.Context.Income.Where(i => i.AccountID == mainAccount.ID).ToList(); // TOO EXPENSIVE!

            Dictionary<string, int> wordWeights = new Dictionary<string, int>();
            Dictionary<string, int> incomeDescriptionWeights = new Dictionary<string, int>();

            foreach (var income in allIncomes)
            {
                if (string.IsNullOrEmpty(income.Description))
                    continue;

                List<string> desciptionWords = GetWords(income.Description);
                AddToWordWeights(wordWeights, desciptionWords);
            }

            foreach (var income in allIncomes)
            {
                if (string.IsNullOrEmpty(income.Description))
                    continue;

                int incomeDesctiptionWeight = 0;
                List<string> desciptionWords = GetWords(income.Description);
                foreach (var desciptionWord in desciptionWords)
                {
                    if (string.IsNullOrEmpty(desciptionWord))
                        continue;

                    incomeDesctiptionWeight += wordWeights[desciptionWord];
                }

                // Handle equal descriptions
                if (incomeDescriptionWeights.ContainsKey(income.Description))
                {
                    incomeDescriptionWeights[income.Description] += incomeDesctiptionWeight;
                }
                else
                {
                    incomeDescriptionWeights.Add(income.Description, incomeDesctiptionWeight);
                }

            }

            return incomeDescriptionWeights.OrderByDescending(g => g.Value).Take(topN).Select(s => s.Key).OrderBy(s => s).ToList();
        }
    }
}