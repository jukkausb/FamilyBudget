using FamilyBudget.v3.App_CodeBase;
using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using FamilyBudget.v3.App_DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyBudget.v3.App_Helpers
{
    public static class DiagramHelper
    {
        public static void ApplyPieDiagramColors(IPieDiagramDataItem pieDiagramDataItem, InvestmentInstrument investmentInstrument)
        {
            if (pieDiagramDataItem == null)
            {
                return;
            }

            if (investmentInstrument != null)
            {
                pieDiagramDataItem.DiagramBackgroundColor = investmentInstrument.DiagramBackgroundColor;
                pieDiagramDataItem.DiagramBackgroundHoverColor = investmentInstrument.DiagramBackgroundHoverColor;
                pieDiagramDataItem.DiagramHoverBorderColor = investmentInstrument.DiagramHoverBorderColor;
            }
            else
            {
                pieDiagramDataItem.DiagramBackgroundColor = Constants.InstrumentDiagram.DEFAULT_COLOR;
                pieDiagramDataItem.DiagramBackgroundHoverColor = Constants.InstrumentDiagram.DEFAULT_COLOR;
                pieDiagramDataItem.DiagramHoverBorderColor = Constants.InstrumentDiagram.DEFAULT_COLOR;
            }
        }
    }
}