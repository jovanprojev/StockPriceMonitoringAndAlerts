using StockPriceMonitoringAndAlerts.Models;

namespace StockPriceMonitoringAndAlerts.DTOs.AlertRules
{
    public class AlertRuleDto
    {
        public int Id { get; set; }
        public StockSymbol StockSymbol { get; set; }
        public double PriceThreshold { get; set; }
        public string Direction { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
