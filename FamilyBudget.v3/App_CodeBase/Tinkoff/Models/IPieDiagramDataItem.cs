using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff.Models
{
    public interface IPieDiagramDataItem
    {
        string Name { get; set; }
        decimal CurrentTotalInPortfolio { get; set; }
        string DiagramBackgroundColor { get; set; }
        string DiagramBackgroundHoverColor { get; set; }
        string DiagramHoverBorderColor { get; set; }
    }
}
