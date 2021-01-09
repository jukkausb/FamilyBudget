using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.App_Helpers;
using System;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff.Models
{
    public class TinkoffPortfolioPosition : IPieDiagramDataItem
    {
        public string Ticker { get; set; }
        public string Isin { get; set; }
        public string Figi { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Provided by Tinkoff
        /// </summary>
        public InstrumentType Type { get; set; }
        /// <summary>
        /// Position without price change (typically brocker account currency, RUB)
        /// </summary>
        public bool IsStatic { get; set; }
        /// <summary>
        /// Amount of lots
        /// </summary>
        public int Lots { get; set; }
        /// <summary>
        /// Amount of stocks
        /// </summary>
        public double Balance { get; set; }
        /// <summary>
        /// Currency of the position
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Current price of the single stock on market
        /// </summary>
        public double CurrentPriceInMarket { get; set; }
        public string CurrentPriceInMarketPresentation
        {
            get
            {
                return CurrentPriceInMarket.ToCurrencyDisplay(Currency);
            }
        }
        /// <summary>
        /// Current price of the single stock in portfolio
        /// </summary>
        public double CurrentPriceInPortfolio { get; set; }
        public string CurrentPriceInPortfolioPresentation
        {
            get
            {
                return CurrentPriceInPortfolio.ToCurrencyDisplay(Currency);
            }
        }

        /// <summary>
        /// Current percent of the position in portfolio
        /// </summary>
        public double CurrentPercentInPortfolio { get; set; }
        public string CurrentPercentInPortfolioPresentation
        {
            get
            {
                return CurrentPercentInPortfolio.ToCurrencyDisplay(Currency);
            }
        }

        /// <summary>
        /// Current total price of the position in portfolio
        /// </summary>
        public double CurrentTotalInPortfolio { get; set; }
        public string CurrentTotalInPortfolioPresentation
        {
            get
            {
                return CurrentTotalInPortfolio.ToCurrencyDisplay(Currency);
            }
        }
        public double CurrentDelta { get; set; }
        public string CurrentDeltaPresentation
        {
            get
            {
                return Math.Abs(CurrentDelta).ToCurrencyDisplay(Currency);
            }
        }
        public DeltaType CurrentDeltaType { get; set; }
        public string CurrentDeltaPercent { get; set; }

        public string DiagramBackgroundColor { get; set; }
        public string DiagramBackgroundHoverColor { get; set; }
        public string DiagramHoverBorderColor { get; set; }

        #region Custom attributes

        public string AvatarImageLink { get; set; }
        public string TickerPageLink { get; set; }

        /// <summary>
        /// Custom attribute - Market
        /// </summary>
        public InvestmentInstrumentMarket CustomMarket { get; set; }

        /// <summary>
        /// Custom attribute - Type
        /// This may be different from Tinkoff instrument type.
        /// It is used for better presentation on diagrams
        /// </summary>
        public InvestmentInstrumentType CustomType { get; set; }

        #endregion
    }
}