using StockPriceMonitoringAndAlerts.DTOs.Stock;
using StockPriceMonitoringAndAlerts.Models;

namespace StockPriceMonitoringAndAlerts.Services
{
    public interface IStockApiClient
    {
        Task<StockQuoteDTO> GetQuoteAsync(StockSymbol symbol);
    }
}
