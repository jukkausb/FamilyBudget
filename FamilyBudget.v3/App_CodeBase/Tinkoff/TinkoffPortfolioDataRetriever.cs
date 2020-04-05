using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using static Tinkoff.Trading.OpenApi.Models.PortfolioCurrencies;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public interface ITinkoffPortfolioDataRetriever
    {
        Task<List<PortfolioCurrency>> GetTinkoffPortfolioCurrencies(string brokerAccountId = null);
        Task<List<TinkoffPortfolioPosition>> GetTinkoffPortfolioPositions(string brokerAccountId = null);
    }

    public class TinkoffPortfolioDataRetriever : TinkoffDataRetrieverBase, ITinkoffPortfolioDataRetriever
    {
        public TinkoffPortfolioDataRetriever(string token) : base(token)
        {
        }

        public async Task<List<PortfolioCurrency>> GetTinkoffPortfolioCurrencies(string brokerAccountId = null)
        {
            var context = GetContext();

            var portfolioCurrencies = string.IsNullOrEmpty(brokerAccountId) ?
                await context.PortfolioCurrenciesAsync() :
                await context.PortfolioCurrenciesAsync(brokerAccountId);

            return portfolioCurrencies.Currencies;
        }

        public async Task<List<TinkoffPortfolioPosition>> GetTinkoffPortfolioPositions(string brokerAccountId = null)
        {
            var context = GetContext();

            var portfolio = string.IsNullOrEmpty(brokerAccountId) ?
                await context.PortfolioAsync() :
                await context.PortfolioAsync(brokerAccountId);

            List<TinkoffPortfolioPosition> portfolioPositions = new List<TinkoffPortfolioPosition>();
            if (portfolio == null)
            {
                return portfolioPositions;
            }

            var marketInstruments = await GetMarketInstruments();

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
                    Ticker = portfolioStock.Ticker,
                    Isin = TinkoffIsinOverride.ResolveIsin(portfolioStock.Ticker, portfolioStock.Isin),
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

        private async Task<List<MarketInstrument>> GetMarketInstruments()
        {
            // Add caching
            var context = GetContext();

            var marketStocks = await context.MarketStocksAsync();
            var marketBonds = await context.MarketBondsAsync();
            var marketEtfs = await context.MarketEtfsAsync();
            var marketCurrencies = await context.MarketCurrenciesAsync();

            var marketInstruments = new List<MarketInstrument>();

            if (marketStocks?.Instruments != null)
            {
                marketInstruments.AddRange(marketStocks.Instruments);
            }
            if (marketBonds?.Instruments != null)
            {
                marketInstruments.AddRange(marketBonds.Instruments);
            }
            if (marketEtfs?.Instruments != null)
            {
                marketInstruments.AddRange(marketEtfs.Instruments);
            }
            if (marketCurrencies?.Instruments != null)
            {
                marketInstruments.AddRange(marketCurrencies.Instruments);
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