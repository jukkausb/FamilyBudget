using FamilyBudget.v3.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyBudget.v3.Areas.Administration.Models
{
    public class InvestmentInstrumentInfoModel : BaseModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string DiagramBackgroundColor { get; set; }
        public string DiagramBackgroundHoverColor { get; set; }
        public string DiagramHoverBorderColor { get; set; }
        public Nullable<int> PortfolioPercent { get; set; }
        public Nullable<int> PortfolioPercentDelta { get; set; }
        public string ExternalAvatarIsinOverride { get; set; }
        public string ExternalPageTickerOverride { get; set; }
        public string Type { get; set; }
        public string Market { get; set; }
    }
}