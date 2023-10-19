using CurrencyExchange.Api.Models;
using CurrencyExchange.Api.Models.Dtos;
using CurrencyExchange.Api.Services.Entity;

namespace CurrencyExchange.Api.Services
{
    public class ExchangeCurrencyService : IExchangeCurrencyService
    {
        private readonly IExchangeRateService _exchangeRateService;

        public ExchangeCurrencyService(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        public async Task<ExchangeCurrencyDto> StartAsync(string currencyBase, string currencyTarget, decimal amount)
        {
            var exchangeRate = await GetExchangeRate(currencyBase, currencyTarget);

            return new ExchangeCurrencyDto();
        }

        private async Task<ExchangeRate> GetExchangeRate(string currencyBase, string currencyTarget)
        {
            try
            {
                var result = await Get(currencyBase, currencyTarget);

                if (result == null)
                {
                    var reverseExchangeRate = await Get(currencyTarget, currencyBase);

                    if (reverseExchangeRate != null)
                        result = ExchangeRate.CreateByReverseExchangeRate(reverseExchangeRate);

                    if (result == null)
                        result = await GetExchangeRateByRateWithSameCurrency(currencyBase, currencyTarget);
                }

                return result ?? throw new KeyNotFoundException("Курс не найден");
            }
            catch
            {
                throw;
            }

            async Task<ExchangeRate> Get(string from, string to)
            {
                try
                {
                    return await _exchangeRateService.GetByCurrencyPairAsync(from, to);
                }
                catch (KeyNotFoundException)
                {
                    return null;
                }
            }
        }

        private async Task<ExchangeRate> GetExchangeRateByRateWithSameCurrency(string currencyBase, string currencyTarget)
        {
            var exchangeRates = await _exchangeRateService.GetAllAsync();

            var exchangeRatesWithBase = GetExchangeRatesWithCurrency(currencyBase);
            var exchangeRatesWithTarget = GetExchangeRatesWithCurrency(currencyTarget);

            if (exchangeRatesWithBase.Count() == 0 || exchangeRatesWithTarget.Count() == 0)
                return null;

            return exchangeRates.First();

            IEnumerable<ExchangeRate> GetExchangeRatesWithCurrency(string currencyCode)
                => exchangeRates
                .Where(rate => rate.CurrencyBase.Code == currencyCode)
                .Union(exchangeRates
                    .Where(rate => rate.CurrencyTarget.Code == currencyCode));
        }
    }
}
