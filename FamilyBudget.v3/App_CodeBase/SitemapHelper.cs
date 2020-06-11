using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyBudget.v3.App_CodeBase
{
    public static class SitemapHelper
    {
        public static Sitemap GetSitemap()
        {
            List<SitemapEntry> menuList = new List<SitemapEntry>
            {
                new SitemapEntry { Id = "pageHome", Url = "/Home", Title = "Главная", IconCssClass = "fa-tachometer-alt" },
                new SitemapEntry { Id = "pageCharts", Url = "/Charts", Title = "Графики", IconCssClass = "fa-chart-area" },
                new SitemapEntry { IsDivider = true },
                new SitemapEntry { Id = "pageIncome", Url = "/Income?sort=Date&sortdir=DESC", Title = "Доходы", IconCssClass = "fa-ruble-sign" },
                new SitemapEntry { Id = "pageExpenditure", Url = "/Expenditure?sort=Date&sortdir=DESC", Title = "Расходы", IconCssClass = "fa-ruble-sign" },
                new SitemapEntry { IsDivider = true },
                new SitemapEntry { Id = "pageInvestment", Url = "/Investment", Title = "Инвестиции", IconCssClass = "fa-dollar-sign" },
                new SitemapEntry { Id = "pageInvestmentConfig", Url = "/Administration/InvestmentConfig", Title = "Конфегурация портфеля", IconCssClass = "fa-cog" },
                new SitemapEntry { IsDivider = true },
                new SitemapEntry { Id = "pageSettings", Title = "Настройка", IconCssClass = "fa-wrench",
                    Children = new List<SitemapEntry>
                    {
                        new SitemapEntry { Id = "pageSettingsAccount", Url = "/Administration/Account", Title = "Счета" },
                        new SitemapEntry { Id = "pageSettingsCurrency", Url = "/Administration/Currency", Title = "Валюты" },
                        new SitemapEntry { Id = "pageSettingsExpenditureCategory", Url = "/Administration/ExpenditureCategory", Title = "Категории затрат" },
                        new SitemapEntry { Id = "pageSettingsIncomeCategory", Url = "/Administration/IncomeCategory", Title = "Категории доходов" },
                        new SitemapEntry { Id = "pageSettingsInvestmentInstrumentMarket", Url = "/Administration/InvestmentInstrumentMarket", Title = "Рынки инструментов" },
                        new SitemapEntry { Id = "pageSettingsInvestmentInstrumentType", Url = "/Administration/InvestmentInstrumentType", Title = "Типы инструментов" }
                    }
                },

            };

            return new Sitemap
            {
                MenuList = menuList
            };
        }
    }
}