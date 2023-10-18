using CurrencyExchange.Api.Models.Dtos;

namespace CurrencyExchange.Api.Services
{
    public interface IExchangeCurrencyService
    {
        Task<ExchangeCurrencyDto> StartAsync(string currencyBase, string currencyTarget, decimal amount);
    }
}
