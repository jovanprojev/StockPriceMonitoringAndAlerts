using Microsoft.EntityFrameworkCore;
using StockPriceMonitoringAndAlerts.Data;
using StockPriceMonitoringAndAlerts.DTOs.Stock;
using StockPriceMonitoringAndAlerts.Models;

namespace StockPriceMonitoringAndAlerts.Services
{
    public class AlertRuleEvaluator : IAlertRuleEvaluator
    {
        private readonly AppDbContext _db;
        private readonly ILogger<AlertRuleEvaluator> _logger;

        public AlertRuleEvaluator(AppDbContext db, ILogger<AlertRuleEvaluator> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task EvaluateAsync(IEnumerable<StockQuoteSnapshot> stockPrices)
        {
            var allAlertRules = await _db.AlertRules
                .Where(r => !r.IsActive)
                .ToListAsync();

            var rulesBySymbol = allAlertRules
                .GroupBy(r => r.StockSymbol)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var stockPrice in stockPrices)
            {
                if (Enum.TryParse<StockSymbol>(stockPrice.Symbol, out var symbolEnum) &&
                    rulesBySymbol.TryGetValue(symbolEnum, out var rules))
                {
                    foreach (var rule in rules)
                    {
                        bool conditionMet =
                            (rule.Direction == Direction.Above && stockPrice.Price > (decimal)rule.PriceThreshold) ||
                            (rule.Direction == Direction.Below && stockPrice.Price < (decimal)rule.PriceThreshold);

                        if (conditionMet)
                        {
                            rule.IsActive = true;

                            var alert = new Alert
                            {
                                AlertRuleId = rule.Id,
                                Message = $"[LOG] Alert: {stockPrice.Symbol} is {stockPrice.Price}",
                                CreatedAt = DateTime.UtcNow
                            };

                            _db.Alerts.Add(alert);
                            _logger.LogInformation(alert.Message);
                        }
                    }
                }
            }

            await _db.SaveChangesAsync();
        }
    }

}
