﻿using System;
using FamilyBudget.Www.Models.Base;

namespace FamilyBudget.Www.Models.Report
{
    public class ReportExpendituresByCategoryModel : BaseModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}