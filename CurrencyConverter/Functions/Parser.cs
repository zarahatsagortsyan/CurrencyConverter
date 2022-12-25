using CurrencyConverter.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Functions
{
    public static class Parser
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
        public static Dictionary<string, string> SearchParser(string text)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            string[] dividedSearch = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (dividedSearch.Length == 4)
            {
                if (!ValidSum(dividedSearch[0]))
                    result.Add("sum", "-1");
                else
                    result.Add("sum", dividedSearch[0]);

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
                result.Add("sum", "-1");
                result.Add("fromCurr", "invalid");
                result.Add("toCurr", "invalid");
            }
            return result;
        }

        public static (double sale, double buy) ConvertCurrency(List<LatestCurrency> latestCurrencies, CurrencyCode from_cur, CurrencyCode to_cur)
        {
            double sale = -1;
            double buy = -1;

            if (from_cur == CurrencyCode.UAH && (to_cur == CurrencyCode.USD || to_cur == CurrencyCode.EUR))
            {
                //GetSaleRate(latestCurrencies, to_cur, out sale, out buy);
                var fromUAH = GetSaleRate(latestCurrencies, to_cur);
                sale = Math.Round((double)1 / fromUAH.sale, 5);
                buy = Math.Round((double)1 / fromUAH.buy, 5);
            }
            else if ((from_cur == CurrencyCode.USD || from_cur == CurrencyCode.EUR) && to_cur == CurrencyCode.UAH)
            {
                //GetSaleRate(latestCurrencies, from_cur, out sale, out buy);
                var toUAH = GetSaleRate(latestCurrencies, from_cur);
                sale = toUAH.sale;
                buy = toUAH.buy;
            }
            else if ((from_cur == CurrencyCode.USD && to_cur == CurrencyCode.EUR) || (from_cur == CurrencyCode.EUR && to_cur == CurrencyCode.USD))
            {
                var get_from_cur = GetSaleRate(latestCurrencies, from_cur);
                var get_to_cur = GetSaleRate(latestCurrencies, to_cur);
                double rate_sale_to = get_to_cur.sale;

                sale = Math.Round((1 / rate_sale_to) / (1 / get_from_cur.sale), 5);
                buy = Math.Round((1 / rate_sale_to) / (1 / get_from_cur.buy), 5);
            }
            return (sale, buy);
        }

        public static (double sale, double buy) GetSaleRate(List<LatestCurrency> latestCurrencies, CurrencyCode ccy)
        {
            double buyRate = 0;
            double saleRate = 0;
            foreach (var item in latestCurrencies)
            {
                if (item.ccy == ccy)
                {
                    saleRate = Convert.ToDouble(item.sale);
                    buyRate = Convert.ToDouble(item.buy);

                    return (saleRate, buyRate);
                }
            }
            return (saleRate, buyRate);
        }
        public static List<LatestCurrency> ExchangeByBaseCCY(CurrencyCode base_ccy, List<LatestCurrency> CashRate)
        {
            List<LatestCurrency> curByBase = new List<LatestCurrency>();

            switch (base_ccy)
            {
                case CurrencyCode.USD:

                    var curTuple = Parser.ConvertCurrency(CashRate, CurrencyCode.UAH, CurrencyCode.USD);
                    curByBase.Add(new LatestCurrency
                    {
                        ccy = CurrencyCode.UAH,
                        buy = Convert.ToString(curTuple.buy),
                        sale = Convert.ToString(curTuple.sale)
                    });

                    curTuple = Parser.ConvertCurrency(CashRate, CurrencyCode.EUR, CurrencyCode.USD);
                    curByBase.Add(new LatestCurrency
                    {
                        ccy = CurrencyCode.EUR,
                        buy = Convert.ToString(curTuple.buy),
                        sale = Convert.ToString(curTuple.sale)
                    });
                    break;
                case CurrencyCode.EUR:
                    curTuple = Parser.ConvertCurrency(CashRate, CurrencyCode.UAH, CurrencyCode.EUR);
                    curByBase.Add(new LatestCurrency
                    {
                        ccy = CurrencyCode.UAH,
                        buy = Convert.ToString(curTuple.buy),
                        sale = Convert.ToString(curTuple.sale)
                    });

                    curTuple = Parser.ConvertCurrency(CashRate, CurrencyCode.USD, CurrencyCode.EUR);
                    curByBase.Add(new LatestCurrency
                    {
                        ccy = CurrencyCode.USD,
                        buy = Convert.ToString(curTuple.buy),
                        sale = Convert.ToString(curTuple.sale)
                    });
                    break;
                default:
                    break;
            }

            return curByBase;
        }
        public static double GetConvertedAmount(double saleRate, double amount, ModelStateDictionary ModelState)
        {
            if (saleRate < 0 || saleRate < 0)
            {
                ModelState.AddModelError("", "Unavailable currency. Use these: USD, EUR, UAH");
                return (-1);
            }
            return Math.Round(saleRate * Convert.ToDouble(amount), 5);
        }
    }
}
