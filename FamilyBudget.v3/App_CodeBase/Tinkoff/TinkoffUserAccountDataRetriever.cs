using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public interface ITinkoffUserAccountDataRetriever
    {
        Task<List<TinkoffBrokerAccount>> GetTinkoffUserAccounts();
    }

    public class TinkoffUserAccountDataRetriever : TinkoffDataRetrieverBase, ITinkoffUserAccountDataRetriever
    {
        public TinkoffUserAccountDataRetriever(string token) : base(token)
        {
        }

        public async Task<List<TinkoffBrokerAccount>> GetTinkoffUserAccounts()
        {
            var context = GetContext();
            var accounts = await context.UserAccountsAsync();
            return accounts.Accounts;
        }
    }
}