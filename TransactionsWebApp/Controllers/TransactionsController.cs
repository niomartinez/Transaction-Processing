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

        public TransactionsController(Logger logger)
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
            //TransIdentifier char is > 50
            if (trans.TransIdentifier.Length > 50)
            {
                valMsg = "Transaction Identifier exceeded maximum of 50 characters.";
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //TransIdentifier is Null, Empty, Or whitespace
            if (string.IsNullOrWhiteSpace(trans.TransIdentifier) || string.IsNullOrEmpty(trans.TransIdentifier))
            {
                valMsg = "Transaction Identifier is blank or empty.";
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }

            if (!decimal.TryParse(trans.Amount.ToString(), out _))
            {
                valMsg = "Amount is not in a decimal format.";
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
