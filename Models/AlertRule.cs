namespace StockPriceMonitoringAndAlerts.Models
{
    public class AlertRule
    {
        public int Id { get; set; } 
        public string StockSymbol { get; set; }
        public double PriceThreshold { get; set; }
        public Direction Direction { get; set; }

        // one-to-many relationship with Alert
        public List<Alert> Alerts { get; set; }
    }
}
