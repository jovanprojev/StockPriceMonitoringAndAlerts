using Microsoft.EntityFrameworkCore;
using StockPriceMonitoringAndAlerts.Data;
using StockPriceMonitoringAndAlerts.Models;
using System.Linq.Expressions;

namespace StockPriceMonitoringAndAlerts.Repositories
{
    public class AlertRuleRepository : IAlertRuleRepository
    {
        private readonly AppDbContext _context;

        public AlertRuleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AlertRule>> GetAllAsync()
        {
            return await _context.AlertRules.ToListAsync();
        }

        public async Task<AlertRule?> GetByIdAsync(int id)
        {
            return await _context.AlertRules.FindAsync(id);
        }

        public async Task AddAsync(AlertRule rule)
        {
            await _context.AlertRules.AddAsync(rule);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AlertRule>> GetActiveAlertRulesAsync()
        {
            return await _context.AlertRules
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rule = await GetByIdAsync(id);
            if (rule != null)
            {
                _context.AlertRules.Remove(rule);
                await SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<AlertRule>> FindAsync(Expression<Func<AlertRule, bool>> predicate)
        {
            return await _context.AlertRules.Where(predicate).ToListAsync();
        }
    }
}
