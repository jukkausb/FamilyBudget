using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using FamilyBudget.v3.App_DataModel;
using System.Collections.Generic;
using Tinkoff.Trading.OpenApi.Models;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public static class TinkoffStaticUrlResolver
    {
        private static string ResolveInstrumentAvatarId(TinkoffPortfolioPosition portfolioPosition, InvestmentInstrument investmentInstrument)
        {
            if (portfolioPosition == null || string.IsNullOrEmpty(portfolioPosition.Ticker))
            {
                return "";
            }

            if (investmentInstrument == null)
            {
                return portfolioPosition.Isin;
            }

            if (!string.IsNullOrEmpty(investmentInstrument.ExternalAvatarIsinOverride))
            {
                return investmentInstrument.ExternalAvatarIsinOverride;
            }

            return portfolioPosition.Isin;
        }

        private static string ResolveInstrumentPageId(TinkoffPortfolioPosition portfolioPosition, InvestmentInstrument investmentInstrument)
        {
            if (portfolioPosition == null || string.IsNullOrEmpty(portfolioPosition.Ticker))
            {
                return "";
            }

            if (investmentInstrument == null)
            {
                return portfolioPosition.Ticker;
            }

            if (!string.IsNullOrEmpty(investmentInstrument.ExternalPageTickerOverride))
            {
                return investmentInstrument.ExternalPageTickerOverride;
            }

            return portfolioPosition.Ticker;
        }

        private static string ResolveTickerPageType(InstrumentType instrumentType)
        {
            if (instrumentType == InstrumentType.Bond)
            {
                return Constants.TinkoffStaticLinks.TICKER_PAGE_URL_SEGMENT_BONDS;
            }

            if (instrumentType == InstrumentType.Stock)
            {
                return Constants.TinkoffStaticLinks.TICKER_PAGE_URL_SEGMENT_STOCKS;
            }

            if (instrumentType == InstrumentType.Etf)
            {
                return Constants.TinkoffStaticLinks.TICKER_PAGE_URL_SEGMENT_ETFS;
            }

            if (instrumentType == InstrumentType.Currency)
            {
                return Constants.TinkoffStaticLinks.TICKER_PAGE_URL_SEGMENT_CURRENCIES;
            }

            return "";
        }

        public static string ResolveAvatarImageLink(TinkoffPortfolioPosition portfolioPosition, InvestmentInstrument investmentInstrument)
        {
            string id = ResolveInstrumentAvatarId(portfolioPosition, investmentInstrument);
            return string.Format(Constants.TinkoffStaticLinks.AVATAR_IMAGE_LINK_FORMAT, id);
        }

        public static string ResolveExternalPageId(TinkoffPortfolioPosition portfolioPosition, InvestmentInstrument investmentInstrument)
        {
            if (investmentInstrument == null || investmentInstrument == null)
            {
                return null;
            }

            string tickerPageType = ResolveTickerPageType(portfolioPosition.Type);
            string tickerOverride = ResolveInstrumentPageId(portfolioPosition, investmentInstrument);
            return string.Format(Constants.TinkoffStaticLinks.TICKER_PAGE_LINK_FORMAT, tickerPageType, tickerOverride);
        }
    }
}