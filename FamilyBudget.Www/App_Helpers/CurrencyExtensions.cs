using System.Globalization;
using System.Linq;

namespace FamilyBudget.Www.App_Helpers
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
    }
}