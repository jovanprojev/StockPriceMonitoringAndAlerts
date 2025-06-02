using StockPriceMonitoringAndAlerts.DTOs.Stock;

namespace StockPriceMonitoringAndAlerts.Services
{
    public interface IStockPriceCacheService
    {
        void Set(string symbol, StockQuoteDTO quote);
        StockQuoteDTO? Get(string symbol);
    }
}
