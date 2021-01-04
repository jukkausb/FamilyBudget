using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public interface ITinkoffUserAccountDataRetriever
    {
        List<UserAccount> GetTinkoffUserAccounts();
    }

    public class TinkoffUserAccountDataRetriever : TinkoffDataRetrieverBase, ITinkoffUserAccountDataRetriever
    {
        public TinkoffUserAccountDataRetriever(string token) : base(token)
        {
        }

        public List<UserAccount> GetTinkoffUserAccounts()
        {
            var accounts = Task.Run(() => AccountsAsync()).Result;
            return accounts.Payload.Accounts.ToList();
        }
    }
}