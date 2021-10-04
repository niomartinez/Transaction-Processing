using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Transaction_Processing.Models
{
    [XmlRoot(ElementName = "PaymentDetails")]
    public class PaymentDetails
    {
        [XmlElement(ElementName = "Amount")]
        public string Amount { get; set; }
        [XmlElement(ElementName = "CurrencyCode")]
        public string CurrencyCode { get; set; }
    }

    [XmlRoot(ElementName = "Transaction")]
    public class Transaction
    {
        [XmlElement(ElementName = "TransactionDate")]
        public string TransactionDate { get; set; }
        [XmlElement(ElementName = "PaymentDetails")]
        public PaymentDetails PaymentDetails { get; set; }
        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot("Transactions")]
    public class Transactions
    {
        [XmlElement(ElementName = "Transaction")]
        public List<Transaction> Transaction { get; set; }
    }

    public class BaseTransaction
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

        public BaseTransaction()
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
