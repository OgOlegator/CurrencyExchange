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
    public class ExchangeRateController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExchangeRateController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("exchangeRates")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var exchangeRates = await _context.ExchangeRates
                    .Join(_context.Currencies,
                        exchangeRate => exchangeRate.BaseCurrencyId,
                        currency => currency.Id,
                        (exchangeRate, currency) => new
                        {
                            exchangeRate = exchangeRate,
                            baseCurrency = currency
                        })
                    .Join(_context.Currencies,
                        exchangeRate => exchangeRate.exchangeRate.TargetCurrencyId,
                        currency => currency.Id,
                        (exchangeRate, currency) => new
                        {
                            exchangeRate = exchangeRate.exchangeRate,
                            baseCurrency = exchangeRate.baseCurrency,
                            targetCurrency = currency
                        })
                    .Select(result => new ExchangeRateDto(result.exchangeRate, result.baseCurrency, result.targetCurrency))
                    .ToListAsync();

                return Ok(exchangeRates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("exchangeRate/{currencyPair}")]
        public async Task<IActionResult> GetByCurrencyPair(string currencyPair)
        {
            if(currencyPair.Length != 6)
                return BadRequest();

            try
            {
                var exchangeRate = await GetExchangeRate(currencyPair.Substring(0, 3), currencyPair.Substring(3));

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

        [HttpPost]
        [Route("exchangeRates")]
        public async Task<IActionResult> Create([FromBody] CreateExchangeRateDto exchangeRateDto)
        {
            try
            {
                var currencyBase = GetCurrencyByCode(exchangeRateDto.BaseCurrencyCode);
                var currencyTarget = GetCurrencyByCode(exchangeRateDto.TargetCurrencyCode);

                var newExchangeRate = new ExchangeRate
                {
                    BaseCurrencyId = currencyBase.Result.Id,
                    TargetCurrencyId = currencyTarget.Result.Id,
                    Rate = exchangeRateDto.Rate,
                    CurrencyBase = currencyBase.Result,
                    CurrencyTarget = currencyTarget.Result,
                };

                await _context.ExchangeRates.AddAsync(newExchangeRate);
                await _context.SaveChangesAsync();

                return Ok(newExchangeRate);
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

        [HttpPatch]
        [Route("exchangeRate/{currencyPair}")]
        public async Task<IActionResult> Update([FromBody] UpdateExchangeRateDto updExchangeRateDto, string currencyPair)
        {
            try
            {
                var updExchangeRate = await GetExchangeRate(currencyPair.Substring(0, 3), currencyPair.Substring(3));

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

        async Task<Currency> GetCurrencyByCode(string code)
        {
            var currency = await _context.Currencies.FirstOrDefaultAsync(currency => currency.Code == code);

            if (currency == null)
                throw new KeyNotFoundException();

            return currency;
        }

        async Task<ExchangeRate> GetExchangeRate(string baseCurrency, string targetCurrency)
        {
            try
            {
                var currencyBase = await GetCurrencyByCode(baseCurrency);
                var currencyTarget = await GetCurrencyByCode(targetCurrency);

                var exchangeRate = await _context.ExchangeRates
                    .FirstOrDefaultAsync(rate
                    => rate.BaseCurrencyId == currencyBase.Id
                    && rate.TargetCurrencyId == currencyTarget.Id);

                if (exchangeRate == null)
                    throw new KeyNotFoundException();

                exchangeRate.CurrencyBase = currencyBase;
                exchangeRate.CurrencyTarget = currencyTarget;

                return exchangeRate;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
