using StockPriceMonitoringAndAlerts.DTOs.Stock;
using StockPriceMonitoringAndAlerts.Models;
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

        public async Task<StockQuoteDTO> GetQuoteAsync(StockSymbol symbol)
        {
            var url = $"https://finnhub.io/api/v1/quote?symbol={symbol}&token={_apiKey}";
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Failed to fetch quote for {symbol}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<StockQuoteDTO>(json) ?? throw new JsonException("Failed to deserialize stock quote");
        }
    }
}
