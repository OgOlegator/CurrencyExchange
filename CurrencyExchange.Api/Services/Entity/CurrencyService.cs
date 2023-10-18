using CurrencyExchange.Api.Data;
using CurrencyExchange.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Api.Services.Entity
{
    public class CurrencyService : ICurrencyService
    {
        private readonly AppDbContext _context;

        public CurrencyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Currency>> GetAll()
        {
            try
            {
                return await _context.Currencies.ToListAsync();
            }
            catch 
            {
                throw;
            }
        }

        public async Task<Currency> GetByCodeAsync(string code)
        {
            var currency = await _context.Currencies.FirstOrDefaultAsync(currency => currency.Code == code);

            if (currency == null)
                throw new KeyNotFoundException();

            return currency;
        }
    }
}
