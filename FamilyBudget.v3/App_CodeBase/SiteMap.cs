
using System.Collections.Generic;
using System.Web;

namespace FamilyBudget.v3.App_CodeBase
{
    public class Sitemap
    {
        public List<SitemapEntry> MenuList { get; set; }
    }

    public class SitemapEntry
    {
        public string Id { get; set; }
        public List<SitemapEntry> Children { get; set; }
        public bool IsDivider { get; set; }
        public string IconCssClass { get; set; }
        public string Url { get; set; }
        public string Path
        {
            get
            {
                if (!Url.Contains("?"))
                {
                    return Url;
                }

                var urlSegments = Url.Split('?');
                return urlSegments[0];
            }
        }
        public string Title { get; set; }
        public bool IsActive
        {
            get
            {
                // Ugly but fast
                return HttpContext.Current.Request.Url.AbsolutePath.EndsWith(this.Path);
            }
        }
    }
}