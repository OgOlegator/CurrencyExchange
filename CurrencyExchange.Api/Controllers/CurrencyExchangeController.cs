using CurrencyExchange.Api.Data;
using CurrencyExchange.Api.Services;
using CurrencyExchange.Api.Services.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class CurrencyExchangeController : ControllerBase
    {
        private readonly IExchangeCurrencyService _exchangeCurrencyService;

        public CurrencyExchangeController(IExchangeCurrencyService exchangeCurrencyService)
        {
            _exchangeCurrencyService = exchangeCurrencyService;
        }

        [HttpGet]
        [Route("exchange")]
        public async Task<IActionResult> Exchange(string from, string to, string amount)
        {
            try
            {
                var result = await _exchangeCurrencyService.StartAsync(from, to, decimal.Parse(amount));

                return Ok(result);
            }
            catch (Exception ex) when (ex is KeyNotFoundException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
