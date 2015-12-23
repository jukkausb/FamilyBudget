
namespace FamilyBudget.Www.App_CodeBase.Csv
{
    public interface ICurrencyProvider
    {
        CurrencyRate[] DownloadCurrencyRates(string sellCurrencyCode, string purchaseCurrencyCode);
        CurrencyRate GetCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode);
        decimal GetSellCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode);
    }
}