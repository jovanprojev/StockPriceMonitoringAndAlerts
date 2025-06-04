using StockPriceMonitoringAndAlerts.DTOs.Stock;
using StockPriceMonitoringAndAlerts.Models;

namespace StockPriceMonitoringAndAlerts.Services
{
    public interface IStockSnapshotService
    {
        List<StockQuoteSnapshot> CreateSnapshots(IEnumerable<StockSymbol> symbols);
    }
}
