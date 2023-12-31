﻿using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.Api.Models.Dtos
{
    public class CreateCurrencyDto
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Sign { get; set; }
    }
}
