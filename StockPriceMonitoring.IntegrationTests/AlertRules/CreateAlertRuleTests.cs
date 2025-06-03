using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using StockPriceMonitoringAndAlerts.Data;
using StockPriceMonitoringAndAlerts.DTOs.AlertRules;
using StockPriceMonitoringAndAlerts.Models;
using System.Net;
using System.Net.Http.Json;


namespace StockPriceMonitoring.IntegrationTests.AlertRules
{
    public class CreateAlertRuleTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public CreateAlertRuleTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostAlertRule_ShouldSucceed_WhenValidInput()
        {
            // Clear database first
            using (var scope1 = _factory.Services.CreateScope())
            {
                var db1 = scope1.ServiceProvider.GetRequiredService<AppDbContext>();
                db1.AlertRules.RemoveRange(db1.AlertRules);
                await db1.SaveChangesAsync();
            }

            // Arrange
            var dto = new CreateAlertRuleDto
            {
                StockSymbol = StockSymbol.AAPL,
                PriceThreshold = 150,
                Direction = "Above"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/AlertRules", dto);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}\n{response.Content}");
            }

            // Assert
            response.EnsureSuccessStatusCode();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var rule = await db.AlertRules.FirstOrDefaultAsync();
            Assert.NotNull(rule);
            Assert.Equal(dto.PriceThreshold, rule.PriceThreshold);
            Assert.Equal(dto.StockSymbol, rule.StockSymbol);
            Assert.Equal(Direction.Above, rule.Direction);
        }

        [Fact]
        public async Task GetAlertRules_ShouldReturnOk_WithData()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Seed data
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.AlertRules.Add(new AlertRule
            {
                StockSymbol = StockSymbol.AAPL,
                PriceThreshold = 150,
                Direction = Direction.Above
            });
            db.SaveChanges();

            // Act
            var response = await client.GetAsync("/api/alertrules");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content.Contains("AAPL");
        }

        [Fact]
        public async Task PostAlertRule_ShouldFail_WhenInvalidInput()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/alertrules", new { });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostAlertRule_ShouldFail_WhenDuplicate()
        {
            var rule = new
            {
                StockSymbol = "AAPL",
                PriceThreshold = 150,
                Direction = "Above"
            };

            var client = _factory.CreateClient();

            var firstResponse = await client.PostAsJsonAsync("/api/alertrules", rule);
            firstResponse.EnsureSuccessStatusCode();

            var secondResponse = await client.PostAsJsonAsync("/api/alertrules", rule);
            secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }
}
