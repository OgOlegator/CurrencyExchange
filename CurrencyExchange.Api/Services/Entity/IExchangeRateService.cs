using CurrencyExchange.Api.Models;

namespace CurrencyExchange.Api.Services.Entity
{
    public interface IExchangeRateService
    {
        Task<ExchangeRate> GetByCurrencyPairAsync(string baseCurrency, string targetCurrency);

        Task<List<ExchangeRate>> GetAllAsync();
    }
}
