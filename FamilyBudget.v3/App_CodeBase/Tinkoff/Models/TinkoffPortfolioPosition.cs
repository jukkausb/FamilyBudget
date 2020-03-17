using FamilyBudget.v3.App_Helpers;
using System;
using Tinkoff.Trading.OpenApi.Models;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff.Models
{
    public class TinkoffPortfolioPosition
    {
        public string Name { get; set; }
        public InstrumentType Type { get; set; }
        public string Ticker { get; set; }
        public int Lots { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public decimal CurrentPriceInMarket { get; set; }
        public string CurrentPriceInMarketPresentation
        {
            get
            {
                return CurrentPriceInMarket.ToCurrencyDisplay(Currency);
            }
        }
        public decimal CurrentPriceInPortfolio { get; set; }
        public string CurrentPriceInPortfolioPresentation
        {
            get
            {
                return CurrentPriceInPortfolio.ToCurrencyDisplay(Currency);
            }
        }
        public decimal CurrentTotalInPortfolio { get; set; }
        public string CurrentTotalInPortfolioPresentation
        {
            get
            {
                return CurrentTotalInPortfolio.ToCurrencyDisplay(Currency);
            }
        }
        public decimal CurrentDelta { get; set; }
        public string CurrentDeltaPresentation
        {
            get
            {
                return Math.Abs(CurrentDelta).ToCurrencyDisplay(Currency);
            }
        }
        public DeltaType CurrentDeltaType { get; set; }
        public string CurrentDeltaPercent { get; set; }
    }

    public enum DeltaType
    {
        Positive,
        Negative,
        Neutral
    }
}