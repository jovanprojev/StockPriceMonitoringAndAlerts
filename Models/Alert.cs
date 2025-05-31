namespace StockPriceMonitoringAndAlerts.Models
{
    public class Alert
    {
        public int Id { get; set; } 
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        // Many-to-one relationship with AlertRule
        public int AlertRuleId { get; set; }
        public AlertRule AlertRule { get; set; }
    }
}
