using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Models
{
    public class Currency
    {
        public string date { get; set; }
        public string bank { get; set; }
        public string baseCurrency { get; set; }
        public string baseCurrencyLit { get; set; }
        public CurrencyRes[] exchangeRate { get; set; }

    }
}
