using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Transaction_Processing.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Maximum length is 50")]
        public string TransIdentifier { get; set; }
        public decimal Amount { get; set; }
        [DataType(DataType.Currency)]
        public string Currency { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime TransDate { get; set; }

        [Required]
        public string Status { get; set; }

        public Transaction()
        {

        }
    }
}
