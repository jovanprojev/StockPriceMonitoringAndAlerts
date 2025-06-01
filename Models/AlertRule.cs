namespace StockPriceMonitoringAndAlerts.Models
{
    public class AlertRule
    {
        public int Id { get; set; } 
        public StockSymbol StockSymbol { get; set; }
        public double PriceThreshold { get; set; }
        public Direction Direction { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Boolean IsActive { get; set; } = true;

        // one-to-many relationship with Alert
        public List<Alert> Alerts { get; set; }
    }
}
