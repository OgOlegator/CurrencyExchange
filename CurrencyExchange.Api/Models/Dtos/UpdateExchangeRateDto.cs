using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.Api.Models.Dtos
{
    public class UpdateExchangeRateDto
    {
        [Required]
        public decimal Rate { get; set; }
    }
}
