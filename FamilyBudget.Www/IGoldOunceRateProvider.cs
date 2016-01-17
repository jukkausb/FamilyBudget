
namespace FamilyBudget.Www
{
    public interface IGoldOunceRateProvider
    {
        void DownloadGoldOunceRates(string currency);
        decimal GetAverageGoldOunceRate(string currency, int year, int month);
    }
}