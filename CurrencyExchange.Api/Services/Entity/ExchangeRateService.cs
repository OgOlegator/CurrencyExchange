using CurrencyExchange.Api.Data;
using CurrencyExchange.Api.Models;
using CurrencyExchange.Api.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Api.Services.Entity
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly AppDbContext _context;
        private readonly ICurrencyService _currencyService;

        public ExchangeRateService(AppDbContext context, ICurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        public async Task<List<ExchangeRate>> GetAllAsync()
        {
            var exchangeRates = await _context.ExchangeRates
                .Join(_context.Currencies,
                    exchangeRate => exchangeRate.BaseCurrencyId,
                    currency => currency.Id,
                    (exchangeRate, currency) => new
                    {
                        exchangeRate = exchangeRate,
                        baseCurrency = currency
                    })
                .Join(_context.Currencies,
                    exchangeRate => exchangeRate.exchangeRate.TargetCurrencyId,
                    currency => currency.Id,
                    (exchangeRate, currency) => new
                    {
                        exchangeRate = exchangeRate.exchangeRate,
                        baseCurrency = exchangeRate.baseCurrency,
                        targetCurrency = currency
                    })
                .Select(result => new ExchangeRate
                {
                    Id = result.exchangeRate.Id,
                    Rate = result.exchangeRate.Rate,
                    BaseCurrencyId = result.exchangeRate.BaseCurrencyId,
                    TargetCurrencyId = result.exchangeRate.TargetCurrencyId,
                    CurrencyBase = result.baseCurrency,
                    CurrencyTarget = result.targetCurrency,
                })
                .ToListAsync();

            return exchangeRates;
        }

        public async Task<ExchangeRate> GetByCurrencyPairAsync(string baseCurrency, string targetCurrency)
        {
            try
            {
                var currencyBase = await _currencyService.GetByCodeAsync(baseCurrency);
                var currencyTarget = await _currencyService.GetByCodeAsync(targetCurrency);

                var exchangeRate = await _context.ExchangeRates
                    .FirstOrDefaultAsync(rate
                    => rate.BaseCurrencyId == currencyBase.Id
                    && rate.TargetCurrencyId == currencyTarget.Id);

                if (exchangeRate == null)
                    throw new KeyNotFoundException();

                exchangeRate.CurrencyBase = currencyBase;
                exchangeRate.CurrencyTarget = currencyTarget;

                return exchangeRate;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
