

namespace FamilyBudget.v3.App_CodeBase
{
    public interface ICurrencyProvider
    {
        void DownloadCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode);
        double GetSellCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode);
        void SetCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode, double value);
    }
}