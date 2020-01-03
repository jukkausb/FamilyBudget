using System.Collections.Generic;

namespace FamilyBudget.v3.Models.Spa.Shared
{
    public class LinkContainerModel
    {
        public LinkContainerModel()
        {
            Links = new List<LinkModel>();
        }
        public string Title { get; set; }
        public string Href { get; set; }
        public string CssModifier { get; set; }
        public List<LinkModel> Links { get; set; }
    }
}