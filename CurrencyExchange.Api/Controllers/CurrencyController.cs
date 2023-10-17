using CurrencyExchange.Api.Data;
using CurrencyExchange.Api.Models;
using CurrencyExchange.Api.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Api.Controllers
{
    [Route("api")] 
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CurrencyController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("currencies")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var currencies = await _context.Currencies.ToListAsync();
                return Ok(currencies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("currency/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                var currency = await _context.Currencies.FirstOrDefaultAsync(currency => currency.Code == code);

                if (currency == null)
                    return NotFound();

                return Ok(currency);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("currencies")]
        public async Task<IActionResult> Create([FromBody] CreateCurrencyDto currencyDto)
        {
            var newCurrency = new Currency
            {
                Code = currencyDto.Code,
                FullName = currencyDto.FullName,
                Sign = currencyDto.Sign,
            };

            try
            {
                await _context.AddAsync(newCurrency);
                await _context.SaveChangesAsync();

                return Ok(newCurrency);
            }
            catch (DbUpdateException ex)
            {
                return Conflict();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
