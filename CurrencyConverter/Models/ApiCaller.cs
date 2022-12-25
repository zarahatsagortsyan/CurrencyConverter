using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyConverter.Models
{
    public class ApiCaller: IApiCaller
    {
        public async Task<List<LatestCurrency>> GetLatestCurrencies(string link)
        {
            List<LatestCurrency> CashRate = new List<LatestCurrency>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(link))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    CashRate = JsonConvert.DeserializeObject<List<LatestCurrency>>(apiResponse);
                }
            }
            return CashRate;
        }
    }

    public interface IApiCaller
    {
        public Task<List<LatestCurrency>> GetLatestCurrencies(string link);
    }
}
