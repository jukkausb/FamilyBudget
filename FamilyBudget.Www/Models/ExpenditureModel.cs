﻿using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Models.Base;
using FamilyBudget.Www.Models.Filter;

namespace FamilyBudget.Www.Models
{
    public class ExpenditureModel : BaseListModel<Expenditure, ExpenditureFilterModel>
    {
        protected override void RestoreDefaultModelState()
        {
            SortDirection = "DESC";
            SortField = "Date";
        }
    }
}