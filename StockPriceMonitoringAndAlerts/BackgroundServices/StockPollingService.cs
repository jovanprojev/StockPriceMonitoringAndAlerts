using StockPriceMonitoringAndAlerts.DTOs.Stock;
using StockPriceMonitoringAndAlerts.Models;
using StockPriceMonitoringAndAlerts.Services;

namespace StockPriceMonitoringAndAlerts.BackgroundServices
{
    public class StockPollingService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StockPollingService> _logger;
        private readonly IConfiguration _configuration;
        private List<StockSymbol> _symbols;

        public StockPollingService(
            IServiceScopeFactory scopeFactory,
            ILogger<StockPollingService> logger,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;

            _symbols = _configuration
                .GetSection("Finnhub:Symbols")
                .Get<List<string>>()
                .Select(s => Enum.TryParse<StockSymbol>(s, out var result) ? result : (StockSymbol?)null)
                .Where(s => s != null)
                .Select(s => s.Value)
                .ToList();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var api = scope.ServiceProvider.GetRequiredService<IStockApiClient>();
                var cache = scope.ServiceProvider.GetRequiredService<IStockPriceCacheService>();
                var evaluator = scope.ServiceProvider.GetRequiredService<IAlertRuleEvaluator>();
                var snapshotService = scope.ServiceProvider.GetRequiredService<IStockSnapshotService>();

                foreach (var symbol in _symbols)
                {
                    var quote = await api.GetQuoteAsync(symbol);
                    if (quote != null)
                    {
                        cache.Set(symbol.ToString(), quote);
                        _logger.LogInformation($"Cached quote for {symbol}: {quote.C}");

                        var snapshots = snapshotService.CreateSnapshots(_symbols);
                        await evaluator.EvaluateAsync(snapshots);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(25), stoppingToken);
            }
        }
    }
}
