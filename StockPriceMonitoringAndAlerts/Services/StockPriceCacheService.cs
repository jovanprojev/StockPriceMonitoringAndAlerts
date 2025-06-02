using Microsoft.Extensions.Caching.Memory;
using StockPriceMonitoringAndAlerts.DTOs.Stock;

namespace StockPriceMonitoringAndAlerts.Services
{
    public class StockPriceCacheService : IStockPriceCacheService
    {
        private readonly IMemoryCache _cache;

        public StockPriceCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Set(string symbol, StockQuoteDTO quote)
        {
            _cache.Set(symbol, quote, TimeSpan.FromMinutes(1));
        }

        public StockQuoteDTO? Get(string symbol)
        {
            _cache.TryGetValue(symbol, out StockQuoteDTO? quote);
            return quote;
        }
    }
}
