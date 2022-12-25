using CurrencyConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.ViewModels
{
    public class ConverterViewModel
    {
        public string convertText { get; set; }
        public double result { get; set; }
        public CurrencyCode base_ccy { get; set; }
    }
}
//;./