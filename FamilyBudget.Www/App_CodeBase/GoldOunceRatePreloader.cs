

namespace FamilyBudget.Www.App_CodeBase
{
    public class GoldOunceRatePreloader
    {
        private readonly IGoldOunceRateProvider _goldOunceRateProvider;

        public GoldOunceRatePreloader(IGoldOunceRateProvider goldOunceRateProvider)
        {
            _goldOunceRateProvider = goldOunceRateProvider;
        }

        public void Preload()
        {
            _goldOunceRateProvider.DownloadGoldOunceRates("USD");
        }
    }
}