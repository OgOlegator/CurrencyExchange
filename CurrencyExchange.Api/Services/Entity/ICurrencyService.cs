using CurrencyExchange.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.Api.Services.Entity
{
    public interface ICurrencyService
    {
        Task<List<Currency>> GetAll();

        Task<Currency> GetByCodeAsync(string code);
    }
}
