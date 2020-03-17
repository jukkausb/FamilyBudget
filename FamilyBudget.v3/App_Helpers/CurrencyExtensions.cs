using System;
using System.Globalization;
using System.Linq;

namespace FamilyBudget.v3.App_Helpers
{
    public static class CurrencyExtensions
    {
        public static string ToCurrencySymbol(this string isoCurrencyCode)
        {
            return CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture)
                .Select(culture =>
                {
                    try
                    {
                        return new RegionInfo(culture.LCID);
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(ri => ri != null && ri.ISOCurrencySymbol == isoCurrencyCode)
                .Select(ri => ri.CurrencySymbol)
                .FirstOrDefault();
        }

        public static string ToCurrencyDisplay(this decimal d, string currency, bool forceRound2Decimals = false)
        {
            string format;
            if (forceRound2Decimals)
            {
                format = "N2";
            }
            else
            {
                format = "N" + GetDecimals(d).ToString();
            }
            return d.ToString(format) + " " + ToCurrencySymbol(currency);
        }

        private static int GetDecimals(decimal d, int i = 2)
        {
            decimal multiplied = (decimal)((double)d * Math.Pow(10, i));
            if (Math.Round(multiplied) == multiplied)
            {
                if (i > 5) return 5;
                return i;
            }
                
            return GetDecimals(d, i + 1);
        }
    }
}