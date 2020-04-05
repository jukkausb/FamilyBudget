using System.Collections.Generic;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public static class TinkoffIsinOverride
    {
        private static readonly Dictionary<string, string> _isinOverrideLookup;

        static TinkoffIsinOverride()
        {
            _isinOverrideLookup = new Dictionary<string, string>()
            {
                { "TCSG", "US87238U2033" },
                { "SBERP", "RU0009029540" },
                { "FXTB", "IE00B84D7P43" },
                { "USD000UTSTOM", "USD" }
            };
        }

        public static string ResolveIsin(string ticker, string isin)
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
    }
}