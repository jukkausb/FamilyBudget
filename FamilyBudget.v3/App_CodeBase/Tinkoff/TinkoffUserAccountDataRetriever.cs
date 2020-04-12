using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public interface ITinkoffUserAccountDataRetriever
    {
        List<TinkoffBrokerAccount> GetTinkoffUserAccounts();
    }

    public class TinkoffUserAccountDataRetriever : TinkoffDataRetrieverBase, ITinkoffUserAccountDataRetriever
    {
        public TinkoffUserAccountDataRetriever(string token) : base(token)
        {
        }

        public List<TinkoffBrokerAccount> GetTinkoffUserAccounts()
        {
            var context = GetContext();
            var accounts = Task.Run(() => context.UserAccountsAsync()).Result;
            return accounts.Accounts;
        }
    }
}