using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyExchange.Api.Models
{
    /// <summary>
    /// Курсы валют
    /// </summary>
    [Index("BaseCurrencyId", "TargetCurrencyId", IsUnique = true, Name = "ExchangeRatePair_Index")]
    public class ExchangeRate
    {
        [Key]
        public int Id { get; set; }

        public int BaseCurrencyId { get; set; }

        public int TargetCurrencyId { get; set; }

        public decimal Rate { get; set; }

        public Currency CurrencyBase { get; set; }

        public Currency CurrencyTarget { get; set; }

        /// <summary>
        /// Получение обратного курса
        /// </summary>
        /// <param name="exchangeRate"></param>
        /// <returns></returns>
        public static ExchangeRate CreateReverseRate(ExchangeRate exchangeRate)
            => new ExchangeRate
            {
                BaseCurrencyId = exchangeRate.TargetCurrencyId,
                TargetCurrencyId = exchangeRate.BaseCurrencyId,
                CurrencyBase = exchangeRate.CurrencyTarget,
                CurrencyTarget = exchangeRate.CurrencyBase,
                Rate = ReverseRate(exchangeRate.Rate),
            };

        /// <summary>
        /// Получение кросс-курса (из двух курсов с общей валютой одного)
        /// </summary>
        /// <param name="currencyBase"></param>
        /// <param name="currencyTarget"></param>
        /// <param name="baseExchangeRate"></param>
        /// <param name="targetExchangeRate"></param>
        /// <returns></returns>
        public static ExchangeRate CreateCorssRate(
            string currencyBase, 
            string currencyTarget, 
            ExchangeRate baseExchangeRate, 
            ExchangeRate targetExchangeRate)
        {
            //Получаем обменные курсы в формате Base-USD и USD-Target
            var baseRate = baseExchangeRate.CurrencyBase.Code == currencyBase 
                ? baseExchangeRate 
                : CreateReverseRate(baseExchangeRate);

            var targetRate = targetExchangeRate.CurrencyTarget.Code == currencyTarget 
                ? targetExchangeRate 
                : CreateReverseRate(targetExchangeRate);

            return new ExchangeRate
            {
                BaseCurrencyId = baseRate.BaseCurrencyId,
                TargetCurrencyId = targetRate.TargetCurrencyId,
                CurrencyBase = baseRate.CurrencyBase,
                CurrencyTarget = targetRate.CurrencyTarget,
                Rate = Math.Round(baseRate.Rate * targetRate.Rate, 3, mode: MidpointRounding.ToPositiveInfinity),
            };
        }

        /// <summary>
        /// Расчет обратного курса
        /// </summary>
        /// <param name="rate">Курс</param>
        /// <returns></returns>
        static decimal ReverseRate(decimal rate)
                => Math.Round(1 / (rate / 1), 3, mode: MidpointRounding.ToPositiveInfinity);
    }
}
