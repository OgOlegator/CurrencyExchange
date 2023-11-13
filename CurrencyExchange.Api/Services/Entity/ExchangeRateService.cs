using CurrencyExchange.Api.Data;
using CurrencyExchange.Api.Models;
using CurrencyExchange.Api.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Api.Services.Entity
{
    /// <summary>
    /// Сервис обертка над сущностью БД Курсы валют
    /// </summary>
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly AppDbContext _context;
        private readonly ICurrencyService _currencyService;

        public ExchangeRateService(AppDbContext context, ICurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        /// <summary>
        /// Получить все курсы
        /// </summary>
        /// <returns></returns>
        public async Task<List<ExchangeRate>> GetAllAsync()
        {
            var exchangeRates = await _context.ExchangeRates
                .Include(rate => rate.CurrencyBase)
                .Include(rate => rate.CurrencyTarget)
                .ToListAsync();

            return exchangeRates;
        }

        /// <summary>
        /// Получить курсы валют по валютной паре
        /// </summary>
        /// <param name="baseCurrency">Из какой валюты</param>
        /// <param name="targetCurrency">В какую валюту</param>
        /// <returns></returns>
        public async Task<ExchangeRate> GetByCurrencyPairAsync(string baseCurrency, string targetCurrency)
        {
            try
            {
                var exchangeRate = await _context.ExchangeRates
                    .Include(rate => rate.CurrencyBase)
                    .Include(rate => rate.CurrencyTarget)
                    .SingleOrDefaultAsync(rate 
                        => rate.CurrencyBase.Code == baseCurrency 
                        && rate.CurrencyTarget.Code == targetCurrency);

                if (exchangeRate == null)
                    throw new KeyNotFoundException();

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
