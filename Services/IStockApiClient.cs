using StockPriceMonitoringAndAlerts.DTOs.Stock;

namespace StockPriceMonitoringAndAlerts.Services
{
    public interface IStockApiClient
    {
        Task<StockQuoteDTO?> GetQuoteAsync(string symbol);
    }
}
