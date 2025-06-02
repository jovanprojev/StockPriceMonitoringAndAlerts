using StockPriceMonitoringAndAlerts.Models;

namespace StockPriceMonitoringAndAlerts.DTOs.AlertRules
{
    public class CreateAlertRuleDto
    {
        public StockSymbol StockSymbol { get; set; }
        public double PriceThreshold { get; set; }
        public string Direction { get; set; }
    }
}
