namespace FamilyBudget.v3.App_CodeBase
{
    public static class Constants
    {
        public const string CURRENCY_RUB = "RUB";
        public const string CURRENCY_USD = "USD";
        public const string CURRENCY_EUR = "EUR";
        public const string CURRENCY_NOK = "NOK";

        public const string CURRENCY_NAME_RUB = "Российский рубль";

        public static class BondType
        {
            public const string BOND_TYPE_MINFIN = "minfin";
        }

        public static class InvestmentType
        {
            public const string INSTRUMENT_TYPE_CODE_STOCKS = "STOCKS";
            public const string INSTRUMENT_TYPE_CODE_ETF = "ETF";
            public const string INSTRUMENT_TYPE_CODE_BONDS = "BONDS";
        }

        public static class TinkoffStaticLinks
        {
            public const string TICKER_PAGE_URL_SEGMENT_BONDS = "bonds";
            public const string TICKER_PAGE_URL_SEGMENT_STOCKS = "stocks";
            public const string TICKER_PAGE_URL_SEGMENT_ETFS = "etfs";
            public const string TICKER_PAGE_URL_SEGMENT_CURRENCIES = "currencies";
            public const string AVATAR_IMAGE_LINK_FORMAT = "https://static.tinkoff.ru/brands/traiding/{0}x160.png";
            public const string TICKER_PAGE_LINK_FORMAT = "https://www.tinkoff.ru/invest/{0}/{1}";
        }
    }
}