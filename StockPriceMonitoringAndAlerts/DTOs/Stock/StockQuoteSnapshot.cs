namespace StockPriceMonitoringAndAlerts.DTOs.Stock
{
    public class StockQuoteSnapshot
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
