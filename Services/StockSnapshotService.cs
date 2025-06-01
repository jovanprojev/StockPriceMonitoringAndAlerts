using StockPriceMonitoringAndAlerts.DTOs.Stock;
using StockPriceMonitoringAndAlerts.Models;

namespace StockPriceMonitoringAndAlerts.Services
{
    public class StockSnapshotService : IStockSnapshotService
    {
        private readonly IStockPriceCacheService _cache;

        public StockSnapshotService(IStockPriceCacheService cache)
        {
            _cache = cache;
        }

        public List<StockQuoteSnapshot> CreateSnapshots(IEnumerable<StockSymbol> symbols)
        {
            return symbols
                .Select(symbol => new { symbol, quote = _cache.Get(symbol.ToString()) })
                .Where(x => x.quote != null)
                .Select(x => new StockQuoteSnapshot
                {
                    Symbol = x.symbol.ToString(),
                    Price = x.quote!.C
                })
                .ToList();
        }
    }
}
