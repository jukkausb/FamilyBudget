
using FamilyBudget.v3.Models.Spa.Shared;

namespace FamilyBudget.v3.App_CodeBase
{
    public interface ISiteMapProvider
    {
        SiteMap GetSiteMap();
        SiteNavigationViewModel GetSiteNavigationViewModel(SiteMap siteMap);
    }
}
