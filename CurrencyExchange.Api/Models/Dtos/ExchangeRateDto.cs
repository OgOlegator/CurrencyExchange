namespace CurrencyExchange.Api.Models.Dtos
{
    public class ExchangeRateDto
    {
        public int Id { get; set; }

        public Currency CurrencyBase { get; set; }

        public Currency CurrencyTarget { get; set; }

        public decimal Rate { get; set; }

        public ExchangeRateDto(ExchangeRate exchangeRate)
        {
            Id = exchangeRate.Id;
            CurrencyBase = exchangeRate.CurrencyBase;
            CurrencyTarget = exchangeRate.CurrencyTarget;
            Rate = exchangeRate.Rate;
        }
    }
}
