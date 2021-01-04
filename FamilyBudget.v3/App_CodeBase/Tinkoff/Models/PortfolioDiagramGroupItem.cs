using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff.Models
{
    public class PortfolioDiagramGroupItem : IPieDiagramDataItem
    {
        public string Name { get; set; }
        public double CurrentTotalInPortfolio { get; set; }
        public double CurrentPercentInPortfolio { get; set; }
        public string DiagramBackgroundColor { get; set; }
        public string DiagramBackgroundHoverColor { get; set; }
        public string DiagramHoverBorderColor { get; set; }
    }
}