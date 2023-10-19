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
                Rate = 1 / (reverseExchangeRate.Rate / 1),
            };
    }
}
