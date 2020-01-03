using System;
using FileHelpers;

namespace FamilyBudget.v3.App_CodeBase.Csv
{
    /* 
     * "USDEUR=X",0.7356,"7/5/2014","7:23am",0.7355,0.7357
     * http://finance.yahoo.com/d/quotes.csv?s=USDEUR=X&f=sl1d1t1ba&e=.csv 
    */

    [IgnoreFirst(1)]
    [DelimitedRecord(",")]
    public class GoldOunceRate
    {
        public const string RateSourceString = "http://www.quandl.com/api/v1/datasets/BUNDESBANK/BBK01_WT5511.csv";

        public const string CurrencyRateCacheKeyFormat = "GoldOunceRateCacheKey_{0}";

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime Date;
        public decimal Rate;

    }
}