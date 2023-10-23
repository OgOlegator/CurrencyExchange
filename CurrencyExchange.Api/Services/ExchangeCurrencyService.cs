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

            return new ExchangeCurrencyDto
            {
                BaseCurrency = exchangeRate.CurrencyBase,
                TargetCurrency = exchangeRate.CurrencyTarget,
                Rate = exchangeRate.Rate,
                Amount = amount,
                ConvertedAmount = exchangeRate.Rate * amount,
            };
        }

        /// <summary>
        /// Получить курс валют
        /// </summary>
        /// <param name="currencyBase"></param>
        /// <param name="currencyTarget"></param>
        /// <returns></returns>
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
                        result = await GetExchangeRateByCrossRate(currencyBase, currencyTarget);
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

        /// <summary>
        /// Получение курса валют по кросс-курсу, т. е. где есть базовая, целевая и общая у этих двух валюта
        /// </summary>
        /// <param name="currencyBase"></param>
        /// <param name="currencyTarget"></param>
        /// <returns></returns>
        private async Task<ExchangeRate> GetExchangeRateByCrossRate(string currencyBase, string currencyTarget)
        {
            var exchangeRates = await _exchangeRateService.GetAllAsync();

            //Получаем все обменные курсы где испоьзуется базовая и целевая валюты
            var exchangeRatesWithBase = GetExchangeRatesWithCurrency(currencyBase);
            var exchangeRatesWithTarget = GetExchangeRatesWithCurrency(currencyTarget);

            if (exchangeRatesWithBase.Count() == 0 || exchangeRatesWithTarget.Count() == 0)
                return null;

            //Получаем пары курсов с общей валютой
            var intersectExchangeRates = from baseRate in exchangeRatesWithBase
                                         from targetRate in exchangeRatesWithTarget
                                         where (baseRate.BaseCurrencyId == targetRate.BaseCurrencyId
                                            || baseRate.BaseCurrencyId == targetRate.TargetCurrencyId
                                            || baseRate.TargetCurrencyId == targetRate.BaseCurrencyId
                                            || baseRate.TargetCurrencyId == targetRate.TargetCurrencyId)
                                         select new {baseRate = baseRate, targetRate = targetRate };

            if (!intersectExchangeRates.Any())
                return null;

            var firstIntersectRate = intersectExchangeRates.First();

            return ExchangeRate
                .CreateByCorssRate(currencyBase, currencyTarget, firstIntersectRate.baseRate, firstIntersectRate.targetRate);

            IEnumerable<ExchangeRate> GetExchangeRatesWithCurrency(string currencyCode)
                => exchangeRates
                .Where(rate => rate.CurrencyBase.Code == currencyCode)
                .Union(exchangeRates
                    .Where(rate => rate.CurrencyTarget.Code == currencyCode));
        }
    }
}
