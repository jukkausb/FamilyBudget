﻿using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff.Models
{
    public class TinkoffPortfolioGroup : IPieDiagramDataItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<TinkoffPortfolioPosition> Positions { get; set; }
        public decimal CurrentTotalInPortfolio { get; set; }
        public string DiagramBackgroundColor { get; set; }
        public string DiagramBackgroundHoverColor { get; set; }
        public string DiagramHoverBorderColor { get; set; }
        public TinkoffPortfolioGroup()
        {
            Positions = new List<TinkoffPortfolioPosition>();
        }
    }
}