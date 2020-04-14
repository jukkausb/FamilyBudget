using System.Collections.Generic;
using Tinkoff.Trading.OpenApi.Models;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public static class TinkoffStaticUrlResolver
    {
        private static readonly Dictionary<string, string> _isinOverrideLookup;
        private static readonly Dictionary<string, string> _tickerOverrideLookup;

        static TinkoffStaticUrlResolver()
        {
            _isinOverrideLookup = new Dictionary<string, string>()
            {
                { "TCSG", "US87238U2033" },
                { "SBERP", "RU0009029540" },
                { "FXTB", "IE00B84D7P43" },
                { "USD000UTSTOM", "USD" },
                { "SU24020RMFS8", Constants.BondType.BOND_TYPE_MINFIN }
            };

            _tickerOverrideLookup = new Dictionary<string, string>()
            {
                { "USD000UTSTOM", "USDRUB" }
            };
        }

        private static string ResolveIsin(string ticker, string isin)
        {
            if (string.IsNullOrEmpty(ticker))
            {
                return isin;
            }

            if (_isinOverrideLookup.ContainsKey(ticker))
            {
                return _isinOverrideLookup[ticker];
            }

            return isin;
        }

        private static string ResolveTicker(string ticker)
        {
            if (string.IsNullOrEmpty(ticker))
            {
                return ticker;
            }

            if (_tickerOverrideLookup.ContainsKey(ticker))
            {
                return _tickerOverrideLookup[ticker];
            }

            return ticker;
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

        public static string ResolveAvatarImageLink(string ticker, string isin)
        {
            string id = ResolveIsin(ticker, isin);
            return string.Format(Constants.TinkoffStaticLinks.AVATAR_IMAGE_LINK_FORMAT, id);
        }

        public static string ResolveTickerPageLink(string ticker, InstrumentType instrumentType)
        {
            string tickerPageType = ResolveTickerPageType(instrumentType);
            string tickerOverride = ResolveTicker(ticker);
            return string.Format(Constants.TinkoffStaticLinks.TICKER_PAGE_LINK_FORMAT, tickerPageType, tickerOverride);
        }
    }
}