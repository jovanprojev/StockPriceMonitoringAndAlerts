using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StockPriceMonitoringAndAlerts.DTOs.Stock;

namespace StockPriceMonitoringAndAlerts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public StocksController(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        [HttpGet("latest")]
        public IActionResult GetLatestQuotes()
        {
            var symbols = _configuration.GetSection("Finnhub:Symbols").Get<List<string>>();
            var quotes = new Dictionary<string, StockQuoteDTO>();

            foreach (var symbol in symbols)
            {
                if (_cache.TryGetValue<StockQuoteDTO>(symbol, out var quote))
                {
                    quotes[symbol] = quote;
                }
            }

            return Ok(quotes);
        }
    }
}
