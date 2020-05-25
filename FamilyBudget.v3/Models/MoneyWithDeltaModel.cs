using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using FamilyBudget.v3.App_Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyBudget.v3.Models
{
    public class MoneyWithDeltaModel : MoneyModel
    {
        public MoneyWithDeltaModel(decimal value, string currency, decimal delta)
        {
            Currency = currency;
            Value = value;
            Delta = new DeltaModel
            {
                Currency = currency,
                Value = delta,
            };
            DeltaPercent = Math.Round(Math.Abs(delta / value * 100), 2);
        }
        public DeltaModel Delta { get; set; }
        public decimal DeltaPercent { get; set; }
        public string DeltaPercentPresentation
        {
            get
            {
                return Math.Abs(DeltaPercent) + "%";
            }
        }
    }
}