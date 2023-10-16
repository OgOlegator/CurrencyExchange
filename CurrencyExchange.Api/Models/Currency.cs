using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.Api.Models
{
    [Index("Code", IsUnique = true, Name = "Code_Index")]
    public class Currency
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }

        public string FullName { get; set; }

        public string Sign { get; set; }

    }
}
