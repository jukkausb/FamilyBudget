using FamilyBudget.v3.Models;
using System.Collections.Generic;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff.Models
{
    public class TinkoffPortfolioGroup
    {
        public List<Message> Messages { get; set; }
        public List<TinkoffPortfolioPosition> Positions { get; set; }
        public TinkoffPortfolioGroup()
        {
            Messages = new List<Message>();
        }
    }
}