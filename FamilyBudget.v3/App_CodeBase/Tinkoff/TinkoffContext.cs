using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using System.Threading.Tasks;
using System.Web;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public class TinkoffContext : Context
    {
        public TinkoffContext(IConnection<Context> connection) : base(connection)
        {
        }

        public async Task<TinkoffBrokerAccounts> UserAccountsAsync()
        {
            var response = await Connection.
                SendGetRequestAsync<TinkoffBrokerAccounts>(Endpoints.UserAccounts).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<Portfolio> PortfolioAsync(string brokerAccountId)
        {
            var brokerAccountIdParam = HttpUtility.UrlEncode(brokerAccountId);
            var path = $"{Endpoints.Portfolio}?brokerAccountId={brokerAccountIdParam}";
            var response = await Connection.SendGetRequestAsync<Portfolio>(path).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<PortfolioCurrencies> PortfolioCurrenciesAsync(string brokerAccountId)
        {
            var brokerAccountIdParam = HttpUtility.UrlEncode(brokerAccountId);
            var path = $"{Endpoints.PortfolioCurrency}?brokerAccountId={brokerAccountIdParam}";
            var response = await Connection.SendGetRequestAsync<PortfolioCurrencies>(path).ConfigureAwait(false);
            return response?.Payload;
        }

        private static class Endpoints
        {
            public const string Portfolio = "portfolio";
            public const string PortfolioCurrency = "portfolio/currencies";
            public const string UserAccounts = "user/accounts";
        }
    }
}