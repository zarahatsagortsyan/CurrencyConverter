using CurrencyConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Functions
{
    public class Parser
    {
        public static bool ValidSum(string sum)
        {
            for (int i = 0; i < sum.Length; i++)
                if (!Char.IsDigit(sum[i]))
                    return false;
            return true;
        }

        public static bool ValidCurr(string curr)
        {
            for (int i = 0; i < curr.Length; i++)
                if (!Char.IsLetter(curr[i]))
                    return false;
            return true;
        }

        public static Dictionary<string, dynamic> SearchParser(string text)
        {
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();

            string[] dividedSearch = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (dividedSearch.Length == 4)
            {
                if (!ValidSum(dividedSearch[0]))
                    result.Add("sum", -1);
                else
                    result.Add("sum", Convert.ToDouble(dividedSearch[0]));

                if (!ValidCurr(dividedSearch[1]))
                    result.Add("fromCurr", "invalid");
                else
                    result.Add("fromCurr", dividedSearch[1]);

                if (!ValidCurr(dividedSearch[3]))
                    result.Add("toCurr", "invalid");
                else
                    result.Add("toCurr", dividedSearch[3]);
            }
            else
            {
                result.Add("sum", -1);
                result.Add("fromCurr", "invalid");
                result.Add("toCurr", "invalid");
            }
            return result;
        }

        public static void ConvertCurrency(List<LatestCurrency> latestCurrencies, string from_cur, string to_cur, out double sale, out double buy)
        {
            sale = -1;
            buy = -1;

            if (from_cur == "UAH" && (to_cur == "USD" || to_cur == "EUR"))
            {
                getSaleRate(latestCurrencies, to_cur, out sale, out buy);
                sale = Math.Round((double)1 / sale, 5);
                buy = Math.Round((double)1 / buy, 5);
            }
            else if ((from_cur == "USD" || from_cur == "EUR") && to_cur == "UAH")
            {
                getSaleRate(latestCurrencies, from_cur, out sale, out buy);
            }
            else if ((from_cur == "USD" && to_cur == "EUR") || (from_cur == "EUR" && to_cur == "USD"))
            {
                double rate_sale_to;
                getSaleRate(latestCurrencies, from_cur, out sale, out buy);
                getSaleRate(latestCurrencies, to_cur, out rate_sale_to, out buy);

                sale = Math.Round((1 / rate_sale_to) / (1 / sale), 5);
                buy = Math.Round((1 / rate_sale_to) / (1 / buy), 5);

            }
        }
        public static void getSaleRate(List<LatestCurrency> latestCurrencies, string ccy, out double saleRate, out double buyRate)
        {
            buyRate = 0;
            saleRate = 0;
            foreach (var item in latestCurrencies)
            {
                if (item.ccy == ccy)
                {
                    saleRate = Convert.ToDouble(item.sale);
                    buyRate = Convert.ToDouble(item.buy);

                    return;
                }
            }
        }
    }
}
