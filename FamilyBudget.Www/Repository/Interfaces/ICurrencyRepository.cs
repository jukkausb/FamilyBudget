﻿using FamilyBudget.Www.App_DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FamilyBudget.Www.Repository.Interfaces
{
    public interface ICurrencyRepository : IGenericRepository<FamilyBudgetEntities, Currency>
    {
    }
}