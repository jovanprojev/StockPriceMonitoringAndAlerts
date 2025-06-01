using StockPriceMonitoringAndAlerts.DTOs.AlertRules;

namespace StockPriceMonitoringAndAlerts.Services
{
    public interface IAlertRuleService
    {
        Task<IEnumerable<AlertRuleDto>> GetAllAsync();
        Task<AlertRuleDto?> GetByIdAsync(int id);
        Task<AlertRuleDto> CreateAsync(CreateAlertRuleDto dto);
        Task<IEnumerable<AlertRuleDto>> GetActiveAlertRulesAsync();
        Task<AlertRuleDto> ActivateAsync(int id);
        Task DeleteAsync(int id);
    }
}
