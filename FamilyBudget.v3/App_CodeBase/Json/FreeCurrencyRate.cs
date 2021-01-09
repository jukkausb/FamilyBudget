namespace FamilyBudget.v3.App_CodeBase.Json
{
    public class FreeCurrencyRate
    {
        public const string RateSourceFormatString = "http://free.currencyconverterapi.com/api/v4/convert?q={0}_{1}&compact=ultra&apiKey=e32c41ca741c5a055bbf";
        public const string CurrencyRateCacheKeyFormat = "FreeCurrencyRateCacheKey_{0}_{1}";

        public double Val { get; set; }
    }
}