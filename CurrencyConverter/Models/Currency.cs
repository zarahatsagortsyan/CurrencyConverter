using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CurrencyConverter.Models
{
    public enum CurrencyCode
    {
        [EnumMember(Value = "EUR")]
        [Description("Euro")]
        EUR = 978,

        [EnumMember(Value = "UAH")]
        [Description("Ukrainian Hryvnia")]
        UAH = 980,

        [EnumMember(Value = "USD")]
        [Description("United States dollar")]
        USD = 840,
    }
    public class Currency
    {
        public string date { get; set; }
        public string bank { get; set; }
        public string baseCurrency { get; set; }
        public string baseCurrencyLit { get; set; }
        public CurrencyRes[] exchangeRate { get; set; }

    }
}
