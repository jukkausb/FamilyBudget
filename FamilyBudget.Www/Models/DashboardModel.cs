using System.Collections.Generic;
using FamilyBudget.Www.App_CodeBase.Widgets;
using FamilyBudget.Www.Models.Base;

namespace FamilyBudget.Www.Models
{
    public class DashboardModel : BaseModel
    {
        public List<Widget> Widgets { get; set; }
    }
}