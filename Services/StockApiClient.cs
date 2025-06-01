using StockPriceMonitoringAndAlerts.DTOs.Stock;
using System.Text.Json;

namespace StockPriceMonitoringAndAlerts.Services
{
    public class StockApiClient : IStockApiClient
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public StockApiClient(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["Finnhub:ApiKey"];
        }

        public async Task<StockQuoteDTO?> GetQuoteAsync(string symbol)
        {
            var url = $"https://finnhub.io/api/v1/quote?symbol={symbol}&token={_apiKey}";
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<StockQuoteDTO>(json);
        }
    }
}
