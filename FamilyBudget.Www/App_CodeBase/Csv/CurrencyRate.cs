using FileHelpers;

namespace FamilyBudget.Www.App_CodeBase.Csv
{
    /* 
     * "USDEUR=X",0.7356,"7/5/2014","7:23am",0.7355,0.7357
     * http://finance.yahoo.com/d/quotes.csv?s=USDEUR=X&f=sl1d1t1ba&e=.csv 
    */

    [DelimitedRecord(",")]
    public class CurrencyRate
    {
        public const string RateSourceFormatString =
            "http://finance.yahoo.com/d/quotes.csv?s={0}{1}=X&f=sl1d1t1ba&e=.csv";

        public const string CurrencyRateCacheKeyFormat = "CurrencyRateCacheKey_{0}_{1}";

        public string ConversionPath { get; set; }
        public decimal MainRate { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public decimal SellRate { get; set; }
        public decimal PurchaseRate { get; set; }
    }
}