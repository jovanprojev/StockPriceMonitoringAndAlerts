using StockPriceMonitoringAndAlerts.Services;

namespace StockPriceMonitoringAndAlerts.BackgroundServices
{
    public class StockPollingService : BackgroundService
    {
        private readonly IStockApiClient _api;
        private readonly IStockPriceCacheService _cache;
        private readonly ILogger<StockPollingService> _logger;
        private readonly IConfiguration _configuration;
        private List<string> _symbols;

        public StockPollingService(IStockApiClient api, IStockPriceCacheService cache, ILogger<StockPollingService> logger, IConfiguration configuration)
        {
            _api = api;
            _cache = cache;
            _logger = logger;
            _configuration = configuration;

            _symbols = configuration.GetSection("Finnhub:Symbols").Get<List<string>>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var symbol in _symbols)
                {
                    var quote = await _api.GetQuoteAsync(symbol);
                    if (quote != null)
                    {
                        _cache.Set(symbol, quote);
                        _logger.LogInformation($"Cached quote for {symbol}: {quote.C}");
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(25), stoppingToken);
            }
        }
    }
}
