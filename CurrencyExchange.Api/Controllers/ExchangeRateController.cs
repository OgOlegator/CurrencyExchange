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
    public class ExchangeRateController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ICurrencyService _currencyService;

        public ExchangeRateController(AppDbContext context, IExchangeRateService exchangeRateService, ICurrencyService currencyService)
        {
            _context = context;
            _exchangeRateService = exchangeRateService;
            _currencyService = currencyService;
        }

        /// <summary>
        /// Получить все обменные курсы
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("exchangeRates")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var exchangeRates = await _exchangeRateService.GetAllAsync();

                return Ok(exchangeRates.Select(rate => new ExchangeRateDto(rate)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Получить курс по паре валют
        /// </summary>
        /// <param name="currencyPair">Из какой валюты, в какую</param>
        /// <returns></returns>
        [HttpGet]
        [Route("exchangeRate/{currencyPair}")]
        public async Task<IActionResult> GetByCurrencyPair(string currencyPair)
        {
            if(currencyPair.Length != 6)
                return BadRequest();

            try
            {
                var exchangeRate = await _exchangeRateService.GetByCurrencyPairAsync(currencyPair.Substring(0, 3), currencyPair.Substring(3));

                return Ok(new ExchangeRateDto(exchangeRate));
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
        /// Добавить обменый курс
        /// </summary>
        /// <param name="exchangeRateDto">Новый курс</param>
        /// <returns></returns>
        [HttpPost]
        [Route("exchangeRates")]
        public async Task<IActionResult> Create([FromBody] CreateExchangeRateDto exchangeRateDto)
        {
            try
            {
                var currencyBase = await _currencyService.GetByCodeAsync(exchangeRateDto.BaseCurrencyCode);
                var currencyTarget = await _currencyService.GetByCodeAsync(exchangeRateDto.TargetCurrencyCode);

                Task.WaitAll();

                var newExchangeRate = new ExchangeRate
                {
                    BaseCurrencyId = currencyBase.Id,
                    TargetCurrencyId = currencyTarget.Id,
                    Rate = exchangeRateDto.Rate,
                    CurrencyBase = currencyBase,
                    CurrencyTarget = currencyTarget,
                };

                await _context.ExchangeRates.AddAsync(newExchangeRate);
                await _context.SaveChangesAsync();

                return Ok(new ExchangeRateDto(newExchangeRate));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DbUpdateException)
            {
                return Conflict();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Обновить курс
        /// </summary>
        /// <param name="updExchangeRateDto">Значение курса</param>
        /// <param name="currencyPair">Для какой пары валют?</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("exchangeRate/{currencyPair}")]
        public async Task<IActionResult> Update([FromBody] UpdateExchangeRateDto updExchangeRateDto, string currencyPair)
        {
            try
            {
                var updExchangeRate = await _exchangeRateService.GetByCurrencyPairAsync(currencyPair.Substring(0, 3), currencyPair.Substring(3));

                updExchangeRate.Rate = updExchangeRateDto.Rate;

                _context.ExchangeRates.Update(updExchangeRate);
                await _context.SaveChangesAsync();

                return Ok(new ExchangeRateDto(updExchangeRate));
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
    }
}
