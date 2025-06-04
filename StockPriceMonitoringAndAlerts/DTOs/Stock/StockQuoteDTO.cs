using System.Text.Json.Serialization;

namespace StockPriceMonitoringAndAlerts.DTOs.Stock
{
    public class StockQuoteDTO
    {
        [JsonPropertyName("c")]
        public decimal C { get; set; }  // Current price
        [JsonPropertyName("d")]
        public decimal D { get; set; }  // Change
        [JsonPropertyName("dp")]
        public decimal Dp { get; set; } // Percent change
        [JsonPropertyName("h")]
        public decimal H { get; set; }  // High
        [JsonPropertyName("l")]
        public decimal L { get; set; }  // Low
        [JsonPropertyName("o")]
        public decimal O { get; set; }  // Open
        [JsonPropertyName("pc")]
        public decimal Pc { get; set; } // Previous close
        [JsonPropertyName("t")]
        public long T { get; set; }     // Timestamp
    }
}
