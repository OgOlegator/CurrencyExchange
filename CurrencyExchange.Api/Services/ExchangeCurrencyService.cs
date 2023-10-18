using CurrencyExchange.Api.Models.Dtos;
using CurrencyExchange.Api.Services.Entity;

namespace CurrencyExchange.Api.Services
{
    public class ExchangeCurrencyService : IExchangeCurrencyService
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ICurrencyService _currencyService;

        public ExchangeCurrencyService(IExchangeRateService exchangeRateService, ICurrencyService currencyService)
        {
            _exchangeRateService = exchangeRateService;
            _currencyService = currencyService;
        }

        public Task<ExchangeCurrencyDto> StartAsync(string currencyBase, string currencyTarget, decimal amount)
        {
            throw new NotImplementedException();
        }
    }
}
