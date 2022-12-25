using CurrencyConverter.Models;
using CurrencyConverter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyConverter.Functions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace CurrencyConverter.Controllers
{
    public class HomeController : Controller
    {
        IApplicationDbContext db;
        IApiCaller apiCaller;
        private IConfigurationRoot configRoot;

        public HomeController(IApplicationDbContext context,
                                IConfiguration _configRoot,
                                IApiCaller _apiCaller)
        {
            db = context;
            configRoot = (IConfigurationRoot)_configRoot;
            apiCaller = _apiCaller;
        }

        #region GettingCurrenciesFromPrivatBank
        public async Task<IActionResult> Index(ConverterViewModel viewModel)
        {
            CurrencyCode base_ccy = CurrencyCode.UAH;

            if (viewModel.base_ccy != 0)
                base_ccy = viewModel.base_ccy;

            if (User.Identity.IsAuthenticated)
            {
                Users user = await db.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                if (user != null)
                    Enum.TryParse<CurrencyCode>(user.BaseCur, out base_ccy);
            }

            List<LatestCurrency> cashRate = await apiCaller.GetLatestCurrencies(configRoot["Privat24:CashRate"]);
            List<LatestCurrency> nonCashRate = await apiCaller.GetLatestCurrencies(configRoot["Privat24:NonCashRate"]);

            List<LatestCurrency> curByBaseCash = Parser.ExchangeByBaseCCY(base_ccy, cashRate);
            List<LatestCurrency> curByBaseNonCash = Parser.ExchangeByBaseCCY(base_ccy, nonCashRate);

            if (base_ccy == CurrencyCode.UAH)
            {
                ViewData["CashRate"] = cashRate;
                ViewData["NonCashRate"] = nonCashRate;
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
            Dictionary<string, string> curPairs = new Dictionary<string, string>();
            CurrencyCode from_cur_code, to_cur_code;
            string from_cur, to_cur, amount;
            double convertedAmount;

            ViewData["Users"] = db.Users.ToList();

            curPairs = Parser.SearchParser(converter.convertText);
            if (!(ErrorRedirection(curPairs, ModelState)))
                return View();

            curPairs.TryGetValue("fromCurr", out from_cur);
            curPairs.TryGetValue("toCurr", out to_cur);
            curPairs.TryGetValue("sum", out amount);

            Enum.TryParse<CurrencyCode>(from_cur.ToUpper(), out from_cur_code);
            Enum.TryParse<CurrencyCode>(to_cur.ToUpper(), out to_cur_code);

            List<LatestCurrency> latestCurrencies = await apiCaller.GetLatestCurrencies(configRoot["Privat24:CashRate"]);

            var buySaleRate = Parser.ConvertCurrency(latestCurrencies, from_cur_code, to_cur_code);
            if ((convertedAmount = Parser.GetConvertedAmount(buySaleRate.sale, Convert.ToDouble(amount), ModelState)) == -1)
                return View();

            ViewBag.convertText = converter.convertText;
            ViewBag.result = convertedAmount;
            return View();
        }
        #endregion

        #region validatingSearchInput
        public static bool ErrorRedirection(Dictionary<string, string> curPairs, ModelStateDictionary ModelState)
        {
            string value;
            if (curPairs.TryGetValue("sum", out value))
            {
                if (value == "-1")
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
