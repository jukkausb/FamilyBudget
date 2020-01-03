using System.Collections.Generic;

namespace FamilyBudget.v3.Models.Spa.Shared
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