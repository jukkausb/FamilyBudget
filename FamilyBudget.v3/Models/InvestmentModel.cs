using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
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
        public List<TinkoffPortfolioTableGroup> TableGroups { get; set; }
        public List<PortfolioDiagramGroupItem> TypeGroupItems { get; set; }
        public List<PortfolioDiagramGroupItem> MarketGroupItems { get; set; }
        public MoneyWithDeltaModel Totals { get; set; }
        public decimal TotalInvested { get; set; }
        public InvestmentAccount()
        {
            MessageGroups = new List<MessageGroup>();
            TableGroups = new List<TinkoffPortfolioTableGroup>();
            TypeGroupItems = new List<PortfolioDiagramGroupItem>();
            MarketGroupItems = new List<PortfolioDiagramGroupItem>();
        }
    }
}