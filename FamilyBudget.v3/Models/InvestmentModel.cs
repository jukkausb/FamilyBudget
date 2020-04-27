﻿using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Models.Base;
using System;
using System.Collections.Generic;

namespace FamilyBudget.v3.Models
{
    public class InvestmentModel : BaseModel
    {
        public MessageModel Message { get; set; }
        public List<InvestmentAccount> Accounts { get; set; }
        public InvestmentModel()
        {
            Accounts = new List<InvestmentAccount>();
        }
    }

    public class InvestmentAccount
    {
        public List<MessageGroup> MessageGroups { get; set; }
        public string Id { get; set; }
        public BrokerAccountType Type { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public List<TinkoffPortfolioGroup> Groups { get; set; }
        public decimal TotalInvested { get; set; }
        public string TotalInvestedPresentation
        {
            get
            {
                return TotalInvested.ToCurrencyDisplay(Currency);
            }
        }
        public decimal TotalBalance { get; set; }
        public string TotalBalancePresentation
        {
            get
            {
                return TotalBalance.ToCurrencyDisplay(Currency, true);
            }
        }
        public DeltaType TotalDeltaType { get; set; }
        public decimal TotalDelta { get; set; }
        public string TotalDeltaPresentation
        {
            get
            {
                return Math.Abs(TotalDelta).ToCurrencyDisplay(Currency, true);
            }
        }
        public string TotalDeltaPercent { get; set; }
        public InvestmentAccount()
        {
            MessageGroups = new List<MessageGroup>();
            Groups = new List<TinkoffPortfolioGroup>();
        }
    }
}