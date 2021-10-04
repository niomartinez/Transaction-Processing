using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transaction_Processing.Models
{
    public class Transaction
    {
        public string TransIdentifier { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime TransDate { get; set; }
        public string Status { get; set; }

        public Transaction()
        {

        }
    }
}
