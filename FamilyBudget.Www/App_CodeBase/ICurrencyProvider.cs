﻿

namespace FamilyBudget.Www.App_CodeBase
{
    public interface ICurrencyProvider
    {
        void DownloadCurrencyRates(string sellCurrencyCode, string purchaseCurrencyCode);
        decimal GetSellCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode);
    }
}