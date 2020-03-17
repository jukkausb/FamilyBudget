
using Tinkoff.Trading.OpenApi.Network;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public abstract class TinkoffDataRetrieverBase
    {
        protected string Token { get; }

        public TinkoffDataRetrieverBase(string token)
        {
            Token = token;
        }

        protected TinkoffContext GetContext()
        {
            var connection = ConnectionFactory.GetConnection(Token);
            return new TinkoffContext(connection);
        }
    }
}