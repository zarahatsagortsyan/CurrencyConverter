namespace CurrencyConverter.Models
{
    public class LatestCurrency
    {
        public CurrencyCode ccy { get; set; }
        public CurrencyCode base_ccy { get; set; }
        public string buy { get; set; }
        public string sale { get; set; }
    }
}
