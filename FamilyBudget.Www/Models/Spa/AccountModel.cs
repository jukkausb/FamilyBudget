﻿
namespace FamilyBudget.Www.Models.Spa
{
    public class AccountModel
    {
        public string DisplayName { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal Balance { get; set; }

        // All incomes minus all expenditures
        public decimal BalanceReal { get; set; }
    }
}