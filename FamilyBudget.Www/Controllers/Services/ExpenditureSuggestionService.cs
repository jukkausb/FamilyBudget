using FamilyBudget.Www.Models.Repository.Interfaces;
using System.Linq;
using System.Collections.Generic;
using FamilyBudget.Www.App_DataModel;

namespace FamilyBudget.Www.Controllers.Services
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
            Account mainAccount = _accountRepository.GetAll().FirstOrDefault(a => a.IsMain);

            var allExpenditures = _expenditureRepository.Context.Expenditure.Where(i => i.AccountID == mainAccount.ID).ToList(); // TOO EXPENSIVE!

            Dictionary<string, int> wordWeights = new Dictionary<string, int>();
            Dictionary<string, int> expenditureDescriptionWeights = new Dictionary<string, int>();

            foreach (var expenditure in allExpenditures)
            {
                if (string.IsNullOrEmpty(expenditure.Description))
                    continue;

                List<string> desciptionWords = GetWords(expenditure.Description);
                AddToWordWeights(wordWeights, desciptionWords);
            }

            foreach (var expenditure in allExpenditures)
            {
                if (string.IsNullOrEmpty(expenditure.Description))
                    continue;

                int ExpenditureDesctiptionWeight = 0;
                List<string> desciptionWords = GetWords(expenditure.Description);
                foreach (var desciptionWord in desciptionWords)
                {
                    if (string.IsNullOrEmpty(desciptionWord))
                        continue;

                    ExpenditureDesctiptionWeight += wordWeights[desciptionWord];
                }

                // Handle equal descriptions
                if (expenditureDescriptionWeights.ContainsKey(expenditure.Description))
                {
                    expenditureDescriptionWeights[expenditure.Description] += ExpenditureDesctiptionWeight;
                }
                else
                {
                    expenditureDescriptionWeights.Add(expenditure.Description, ExpenditureDesctiptionWeight);
                }

            }

            return expenditureDescriptionWeights.OrderByDescending(g => g.Value).Take(topN).Select(s => s.Key).OrderBy(s => s).ToList();
        }
    }
}