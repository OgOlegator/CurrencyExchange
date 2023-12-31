﻿using CurrencyExchange.Api.Data;
using CurrencyExchange.Api.Services;
using CurrencyExchange.Api.Services.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.Api.Controllers
{
    [ApiController]
    public class CurrencyExchangeController : ControllerBase
    {
        private readonly IExchangeCurrencyService _exchangeCurrencyService;

        public CurrencyExchangeController(IExchangeCurrencyService exchangeCurrencyService)
        {
            _exchangeCurrencyService = exchangeCurrencyService;
        }

        /// <summary>
        /// Обмен валюты
        /// </summary>
        /// <param name="from">Из какой валюты</param>
        /// <param name="to">В какую валюту</param>
        /// <param name="amount">Количество</param>
        /// <returns>Обменный курс и стоимость обмена</returns>
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
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                return BadRequest("Введено некорректное количество");
            }
            catch 
            {
                return StatusCode(500);
            }
        }
    }
}
