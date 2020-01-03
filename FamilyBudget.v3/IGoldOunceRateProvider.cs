
namespace FamilyBudget.v3
{
    public interface IGoldOunceRateProvider
    {
        void DownloadGoldOunceRates(string currency);
        decimal GetAverageGoldOunceRate(string currency, int year, int month);
    }
}