**📈 Stock Price Alert API (Demo)**

This is a demo .NET Core Web API project built as part of a job interview process.\
The application fetches real stock prices using the [Finnhub.io](https://finnhub.io/) API and allows users to register alert rules based on price thresholds.

-----
**✅ Features**

- Fetches real-time stock prices from Finnhub for predefined symbols (AAPL, GOOGL, MSFT, AMZN, TSLA).
- Periodic price updates every 25 seconds via background service.
- Alerts are registered for price thresholds (above/below a value).
- Alerts are grouped and optimized for performance.
- Sends real-time notifications to the frontend using SignalR (WebSocket).
- Persists triggered alerts in the database.
-----
**📊 Technologies**

- .NET 8 Web API
- Entity Framework Core 
- SignalR for WebSocket communication
- Hosted Services for background stock polling
- [Finnhub.io API](https://finnhub.io/) for live stock data
-----
**🔐 Finnhub Setup**

1. Register for a free API key at [https://finnhub.io](https://finnhub.io/).
1. Add your API key to your configuration:

```json
{
  "Finnhub": {
    "ApiKey": "your_finnhub_api_key",
    "Symbols": [ "AAPL", "GOOGL", "MSFT", "AMZN", "TSLA" ]
  }
}
```

Predefined stock symbols are also configured in appsettings.json, making the system easily configurable.-----

**⚡ Optimization Strategies**

**✅ In-Memory Price Caching**

Stock prices fetched from Finnhub during each polling cycle are cached in memory and reused when checking alert conditions. This avoids repeated API calls and improves performance.

**✅ Avoid Rechecking Triggered Alerts**

Each alert rule has an IsActive flag. Once a rule is satisfied, it is marked as inactive and stored in the database. This prevents duplicate processing and ensures notifications are sent only once per condition.

**✅ Grouping and Batching with EF Core**

Alert rules are retrieved from the database in a single query and grouped by stock symbol to minimize data access overhead:

```
var allAlertRules = await _db.AlertRules
.Where(r => r.IsActive)
.ToListAsync();

var rulesBySymbol = allAlertRules
.GroupBy(r => r.StockSymbol)
.ToDictionary(g => g.Key, g => g.ToList());
```

**↺ Future Optimization Ideas**

|**Optimization**|**Description**|
| :-: | :-: |
|**Per-Symbol Processing**|Execute alert evaluations per symbol in isolated tasks to enable better parallelism and load distribution. To further optimize performance, the alert evaluation logic can be split across multiple functions, each responsible for a single stock symbol (e.g., AAPL, GOOGL, etc.). This separation would allow better concurrency and scalability when checking conditions.|
|**Trading Hours Pause**|Stop polling stock prices outside of active market hours (e.g., weekends, or before 15:30 and after 22:00 GMT+2).|
|**Database Indexing**|Index frequently queried fields like StockSymbol, IsActive, PriceThreshold.|
|**Distributed Queues**|Use background messaging (e.g., RabbitMQ) for scalable alert processing.|
|**SignalR Backplane**|Add support for SignalR scaling with Redis if used in distributed hosting.|

-----
**🔓 User Scope**

This is a **demo application**, so authentication is not implemented.\
All alerts and notifications are treated as global for a single demo user. This decision was made to avoid adding unnecessary frontend complexity.\
Multi-user support could be introduced later with authentication and user-scoped SignalR communication.

-----
**🚀 Setup & Run**

**Requirements:**

- .NET 8 SDK
- I understand that sharing credentials in the repository is not a good practice due to the sensitivity of the data, but I did it temporarily to simplify testing.

**Steps via Visual Studio:**

dotnet restore

dotnet run

**Steps via Docker Container**

git clone https://github.com/jovanprojev/StockPriceMonitoringAndAlerts

cd StockPriceMonitoringAndAlerts

docker-compose up –build

After build hit on http://localhost:5230 via browser

-----
**🌊 Stock Price Updates**

- Prices are fetched every 25 seconds.
- Real-time data is retrieved from [Finnhub.io](https://finnhub.io/).
- If market is closed (weekends, or outside trading hours), polling can be paused - future optimization idea.
-----
**🌐 API Endpoints**

|**Endpoint**|**Method**|**Description**|
| :-: | :-: | :-: |
|/api/AlertRules|POST|Register an alert rule|
|/api/AlertRules/active|GET|Get all active alerts|
|/api/AlertRules/{id}|DELETE|Delete an alert by ID|

**Sample Alert Rule Payload**

```json
{
  "stockSymbol": "AAPL",
  "priceThreshold": 200.0,
  "direction": "Above"
}
```

-----
**📢 Real-Time Notifications**

When an alert condition is met, a WebSocket notification is broadcast via SignalR:

```json
{
  "stock": "AAPL",
  "price": 202.35,
  "message": "AAPL goes above 200!"
}
```

-----
**💻 Sample Frontend**

A minimal frontend was added to help test and demonstrate backend functionality:

- Connects to SignalR WebSocket for real-time alerts
- Form to create alert rules by selecting stock, price, and direction
- Displays triggered alerts in real-time

To run it:

- Run project and hit http://localhost: 5230

This allows easy manual testing of alert behavior without needing Postman or a full UI framework.

-----

