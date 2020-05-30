using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyBudget.v3.App_CodeBase
{
    public interface IPieDiagramDataSourceItem
    {
        string DiagramBackgroundColor { get; set; }
        string DiagramBackgroundHoverColor { get; set; }
        string DiagramHoverBorderColor { get; set; }
    }
}