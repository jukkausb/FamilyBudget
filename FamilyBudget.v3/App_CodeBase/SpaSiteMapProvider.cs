using System.Collections.Generic;
using FamilyBudget.v3.Models.Spa.Shared;

namespace FamilyBudget.v3.App_CodeBase
{
    public class SpaSiteMapProvider : ISiteMapProvider
    {
        public SiteMap GetSiteMap()
        {
            return new SiteMap()
            {
                PageUrlDefault = "/Spa",
                PageUrlIncomes = "/Spa/Income",
                PageUrlExpenditures = "/Spa/Expenditure",
            };
        }

        public SiteNavigationViewModel GetSiteNavigationViewModel(SiteMap siteMap)
        {
            return new SiteNavigationViewModel()
            {
                LinkContainers = new List<LinkContainerModel>()
                {
                    new LinkContainerModel()
                    {
                        Title = "Главная",
                        CssModifier = "dashboard",
                        Href = siteMap.PageUrlDefault
                    },
                    new LinkContainerModel()
                    {
                        Title = "Работа с деньгами",
                        CssModifier = "money",
                        Links = new List<LinkModel>()
                        {
                            new LinkModel() { Href = siteMap.PageUrlIncomes, Text = "Доходы" },
                            new LinkModel() { Href = siteMap.PageUrlExpenditures, Text = "Расходы" }
                        }
                    }
                }
            };
        }
    }
}