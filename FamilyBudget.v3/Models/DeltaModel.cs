using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using FamilyBudget.v3.App_Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyBudget.v3.Models
{
    public class DeltaModel : MoneyModel
    {
        public DeltaType Type => BusinessHelper.GetDeltaType(Value);
    }
}