using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Models
{
    public class CurrencyRes
    {
        public string currency { get; set; }
        public string baseCurrency { get; set; }
        public string saleRateNB  { get; set; }
        public string purchaseRateNB { get; set; }
        public string saleRate { get; set; }
        public string purchaseRate { get; set; }

        //"ccy": "EUR",
        //"base_ccy": "UAH",
        //"buy": "40.40000",
        //"sale": "41.40000"
    }
}
