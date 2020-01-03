using System.Collections.Generic;
using System.Linq;

namespace FamilyBudget.v3.Controllers.Services
{
    public class SuggestionServiceBase
    {
        protected List<string> GetWords(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new List<string>();
            }

            return str.Split(' ').ToList();
        }

        protected void AddToWordWeights(Dictionary<string, int> wordWeights, List<string> words)
        {
            foreach (var word in words)
            {
                if (string.IsNullOrEmpty(word))
                    continue;

                if (wordWeights.ContainsKey(word))
                {
                    wordWeights[word]++;
                }
                else
                {
                    wordWeights.Add(word, 1);
                }
            }
        }
    }
}