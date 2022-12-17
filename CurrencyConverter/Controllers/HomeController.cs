using CurrencyConverter.Models;
using CurrencyConverter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CurrencyConverter.Functions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CurrencyConverter.Controllers
{
    public class HomeController : Controller
    {
        readonly ApplicationDbContext db;

        public HomeController(ApplicationDbContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<IActionResult> CurHistory()
        {
            Currency currency = new Currency();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://api.privatbank.ua/p24api/exchange_rates?date=01.12.2014"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    currency = JsonConvert.DeserializeObject<Currency>(apiResponse);
                }
            }

            ViewData["Users"] = db.Users.ToList();

            return View(currency);
        }

        #region GettingCurrenciesFromPrivatBank
        public async Task<IActionResult> Index(ConverterViewModel viewModel)
        {
            string base_ccy;
            base_ccy = "UAH";

            if (viewModel.base_ccy != null)
                base_ccy = viewModel.base_ccy;

            List<LatestCurrency> CashRate = new List<LatestCurrency>();
            List<LatestCurrency> NonCashRate = new List<LatestCurrency>();

            if (User.Identity.IsAuthenticated)
            {
                Users user = await db.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                if (user != null)
                    base_ccy = user.BaseCur;
            }

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://api.privatbank.ua/p24api/pubinfo?json&exchange&coursid=5"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    CashRate = JsonConvert.DeserializeObject<List<LatestCurrency>>(apiResponse);
                }
            }

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://api.privatbank.ua/p24api/pubinfo?exchange&json&coursid=11"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    NonCashRate = JsonConvert.DeserializeObject<List<LatestCurrency>>(apiResponse);
                }
            }

            List<LatestCurrency> curByBaseCash = new List<LatestCurrency>();
            curByBaseCash = ExchangeByBaseCCY(base_ccy, CashRate);

            List<LatestCurrency> curByBaseNonCash = new List<LatestCurrency>();
            curByBaseNonCash = ExchangeByBaseCCY(base_ccy, NonCashRate);

            if (base_ccy == "UAH")
            {
                ViewData["CashRate"] = CashRate;
                ViewData["NonCashRate"] = NonCashRate;
            }
            else
            {
                ViewData["CashRate"] = curByBaseCash;
                ViewData["NonCashRate"] = curByBaseNonCash;
            }
            ViewData["Users"] = db.Users.ToList();

            return View();
        }

        #endregion

        #region CalculatingRatesByBase
        [HttpGet]
        public static List<LatestCurrency> ExchangeByBaseCCY(string base_ccy, List<LatestCurrency> CashRate)
        {
            List<LatestCurrency> curByBase = new List<LatestCurrency>();
            double _buy, _sale;
            switch (base_ccy)
            {
                case "USD":

                    Parser.ConvertCurrency(CashRate, "UAH", "USD", out _sale, out _buy);
                    curByBase.Add(new LatestCurrency
                    {
                        ccy = "UAH",
                        buy = Convert.ToString(_buy),
                        sale = Convert.ToString(_sale)
                    });
                    Parser.ConvertCurrency(CashRate, "EUR", "USD", out _sale, out _buy);
                    curByBase.Add(new LatestCurrency
                    {
                        ccy = "EUR",
                        buy = Convert.ToString(_buy),
                        sale = Convert.ToString(_sale)
                    });
                    break;
                case "EUR":
                    Parser.ConvertCurrency(CashRate, "UAH", "EUR", out _sale, out _buy);
                    curByBase.Add(new LatestCurrency
                    {
                        ccy = "UAH",
                        buy = Convert.ToString(_buy),
                        sale = Convert.ToString(_sale)
                    });
                    Parser.ConvertCurrency(CashRate, "USD", "EUR", out _sale, out _buy);
                    curByBase.Add(new LatestCurrency
                    {
                        ccy = "USD",
                        buy = Convert.ToString(_buy),
                        sale = Convert.ToString(_sale)
                    });
                    break;
                default:
                    break;
            }

            return curByBase;
        }
        #endregion

        #region ConverterPage
        [HttpGet]
        public IActionResult ConverterPage()
        {
            ViewData["Users"] = db.Users.ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConverterPage(ConverterViewModel converter)
        {
            Dictionary<string, dynamic> curPairs = new Dictionary<string, dynamic>();
            List<LatestCurrency> latestCurrencies = new List<LatestCurrency>();
            ViewData["Users"] = db.Users.ToList();

            dynamic from_cur, to_cur, amount;
            string from_cur_str, to_cur_str;
            double sale, buy;

            curPairs = Parser.SearchParser(converter.convertText);

            if (!(ErrorRedirection(curPairs, ModelState)))
                return View();

            curPairs.TryGetValue("fromCurr", out from_cur);
            curPairs.TryGetValue("toCurr", out to_cur);
            curPairs.TryGetValue("sum", out amount);

            from_cur_str = Convert.ToString(from_cur);
            to_cur_str = Convert.ToString(to_cur);

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://api.privatbank.ua/p24api/pubinfo?json&exchange&coursid=5"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    latestCurrencies = JsonConvert.DeserializeObject<List<LatestCurrency>>(apiResponse);
                }
            }

            Parser.ConvertCurrency(latestCurrencies, from_cur_str.ToUpper(), to_cur_str.ToUpper(), out sale, out buy);
            if (sale < 0 || buy < 0)
            {
                ModelState.AddModelError("", "Unavailable currency. Use these: USD, EUR, UAH");
                return View();
            }

            ViewBag.convertText = converter.convertText;
            ViewBag.result = sale * Convert.ToDouble(amount);
            return View();
        }
        #endregion

        #region validatingSearchInput
        public static bool ErrorRedirection(Dictionary<string, dynamic> curPairs, ModelStateDictionary ModelState)
        {
            dynamic value;
            if (curPairs.TryGetValue("sum", out value))
            {
                if (value == -1)
                {
                    ModelState.AddModelError("", "invalid input!");
                    return false;
                }
            }
            if (curPairs.TryGetValue("fromCurr", out value))
            {
                if (value == "invalid")
                {
                    ModelState.AddModelError("", "invalid input!");
                    return false;
                }
            }
            if (curPairs.TryGetValue("toCurr", out value))
            {
                if (value == "invalid")
                {
                    ModelState.AddModelError("", "invalid input!");
                    return false;
                }
            }
            return true;
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
