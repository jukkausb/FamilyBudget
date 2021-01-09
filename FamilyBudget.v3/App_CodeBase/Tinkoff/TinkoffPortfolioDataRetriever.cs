using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using FamilyBudget.v3.App_Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public interface ITinkoffPortfolioDataRetriever
    {
        List<CurrencyPosition> GetTinkoffPortfolioCurrencies(string brokerAccountId = null);
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

        public List<CurrencyPosition> GetTinkoffPortfolioCurrencies(string brokerAccountId = null)
        {
            var portfolioCurrencies = Task.Run(() => PortfolioCurrenciesAsync(brokerAccountId)).Result;
            return portfolioCurrencies.Payload.Currencies1.ToList();
        }

        public List<TinkoffPortfolioPosition> GetTinkoffPortfolioPositions(string brokerAccountId = null)
        {
            var portfolio = Task.Run(() => PortfolioAsync(brokerAccountId)).Result;

            List<TinkoffPortfolioPosition> portfolioPositions = new List<TinkoffPortfolioPosition>();
            if (portfolio == null)
            {
                return portfolioPositions;
            }

            var marketInstruments = GetMarketInstruments();

            foreach (var position in portfolio.Payload.Positions)
            {
                var portfolioInstrument = marketInstruments.FirstOrDefault(s => s.Ticker == position.Ticker);
                if (portfolioInstrument == null)
                {
                    continue;
                }

                string currency = position.AveragePositionPrice.Currency.ToString().ToUpper();
                double currentTotalInPortfolio = position.Balance * position.AveragePositionPrice.Value + position.ExpectedYield.Value;
                double currentPriceInMarket = currentTotalInPortfolio / position.Balance;

                var portfolioPosition = new TinkoffPortfolioPosition
                {
                    Name = portfolioInstrument.Name,
                    Type = position.InstrumentType,
                    Ticker = portfolioInstrument.Ticker,
                    Isin = portfolioInstrument.Isin,
                    Figi = portfolioInstrument.Figi,
                    Lots = position.Lots,
                    Balance = position.Balance,
                    Currency = currency,
                    CurrentPriceInMarket = currentPriceInMarket,
                    CurrentPriceInPortfolio = position.AveragePositionPrice.Value,
                    CurrentTotalInPortfolio = currentTotalInPortfolio,
                    CurrentDelta = position.ExpectedYield.Value,
                    CurrentDeltaType = BusinessHelper.GetDeltaType(position.ExpectedYield.Value),
                    CurrentDeltaPercent = Math.Round(Math.Abs(position.ExpectedYield.Value / (position.Balance * position.AveragePositionPrice.Value) * 100), 2).ToString("N2")
                };


                portfolioPositions.Add(portfolioPosition);
            }

            return portfolioPositions;
        }

        private List<MarketInstrument> GetMarketInstruments()
        {
            var marketInstrumentsCurrencies = CacheHelper.Get<IList<MarketInstrument>>(CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_CURRENCIES);
            if (marketInstrumentsCurrencies == null)
            {
                var marketCurrencies = Task.Run(() => MarketCurrenciesAsync()).Result;
                marketInstrumentsCurrencies = marketCurrencies.Payload.Instruments.ToList();
                CacheHelper.Add(marketCurrencies.Payload.Instruments, CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_CURRENCIES);
            }

            var marketInstrumentsEtfs = CacheHelper.Get<IList<MarketInstrument>>(CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_ETFS);
            if (marketInstrumentsEtfs == null)
            {
                var marketEtfs = Task.Run(() => MarketEtfsAsync()).Result;
                marketInstrumentsEtfs = marketEtfs.Payload.Instruments.ToList();
                CacheHelper.Add(marketEtfs.Payload.Instruments, CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_ETFS);
            }

            var marketInstrumentsBonds = CacheHelper.Get<IList<MarketInstrument>>(CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_BONDS);
            if (marketInstrumentsBonds == null)
            {
                var marketBonds = Task.Run(() => MarketBondsAsync()).Result;
                marketInstrumentsBonds = marketBonds.Payload.Instruments.ToList();
                CacheHelper.Add(marketBonds.Payload.Instruments, CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_BONDS);
            }

            var marketInstrumentsStocks = CacheHelper.Get<IList<MarketInstrument>>(CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_STOCKS);
            if (marketInstrumentsStocks == null)
            {
                var marketStocks = Task.Run(() => MarketStocksAsync()).Result;
                marketInstrumentsStocks = marketStocks.Payload.Instruments.ToList();
                CacheHelper.Add(marketStocks.Payload.Instruments, CACHE_KEY_TINKOFF_MARKET_INSTRUMENTS_STOCKS);
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
    }
}