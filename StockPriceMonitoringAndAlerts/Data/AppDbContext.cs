using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Create the converter
            var stockSymbolConverter = new ValueConverter<StockSymbol, string>(
                v => v.ToString(),                            // convert enum to string when saving to DB
                v => (StockSymbol)Enum.Parse(typeof(StockSymbol), v) // convert string to enum when reading from DB
            );

            // 2. Apply the converter to AlertRule.StockSymbol
            modelBuilder.Entity<AlertRule>()
                .Property(ar => ar.StockSymbol)
                .HasConversion(stockSymbolConverter);

            base.OnModelCreating(modelBuilder);
        }
    }
}
