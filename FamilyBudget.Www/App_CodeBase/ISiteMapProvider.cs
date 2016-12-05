
using FamilyBudget.Www.Models.Spa.Shared;

namespace FamilyBudget.Www.App_CodeBase
{
    public interface ISiteMapProvider
    {
        SiteMap GetSiteMap();
        SiteNavigationViewModel GetSiteNavigationViewModel(SiteMap siteMap);
    }
}
