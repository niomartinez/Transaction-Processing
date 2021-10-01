using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Transaction_Processing.Models;
using TransactionsWebApp.Data;
using TransactionsWebApp.Helpers.ClassMappers;

namespace TransactionsWebApp.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Transaction.ToListAsync());
        }

        public IActionResult UploadFile(IFormFile file)
        {
            List<Transaction> transactions = new();
            //read File
            #region Read CSV
            var path = file.OpenReadStream();
            CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, csvConfiguration))
            {
                csv.Context.RegisterClassMap<TransactionsMap>();
                while (csv.Read())
                {
                    var transaction = csv.GetRecord<Transaction>();
                    transactions.Add(transaction);
                }
            }
            #endregion

            //Validate File columns per row
            //if no validations return http 200 and insert to DB
            //if has validations list validations and note file name to a log file and display validations on bad request
            return View();
        }
        #region Validate CSV
        public void ValidateCsv(string fileContents)
        {
            var fileLines = fileContents.Split(
                  new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            if (fileLines.Length < 2)
                //fail - no data row.

                ValidateColumnHeader(fileLines[0]);

            ValidateRows(fileLines.Skip(1));
        }

        public bool ValidateColumnHeader(string header)
        {
            return header.Trim().Replace(' ', (char)0).ToLower() ==
               "TransactionIdentificator,Amount,CurrencyCode,TransactionDate,Status";
        }

        public bool ValidateRows(IEnumerable<string> rows)
        {
            foreach (var row in rows)
            {
                var cells = row.Split(',');

                //check if the number of cells is correct
                if (cells.Length != 4)
                    return false;

                //ensure gender is correct
                if (cells[3] != "M" && cells[3] != "F")
                    return false;

                //perform any additional row checks relevant to your domain
            }
            return true;
        }
        #endregion
        
    }
}
