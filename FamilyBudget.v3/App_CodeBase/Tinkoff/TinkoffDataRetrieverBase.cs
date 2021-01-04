
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public abstract class TinkoffDataRetrieverBase
    {
        private readonly ITinkoffInvestApiClient _tinkoffInvestApiClient;

        protected string Token { get; }

        public TinkoffDataRetrieverBase(string token)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _tinkoffInvestApiClient = new TinkoffInvestApiClient(httpClient);
        }

        public async Task<PortfolioCurrenciesResponse> PortfolioCurrenciesAsync(string brokerAccountId)
        {
            return await _tinkoffInvestApiClient.CurrenciesAsync(brokerAccountId);
        }

        public async Task<PortfolioResponse> PortfolioAsync(string brokerAccountId)
        {
            return await _tinkoffInvestApiClient.PortfolioAsync(brokerAccountId);
        }

        public async Task<MarketInstrumentListResponse> MarketCurrenciesAsync()
        {
            return await _tinkoffInvestApiClient.Currencies2Async();
        }

        public async Task<MarketInstrumentListResponse> MarketEtfsAsync()
        {
            return await _tinkoffInvestApiClient.EtfsAsync();
        }

        public async Task<MarketInstrumentListResponse> MarketBondsAsync()
        {
            return await _tinkoffInvestApiClient.BondsAsync();
        }

        public async Task<MarketInstrumentListResponse> MarketStocksAsync()
        {
            return await _tinkoffInvestApiClient.StocksAsync();
        }

        public async Task<UserAccountsResponse> AccountsAsync()
        {
            return await _tinkoffInvestApiClient.AccountsAsync();
        }
    }
}