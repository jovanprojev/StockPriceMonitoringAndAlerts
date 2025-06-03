using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StockPriceMonitoringAndAlerts.Data;
using StockPriceMonitoringAndAlerts.DTOs.Stock;
using StockPriceMonitoringAndAlerts.Hubs;
using StockPriceMonitoringAndAlerts.Models;
using StockPriceMonitoringAndAlerts.Services;

namespace StockPriceMonitoring.Tests.Services
{
    public class AlertRuleEvaluatorTests
    {
        [Fact]
        public async Task EvaluateAlertRulesAsync_ShouldTriggerNotification_WhenRuleIsMet()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<AlertRuleEvaluator>>();
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            // Setup hub context to broadcast message
            mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);
            mockClients.Setup(x => x.All).Returns(mockClientProxy.Object);
            mockClientProxy
                .Setup(x => x.SendCoreAsync("ReceiveNotification", It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);

            // Create in-memory DbContext with rule(s)
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            await using var dbContext = new AppDbContext(options);

            dbContext.AlertRules.Add(new AlertRule
            {
                StockSymbol = StockSymbol.AAPL,
                PriceThreshold = 150,
                Direction = Direction.Above,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();

            var evaluator = new AlertRuleEvaluator(dbContext, mockLogger.Object, mockHubContext.Object);

            var prices = new List<StockQuoteSnapshot>
            {
                new StockQuoteSnapshot { Symbol = "AAPL", Price = 155 }
            };

            // Act
            await evaluator.EvaluateAsync(prices);

            // Assert
            mockClientProxy.Verify(
                x => x.SendCoreAsync("ReceiveNotification", It.IsAny<object[]>(), default),
                Times.Once);
        }

        [Fact]
        public async Task EvaluateAlertRulesAsync_ShouldNotTriggerNotification_WhenRuleIsNotMet()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<AlertRuleEvaluator>>();
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);
            mockClients.Setup(x => x.All).Returns(mockClientProxy.Object);

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            await using var dbContext = new AppDbContext(options);

            dbContext.AlertRules.Add(new AlertRule
            {
                StockSymbol = StockSymbol.AAPL,
                PriceThreshold = 150,
                Direction = Direction.Above,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();

            var evaluator = new AlertRuleEvaluator(dbContext, mockLogger.Object, mockHubContext.Object);

            var prices = new List<StockQuoteSnapshot>
            {
                new StockQuoteSnapshot { Symbol = "AAPL", Price = 140 } // Price below threshold
            };

            // Act
            await evaluator.EvaluateAsync(prices);

            // Assert
            mockClientProxy.Verify(
                x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default),
                Times.Never);
        }

        [Fact]
        public async Task EvaluateAlertRulesAsync_ShouldMarkRuleAsActive_WhenTriggered()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<AlertRuleEvaluator>>();
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);
            mockClients.Setup(x => x.All).Returns(mockClientProxy.Object);
            mockClientProxy
                .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            await using var dbContext = new AppDbContext(options);

            var rule = new AlertRule
            {
                StockSymbol = StockSymbol.AAPL,
                PriceThreshold = 150,
                Direction = Direction.Above,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.AlertRules.Add(rule);
            await dbContext.SaveChangesAsync();

            var evaluator = new AlertRuleEvaluator(dbContext, mockLogger.Object, mockHubContext.Object);

            var prices = new List<StockQuoteSnapshot>
            {
                new StockQuoteSnapshot { Symbol = "AAPL", Price = 155 }
            };

            // Act
            await evaluator.EvaluateAsync(prices);

            // Assert
            var updatedRule = await dbContext.AlertRules.FirstAsync();
            Assert.True(updatedRule.IsActive);
        }

        [Fact]
        public async Task EvaluateAlertRulesAsync_ShouldSaveAlertToDatabase_WhenRuleIsMet()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<AlertRuleEvaluator>>();
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);
            mockClients.Setup(x => x.All).Returns(mockClientProxy.Object);
            mockClientProxy
                .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            await using var dbContext = new AppDbContext(options);

            dbContext.AlertRules.Add(new AlertRule
            {
                StockSymbol = StockSymbol.AAPL,
                PriceThreshold = 150,
                Direction = Direction.Above,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();

            var evaluator = new AlertRuleEvaluator(dbContext, mockLogger.Object, mockHubContext.Object);

            var prices = new List<StockQuoteSnapshot>
            {
                new StockQuoteSnapshot { Symbol = "AAPL", Price = 155 }
            };

            // Act
            await evaluator.EvaluateAsync(prices);

            // Assert
            var alert = await dbContext.Alerts.FirstOrDefaultAsync();
            Assert.NotNull(alert);
            Assert.Contains("AAPL goes above", alert.Message);
        }
    }
}
