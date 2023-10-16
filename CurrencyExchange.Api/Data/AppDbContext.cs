using CurrencyExchange.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ExchangeRate>()
                .HasOne(exchangeRate => exchangeRate.CurrencyBase)
                .WithMany()
                .HasForeignKey(currency => currency.BaseCurrencyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<ExchangeRate>()
                .HasOne(exchangeRate => exchangeRate.CurrencyTarget)
                .WithMany()
                .HasForeignKey(currency => currency.TargetCurrencyId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
