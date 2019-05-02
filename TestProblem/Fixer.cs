using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestProblem
{
    public class Fixer
    {
        private string ApiKey { get; set; } = "e1a47785c4a9384a535e83449319b045";
        private string BaseUri { get; set; } = "http://data.fixer.io/api/";
        private string Endpoint { get; set; } = "latest";

        public string FormString(double amount, string currencyFrom, string currencyTo)
        {
            if (string.Equals(currencyFrom, "USD"))
                currencyFrom = "EUR"; // USD is not supported by free version of Fixer.io
            return $"{BaseUri}{Endpoint}?access_key={ApiKey}&base={currencyFrom}&symbols={currencyTo}";
        }

        public double GetPrice(string url, double amount, string currencyTo)
        {
            var client = new WebClient();

            var response = client.DownloadString(url);
            JObject currencies = JObject.Parse(response);
            var currency = currencies.SelectToken("rates").SelectToken(currencyTo);
            return amount * currency.ToObject<Double>(); // currencyTo - in what currency the result should be represented
        }
    }
}
