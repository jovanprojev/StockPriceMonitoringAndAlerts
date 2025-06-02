using StockPriceMonitoringAndAlerts.Models;

namespace StockPriceMonitoringAndAlerts.Repositories
{
    public interface IAlertRuleRepository
    {
        Task<IEnumerable<AlertRule>> GetAllAsync();
        Task<AlertRule?> GetByIdAsync(int id);
        Task AddAsync(AlertRule rule);
        Task SaveChangesAsync();
        Task<IEnumerable<AlertRule>> GetActiveAlertRulesAsync();
        Task DeleteAsync(int id);
    }
}
