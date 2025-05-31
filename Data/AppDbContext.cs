using Microsoft.EntityFrameworkCore;
using StockPriceMonitoringAndAlerts.Models;


namespace StockPriceMonitoringAndAlerts.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Alert> Alerts { get; set; }
        public DbSet<AlertRule> AlertRules { get; set; }
    }
}
