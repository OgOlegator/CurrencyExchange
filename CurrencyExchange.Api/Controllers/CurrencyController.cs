using CurrencyExchange.Api.Data;
using CurrencyExchange.Api.Models;
using CurrencyExchange.Api.Models.Dtos;
using CurrencyExchange.Api.Services.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Api.Controllers
{
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICurrencyService _currencyService;

        public CurrencyController(AppDbContext context, ICurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        /// <summary>
        /// Получить все валюты
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("currencies")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var currencies = await _currencyService.GetAll();
                return Ok(currencies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Получить валюту по коду
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("currency/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                var currency = await _currencyService.GetByCodeAsync(code);

                return Ok(currency);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Добавить валюту
        /// </summary>
        /// <param name="currencyDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("currencies")]
        public async Task<IActionResult> Create([FromBody] CreateCurrencyDto currencyDto)
        {
            var newCurrency = new Currency
            {
                Code = currencyDto.Code,
                FullName = currencyDto.Name,
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
