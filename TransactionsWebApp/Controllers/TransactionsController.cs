using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
using TransactionsWebApp.Helpers.Utilities;
using TransactionsWebApp.Models;

namespace TransactionsWebApp.Controllers
{
    
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IXmlHandler _xmlHandler;
        private readonly ICsvHandler _csvHandler;
        private readonly ITransactionRepository _transRepo;

        public TransactionsController(ApplicationDbContext context, IXmlHandler xmlHandler, ICsvHandler csvHandler, ITransactionRepository transRepo)
        {
            _context = context;
            _xmlHandler = xmlHandler;
            _csvHandler = csvHandler;
            _transRepo = transRepo;
        }

        // GET: Transactions
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Transaction.ToListAsync());
        }

        [HttpGet("{search}")]
        public async Task<ActionResult<IEnumerable<BaseTransaction>>> Search(string currency, DateTime? start, DateTime? end, string status)
        {
            try
            {
                var re = await _transRepo.Search(currency, start, end, status);
                List<Result> resModel = new();
                foreach (BaseTransaction trans in re)
                {
                    string outStatus = "";
                    if (trans.Status.ToString().ToLower() == "Approved")
                    {
                        outStatus = "A";
                    }else if (trans.Status.ToString().ToLower() == "failed" || trans.Status.ToString().ToLower() == "rejected")
                    {
                        outStatus = "R";
                    }else if (trans.Status.ToString().ToLower() == "finished" || trans.Status.ToString().ToLower() == "done")
                    {
                        outStatus = "D";
                    }
                    Result resMod = new()
                    {
                        Id = trans.TransIdentifier.ToString(),
                        Payment = trans.Amount.ToString() + " " + trans.Currency.ToString(),
                        Status = outStatus
                    };
                    resModel.Add(resMod);
                }
                if (re.Any())
                {
                    //map data to resultModel
                    return Ok(resModel);
                }
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from database.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            (bool, string) res = await ProcessFile(file);

            if (res.Item1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return BadRequest(res.Item2);
            }
        }
        private async Task<(bool, string)> ProcessFile(IFormFile file)
        {
            (bool, string) res = (false, String.Empty);
            if (Path.GetExtension(file.FileName).ToLower().Contains(".csv"))
            {
                res = await _csvHandler.ProcessCsvAsync(file);
            }
            else if (Path.GetExtension(file.FileName).ToLower().Contains(".xml"))
            {
                res = await _xmlHandler.ProcessXmlAsync(file);
            }
            return res;
        }
    }
}
