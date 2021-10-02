using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Transaction_Processing.Models;
using TransactionsWebApp.Data;
using TransactionsWebApp.Helpers.ClassMappers;
using TransactionsWebApp.Helpers.LogService;
using Newtonsoft.Json;

namespace TransactionsWebApp.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        readonly Logger _logger;

        public TransactionsController(ILogger logger)
        {
            _logger = logger;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Transaction.ToListAsync());
        }

        public IActionResult UploadFile(IFormFile file, Transaction transactionModel)
        {
            List<Transaction> transactions = new();
            //read File
            #region Read CSV
            var path = file.OpenReadStream();
            CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false, Delimiter = ","
            };
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, csvConfiguration))
            {
                csv.Context.RegisterClassMap<TransactionsMap>();
                transactions = csv.GetRecords<Transaction>().ToList();
            }
            #endregion

            //Validate File columns per row
            int counter = 1;
            foreach (Transaction trans in transactions)
            {

                ValidateTransaction(trans, file,  counter);
                //transactionModel.TransIdentifier = trans.TransIdentifier;
                //transactionModel.Amount = trans.Amount;
                //transactionModel.Currency = trans.Currency;
                //transactionModel.TransDate = trans.TransDate;
                //transactionModel.Status = trans.Status;
                counter++;
            }
            //if no validations return http 200 and insert to DB
            //if has validations list validations and note file name to a log file and display validations on bad request
            return View();
        }
        #region Validate CSV Records
        public (bool, List<string>) ValidateTransaction(Transaction trans, IFormFile file, int counter)
        {
            List<string> validations = new();
            bool hasValidation = false;
            string valMsg;
            //TransIdentifier is Null, Empty, Or whitespace
            if (string.IsNullOrWhiteSpace(trans.TransIdentifier.ToString()) || string.IsNullOrEmpty(trans.TransIdentifier.ToString()))
            {
                valMsg = "Transaction Identifier is blank or empty.";
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //TransIdentifier char is > 50
            if (trans.TransIdentifier.ToString().Length > 50)
            {
                valMsg = "Transaction Identifier exceeded maximum of 50 characters.";
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //Amount is Null, Empty, Or whitespace
            if (string.IsNullOrWhiteSpace(trans.Amount.ToString()) || string.IsNullOrEmpty(trans.Amount.ToString()))
            {
                valMsg = "Amount is blank or empty.";
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //Amount is not in decimal format
            if (!decimal.TryParse(trans.Amount.ToString(), out _))
            {
                valMsg = "Amount is not in a decimal format.";
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //Currency is Null, Empty, Or whitespace
            if (string.IsNullOrWhiteSpace(trans.Currency.ToString()) || string.IsNullOrEmpty(trans.Currency.ToString()))
            {
                valMsg = "Currency is blank or empty.";
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //Currency is not in ISO4217 format
            IEnumerable<string> currencySymbols = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    .Select(x => (new RegionInfo(x.LCID)).ISOCurrencySymbol)
                    .Distinct()
                    .OrderBy(x => x);

            if (!currencySymbols.Any(stringToCheck => stringToCheck.Contains(trans.Currency.ToString())))
            {
                valMsg = "Amount is not in a decimal format.";
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //DateTime is Null, Empty, Or whitespace
            if (string.IsNullOrWhiteSpace(trans.TransDate.ToString()) || string.IsNullOrEmpty(trans.TransDate.ToString()))
            {
                valMsg = "Date is blank or empty.";
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //DateTime is not in decimal format
            if (!System.DateTime.TryParseExact(trans.TransDate.ToString(), "dd/MM/yyyy hh:mm:ss", null, DateTimeStyles.None, out _))
            {
                valMsg = "Amount is not in a correct DateTime format. (dd/MM/yyyy hh:mm:ss)";
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }

            //Status is Null, Empty, Or whitespace
            if (string.IsNullOrWhiteSpace(trans.Status.ToString()) || string.IsNullOrEmpty(trans.Status.ToString()))
            {
                valMsg = "Date is blank or empty.";
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //Status is invalid
            if (!Enum.TryParse<CsvStatuses>(trans.Status.ToString(), out _))
            {
                valMsg = "Status is invalid or not defined for CSV Statuses.";
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }


            return (hasValidation, validations);
        }



        #endregion

        #region logging
        private void Log(Transaction trans, string valMsg, IFormFile file, int counter)
        {
            _logger.Log("Invalid Record from File " + file.FileName + ": " + Environment.NewLine +
                       "Exception: " + valMsg + " on row: " + counter + Environment.NewLine +
                       JsonConvert.SerializeObject(trans) + Environment.NewLine);
        }
        #endregion
    }
}
