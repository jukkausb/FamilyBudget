using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff.Models
{
    public class TinkoffBrokerAccounts
    {
        public List<TinkoffBrokerAccount> Accounts { get; set; }
    }
    public class TinkoffBrokerAccount
    {
        public string BrokerAccountId { get; set; }
        public BrokerAccountType BrokerAccountType { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum BrokerAccountType
    {
        Tinkoff = 1,
        TinkoffIis = 2
    }
}