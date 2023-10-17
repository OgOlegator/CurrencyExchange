using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.Api.Models.Dtos
{
    public class CreateExchangeRateDto
    {
        [Required]
        public string BaseCurrencyCode { get; set; }

        [Required]
        public string TargetCurrencyCode { get; set; }

        [Required]
        public decimal Rate { get; set; }
    }
}
