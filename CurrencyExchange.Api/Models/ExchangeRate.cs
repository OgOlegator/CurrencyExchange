using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyExchange.Api.Models
{
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

        public static ExchangeRate CreateByReverseExchangeRate(ExchangeRate reverseExchangeRate)
            => new ExchangeRate
            {
                BaseCurrencyId = reverseExchangeRate.TargetCurrencyId,
                TargetCurrencyId = reverseExchangeRate.BaseCurrencyId,
                CurrencyBase = reverseExchangeRate.CurrencyTarget,
                CurrencyTarget = reverseExchangeRate.CurrencyBase,
                Rate = ReverseRate(reverseExchangeRate.Rate),
            };

        public static ExchangeRate CreateByCorssRate(
            string currencyBase, 
            string currencyTarget, 
            ExchangeRate baseExchangeRate, 
            ExchangeRate targetExchangeRate)
        {
            //Получаем обменные курсы в формате Base-USD и USD-Target
            var baseRate = baseExchangeRate.CurrencyBase.Code == currencyBase 
                ? baseExchangeRate 
                : CreateByReverseExchangeRate(baseExchangeRate);

            var targetRate = targetExchangeRate.CurrencyTarget.Code == currencyTarget 
                ? targetExchangeRate 
                : CreateByReverseExchangeRate(targetExchangeRate);

            return new ExchangeRate
            {
                BaseCurrencyId = baseRate.BaseCurrencyId,
                TargetCurrencyId = targetRate.TargetCurrencyId,
                CurrencyBase = baseRate.CurrencyBase,
                CurrencyTarget = targetRate.CurrencyTarget,
                Rate = Math.Round(baseRate.Rate * targetRate.Rate, 3, mode: MidpointRounding.ToPositiveInfinity),
            };
        }

        static decimal ReverseRate(decimal rate)
                => Math.Round(1 / (rate / 1), 3, mode: MidpointRounding.ToPositiveInfinity);
    }
}
