using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using FamilyBudget.v3.App_Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using static FamilyBudget.v3.App_CodeBase.Tinkoff.Models.PortfolioCurrenciesExtended;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public interface ITinkoffPortfolioDataRetriever
    {
        List<PortfolioCurrencyExtended> GetTinkoffPortfolioCurrencies(string brokerAccountId = null);
        List<TinkoffPortfolioPosition> GetTinkoffPortfolioPositions(string brokerAccountId = null);
    }

    public class TinkoffPortfolioDataRetriever : TinkoffDataRetrieverBase, ITinkoffPortfolioDataRetriever
    {
        private const string CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_STOCKS = "TinkoffMarketInstrumentsStocks";
        private const string CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_ETFS = "TinkoffMarketInstrumentsEtfs";
        private const string CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_BONDS = "TinkoffMarketInstrumentsBonds";
        private const string CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_CURRENCIES = "TinkoffMarketInstrumentsCurrencies";

        public TinkoffPortfolioDataRetriever(string token) : base(token)
        {
        }

        public List<PortfolioCurrencyExtended> GetTinkoffPortfolioCurrencies(string brokerAccountId = null)
        {
            var context = GetContext();

            var portfolioCurrencies = string.IsNullOrEmpty(brokerAccountId) ?
                Task.Run(() => context.PortfolioCurrenciesAsync()).Result :
                Task.Run(() => context.PortfolioCurrenciesAsync(brokerAccountId)).Result;

            return portfolioCurrencies.Currencies;
        }

        public List<TinkoffPortfolioPosition> GetTinkoffPortfolioPositions(string brokerAccountId = null)
        {
            var context = GetContext();

            var portfolio = string.IsNullOrEmpty(brokerAccountId) ?
                Task.Run(() => context.PortfolioAsync()).Result :
                Task.Run(() => context.PortfolioAsync(brokerAccountId)).Result;

            List<TinkoffPortfolioPosition> portfolioPositions = new List<TinkoffPortfolioPosition>();
            if (portfolio == null)
            {
                return portfolioPositions;
            }

            var marketInstruments = GetMarketInstruments();

            foreach (var position in portfolio.Positions)
            {
                var portfolioStock = marketInstruments.FirstOrDefault(s => s.Ticker == position.Ticker);

                string currency = position.AveragePositionPrice.Currency.ToString().ToUpper();
                decimal currentTotalInPortfolio = position.Balance * position.AveragePositionPrice.Value + position.ExpectedYield.Value;
                decimal currentPriceInMarket = currentTotalInPortfolio / position.Balance;

                var portfolioPosition = new TinkoffPortfolioPosition
                {
                    Name = portfolioStock?.Name,
                    Type = position.InstrumentType,
                    Ticker = portfolioStock?.Ticker,
                    Isin = TinkoffIsinOverride.ResolveIsin(portfolioStock?.Ticker, portfolioStock?.Isin),
                    Lots = position.Lots,
                    Balance = position.Balance,
                    Currency = currency,
                    CurrentPriceInMarket = currentPriceInMarket,
                    CurrentPriceInPortfolio = position.AveragePositionPrice.Value,
                    CurrentTotalInPortfolio = currentTotalInPortfolio,
                    CurrentDelta = position.ExpectedYield.Value,
                    CurrentDeltaType = GetPositionDeltaType(position.ExpectedYield.Value),
                    CurrentDeltaPercent = Math.Round(Math.Abs(position.ExpectedYield.Value / (position.Balance * position.AveragePositionPrice.Value) * 100), 2).ToString("N2")
                };

                portfolioPositions.Add(portfolioPosition);
            }

            return portfolioPositions;
        }

        private List<MarketInstrument> GetMarketInstruments()
        {
            var context = GetContext();

            var marketInstrumentsCurrencies = CacheHelper.Get<List<MarketInstrument>>(CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_CURRENCIES);
            if (marketInstrumentsCurrencies == null)
            {
                var marketCurrencies = Task.Run(() => context.MarketCurrenciesAsync()).Result;
                marketInstrumentsCurrencies = marketCurrencies.Instruments;
                CacheHelper.Add(marketCurrencies.Instruments, CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_CURRENCIES);
            }

            var marketInstrumentsEtfs = CacheHelper.Get<List<MarketInstrument>>(CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_ETFS);
            if (marketInstrumentsEtfs == null)
            {
                var marketEtfs = Task.Run(() => context.MarketEtfsAsync()).Result;
                marketInstrumentsEtfs = marketEtfs.Instruments;
                CacheHelper.Add(marketEtfs.Instruments, CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_ETFS);
            }

            var marketInstrumentsBonds = CacheHelper.Get<List<MarketInstrument>>(CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_BONDS);
            if (marketInstrumentsBonds == null)
            {
                var marketBonds = Task.Run(() => context.MarketBondsAsync()).Result;
                marketInstrumentsBonds = marketBonds.Instruments;
                CacheHelper.Add(marketBonds.Instruments, CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_BONDS);
            }

            var marketInstrumentsStocks = CacheHelper.Get<List<MarketInstrument>>(CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_STOCKS);
            if (marketInstrumentsStocks == null)
            {
                var marketStocks = Task.Run(() => context.MarketStocksAsync()).Result;
                marketInstrumentsStocks = marketStocks.Instruments;
                CacheHelper.Add(marketStocks.Instruments, CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_STOCKS);
            }

            var marketInstruments = new List<MarketInstrument>();

            if (marketInstrumentsStocks != null)
            {
                marketInstruments.AddRange(marketInstrumentsStocks);
            }
            if (marketInstrumentsBonds != null)
            {
                marketInstruments.AddRange(marketInstrumentsBonds);
            }
            if (marketInstrumentsEtfs != null)
            {
                marketInstruments.AddRange(marketInstrumentsEtfs);
            }
            if (marketInstrumentsCurrencies != null)
            {
                marketInstruments.AddRange(marketInstrumentsCurrencies);
            }

            return marketInstruments;
        }

        private DeltaType GetPositionDeltaType(decimal deltaValue)
        {
            if (deltaValue > 0)
            {
                return DeltaType.Positive;
            }

            if (deltaValue < 0)
            {
                return DeltaType.Negative;
            }

            return DeltaType.Neutral;
        }
    }
}