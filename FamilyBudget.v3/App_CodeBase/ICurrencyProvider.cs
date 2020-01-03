

namespace FamilyBudget.v3.App_CodeBase
{
    public interface ICurrencyProvider
    {
        void DownloadCurrencyRates(string sellCurrencyCode, string purchaseCurrencyCode);
        decimal GetSellCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode);
    }
}