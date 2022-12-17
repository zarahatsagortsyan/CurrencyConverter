using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Models
{
    public class LatestCurrency
    {
        public string ccy { get; set; }
        public string base_ccy { get; set; }
        public string buy { get; set; }
        public string sale { get; set; }
    //      "ccy":"USD",
    //"base_ccy":"UAH",
    //"buy":"15.50000",
    //"sale":"15.85000"
    }
}
