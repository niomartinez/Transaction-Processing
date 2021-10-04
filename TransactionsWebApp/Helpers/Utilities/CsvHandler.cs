using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Transaction_Processing.Models;
using TransactionsWebApp.Data;
using TransactionsWebApp.Data.Repositories;
using TransactionsWebApp.Helpers.ClassMappers;
using TransactionsWebApp.Helpers.LogService;

namespace TransactionsWebApp.Helpers.Utilities
{
    public class CsvHandler : FileHandler, ICsvHandler
    {

        private readonly ApplicationDbContext _context;
        private readonly ITransactionRepository _transRepo;
        private readonly ILogger _logger;

        public CsvHandler(ApplicationDbContext context, ILogger logger, ITransactionRepository transRepo)
        {
            _context = context;
            _logger = logger;
            _transRepo = transRepo;
        }
        public async Task<(bool,string)> ProcessCsvAsync(IFormFile file)
        {
            List<TransactionCsv> transactions = new();

            //read File
            #region Read CSV
            var path = file.OpenReadStream();
            CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Delimiter = ","
            };
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, csvConfiguration))
            {
                csv.Context.RegisterClassMap<TransactionsMap>();
                transactions = csv.GetRecords<TransactionCsv>().ToList();
            }
            #endregion
            //Validate File per record
            #region Validate CSV per Record
            int counter = 1;
            List<List<string>> validations = new();
            foreach (TransactionCsv trans in transactions)
            {
                if (ValidateTransaction(trans, file, counter).Item1)
                {
                    validations.Add(ValidateTransaction(trans, file, counter).Item2);
                }
                counter++;
            }
            #endregion
            //Create and insert transactions
            #region No Validations
            if (validations.Count <= 0)// file has no validations
            {
                foreach (TransactionCsv trans in transactions)
                {
                    TransactionCsv transactionModel = new()
                    {
                        TransIdentifier = trans.TransIdentifier,
                        Amount = trans.Amount,
                        Currency = trans.Currency,
                        TransDate = trans.TransDate,
                        Status = trans.Status

                    };
                        _context.Add(transactionModel);
                        await _context.SaveChangesAsync();
                }
                return (true, String.Empty);
            }
            #endregion
            //Return and log Validations
            #region Has Validations
            else
            {
                string valMsg = "Bad Request: CSV Validation(s) below: " + Environment.NewLine;
                foreach (List<string> list in validations)
                {
                    foreach (string s in list)
                    {
                        valMsg += s;
                    }
                    valMsg += Environment.NewLine;
                }
                valMsg += "Refer to application logs for more details. (C:\\Users\\{UserName}\\Documents\\TransactionServiceLogs)" + Environment.NewLine;
                return (false, valMsg);
            }
            #endregion
        }

        public (bool, List<string>) ValidateTransaction(TransactionCsv trans, IFormFile file, int counter)
        {
            #region Record validation method
            List<string> validations = new();
            bool hasValidation = false;
            string valMsg;
            //TransIdentifier is Null, Empty, Or whitespace
            if (string.IsNullOrWhiteSpace(trans.TransIdentifier.ToString()) || string.IsNullOrEmpty(trans.TransIdentifier.ToString()))
            {
                valMsg = "Transaction Identifier is blank or empty." + Environment.NewLine;
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //TransIdentifier Exists
            Expression<Func<TransactionCsv, bool>> filter = (e) => e.TransIdentifier == trans.TransIdentifier;
            var opp = _transRepo.RetrieveAll(filter);
            if (opp.Count != 0)
            {
                valMsg = "Record already exists: " + trans.TransIdentifier.ToString() + Environment.NewLine;
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //TransIdentifier char is > 50
            if (trans.TransIdentifier.ToString().Length > 50)
            {
                valMsg = "Transaction Identifier exceeded maximum of 50 characters." + Environment.NewLine;
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //Amount is Null, Empty, Or whitespace
            if (string.IsNullOrWhiteSpace(trans.Amount.ToString()) || string.IsNullOrEmpty(trans.Amount.ToString()))
            {
                valMsg = "Amount is blank or empty." + Environment.NewLine;
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //Amount is not in decimal format
            if (!decimal.TryParse(trans.Amount.ToString(), out _))
            {
                valMsg = "Amount is not in a decimal format." + Environment.NewLine;
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //Currency is Null, Empty, Or whitespace
            if (string.IsNullOrWhiteSpace(trans.Currency.ToString()) || string.IsNullOrEmpty(trans.Currency.ToString()))
            {
                valMsg = "Currency is blank or empty." + Environment.NewLine;
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
                valMsg = "Currency is not in ISO2417 Format." + Environment.NewLine;
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //DateTime is Null, Empty, Or whitespace
            if (string.IsNullOrWhiteSpace(trans.TransDate.ToString()) || string.IsNullOrEmpty(trans.TransDate.ToString()))
            {
                valMsg = "Transaction Date is blank or empty." + Environment.NewLine;
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //DateTime is not in decimal format
            if (!System.DateTime.TryParseExact(trans.TransDate.ToString(), "dd/MM/yyyy hh:mm:ss", null, DateTimeStyles.None, out _))
            {
                valMsg = "Transaction Date is not in a correct DateTime format. (dd/MM/yyyy hh:mm:ss)" + Environment.NewLine;
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }

            //Status is Null, Empty, Or whitespace
            if (string.IsNullOrWhiteSpace(trans.Status.ToString()) || string.IsNullOrEmpty(trans.Status.ToString()))
            {
                valMsg = "Date is blank or empty." + Environment.NewLine;
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }
            //Status is invalid
            if (!Enum.TryParse<CsvStatuses>(trans.Status.ToString(), out _))
            {
                valMsg = "Status is invalid or not defined for CSV Statuses." + Environment.NewLine;
                Log(trans, valMsg, file, counter);
                validations.Add(valMsg);
                hasValidation = true;
            }


            return (hasValidation, validations);
            #endregion
        }

        public void Log(TransactionCsv trans, string valMsg, IFormFile file, int counter)
        {
            #region Logging
            _logger.Log("Invalid Record from File " + file.FileName + ": " + Environment.NewLine +
                       "Exception: " + valMsg + " on row: " + counter + Environment.NewLine +
                       JsonConvert.SerializeObject(trans) + Environment.NewLine);
            #endregion
        }

    }
}
