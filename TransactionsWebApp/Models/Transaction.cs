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
        [Key]
        [Required]
        [Display(Name = "Transaction Identifier")]
        public string TransIdentifier { get; set; }
        public string Amount { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        [Display(Name = "Transaction Date")]
        public string TransDate { get; set; }
        [Required]
        public string Status { get; set; }

        public Transaction()
        {

        }
    }

    enum CsvStatuses
    {
        Approved, Failed, Finished
    }
    enum XmlStatuses
    {
        Approved, Rejected, Done
    }

}
