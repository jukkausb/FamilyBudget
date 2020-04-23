using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff.Models
{
    public class TinkoffPortfolioGroup
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public List<Message> Messages { get; set; }
        public List<TinkoffPortfolioPosition> Positions { get; set; }
        public decimal CurrentTotalInPortfolio { get; set; }
        public string PositionsAsJson => JsonSerializeHelper.Serialize(Positions);
        public TinkoffPortfolioGroup()
        {
            Messages = new List<Message>();
            Positions = new List<TinkoffPortfolioPosition>();
        }
    }
}