using StockPriceMonitoringAndAlerts.DTOs.Stock;

namespace StockPriceMonitoringAndAlerts.Services
{
    public interface IAlertRuleEvaluator
    {
        Task EvaluateAsync(IEnumerable<StockQuoteSnapshot> currentPrices);
    }
}
