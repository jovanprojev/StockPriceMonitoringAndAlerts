using Moq;
using StockPriceMonitoringAndAlerts.DTOs.AlertRules;
using StockPriceMonitoringAndAlerts.Models;
using StockPriceMonitoringAndAlerts.Repositories;
using StockPriceMonitoringAndAlerts.Services;

namespace StockPriceMonitoring.Tests.Services
{
    public class AlertRuleServiceTests
    {
        [Fact]
        public async Task CreateAlertRule_ShouldReturnExpectedDto()
        {
            // Arrange
            var mockRepo = new Mock<IAlertRuleRepository>();
            var service = new AlertRuleService(mockRepo.Object);

            var dto = new CreateAlertRuleDto
            {
                StockSymbol = StockSymbol.AAPL,
                PriceThreshold = 150,
                Direction = "Above"
            };

            mockRepo.Setup(r => r.AddAsync(It.IsAny<AlertRule>()))
                    .Returns(Task.CompletedTask);

            mockRepo.Setup(r => r.SaveChangesAsync())
                    .Returns(Task.CompletedTask);

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.Equal(dto.PriceThreshold, result.PriceThreshold);
            Assert.Equal(dto.StockSymbol, result.StockSymbol);
            Assert.Equal(dto.Direction, result.Direction);
        }

        [Fact]
        public async Task CreateAlertRule_ShouldThrow_WhenDirectionIsInvalid()
        {
            // Arrange
            var mockRepo = new Mock<IAlertRuleRepository>();
            var service = new AlertRuleService(mockRepo.Object);

            var dto = new CreateAlertRuleDto
            {
                StockSymbol = StockSymbol.AAPL,
                PriceThreshold = 150,
                Direction = "INVALID"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAlertRule_ShouldThrow_WhenPriceThresholdIsInvalid()
        {
            // Arrange
            var mockRepo = new Mock<IAlertRuleRepository>();
            var service = new AlertRuleService(mockRepo.Object);

            var dto = new CreateAlertRuleDto
            {
                StockSymbol = StockSymbol.AAPL,
                PriceThreshold = -1,
                Direction = "Below"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto));
        }
    }
}
