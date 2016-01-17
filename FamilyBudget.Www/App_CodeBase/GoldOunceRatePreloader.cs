
using Microsoft.Practices.ServiceLocation;

namespace FamilyBudget.Www.App_CodeBase
{
    public static class GoldOunceRatePreloader
    {
        private static IGoldOunceRateProvider _goldOunceRateProvider;

        public static void Preload()
        {
            _goldOunceRateProvider = ServiceLocator.Current.GetInstance<IGoldOunceRateProvider>();

            _goldOunceRateProvider.DownloadGoldOunceRates("USD");
        }
    }
}