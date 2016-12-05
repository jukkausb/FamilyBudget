using System.Collections.Generic;

namespace FamilyBudget.Www.Models.Spa.Shared
{
    public class SiteNavigationViewModel
    {
        public SiteNavigationViewModel()
        {
            LinkContainers = new List<LinkContainerModel>();
        }
        public List<LinkContainerModel> LinkContainers { get; set; }
    }
}