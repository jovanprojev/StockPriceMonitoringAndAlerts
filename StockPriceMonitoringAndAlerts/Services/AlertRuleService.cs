using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockPriceMonitoringAndAlerts.DTOs.AlertRules;
using StockPriceMonitoringAndAlerts.Models;
using StockPriceMonitoringAndAlerts.Repositories;

namespace StockPriceMonitoringAndAlerts.Services
{
    public class AlertRuleService : IAlertRuleService
    {
        private readonly IAlertRuleRepository _repository;

        public AlertRuleService(IAlertRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AlertRuleDto>> GetAllAsync()
        {
            var rules = await _repository.GetAllAsync();
            return rules.Select(r => new AlertRuleDto
            {
                Id = r.Id,
                StockSymbol = r.StockSymbol,
                PriceThreshold = r.PriceThreshold,
                Direction = r.Direction.ToString(),
                CreatedAt = r.CreatedAt,
                IsActive = r.IsActive,
            });
        }

        public async Task<IEnumerable<AlertRuleDto>> GetActiveAlertRulesAsync()
        {
            var activeRules = await _repository.GetActiveAlertRulesAsync();
            return activeRules.Select(r => new AlertRuleDto
            {
                Id = r.Id,
                StockSymbol = r.StockSymbol,
                PriceThreshold = r.PriceThreshold,
                Direction = r.Direction.ToString(),
                CreatedAt = r.CreatedAt,
                IsActive = r.IsActive
            });
        }

        public async Task<AlertRuleDto?> GetByIdAsync(int id)
        {
            var rule = await _repository.GetByIdAsync(id);
            if (rule == null) return null;

            return new AlertRuleDto
            {
                Id = rule.Id,
                StockSymbol = rule.StockSymbol,
                PriceThreshold = rule.PriceThreshold,
                Direction = rule.Direction.ToString(),
                CreatedAt = rule.CreatedAt,
                IsActive = rule.IsActive
            };
        }

        public async Task<AlertRuleDto> CreateAsync(CreateAlertRuleDto dto)
        {
            if (!Enum.TryParse<Direction>(dto.Direction, true, out var direction))
                throw new ArgumentException("Invalid direction value");

            if (dto.PriceThreshold <= 0)
                throw new ArgumentException("Price threshold must be greater than zero.");

            var existingRule = await _repository.FindAsync(r =>
                r.StockSymbol == dto.StockSymbol &&
                r.PriceThreshold == dto.PriceThreshold &&
                r.Direction == direction &&
                r.IsActive == false);

            if (existingRule.Any())
                throw new ArgumentException("A similar alert rule already exists.");

            var rule = new AlertRule
            {
                StockSymbol = dto.StockSymbol,
                PriceThreshold = dto.PriceThreshold,
                Direction = direction,
                CreatedAt = DateTime.Now,
                IsActive = false
            };

            await _repository.AddAsync(rule);
            await _repository.SaveChangesAsync();

            return new AlertRuleDto
            {
                Id = rule.Id,
                StockSymbol = rule.StockSymbol,
                PriceThreshold = rule.PriceThreshold,
                Direction = rule.Direction.ToString(),
                CreatedAt = rule.CreatedAt,
                IsActive = rule.IsActive
            };
        }

        public async Task DeleteAsync(int id)
        {
            var rule = await _repository.GetByIdAsync(id);
            if (rule == null) throw new KeyNotFoundException("Alert rule not found");
            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }

    }
}
