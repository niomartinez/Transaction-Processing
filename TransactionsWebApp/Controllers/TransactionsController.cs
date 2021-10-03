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
using TransactionsWebApp.Helpers.ClassMappers;
using TransactionsWebApp.Helpers.LogService;
using TransactionsWebApp.Helpers.Utilities;

namespace TransactionsWebApp.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IXmlHandler _xmlHandler;

        public TransactionsController(ApplicationDbContext context, IXmlHandler xmlHandler)
        {
            _context = context;
            _xmlHandler = xmlHandler;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Transaction.ToListAsync());
        }

        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            //(bool, string) csvResult = await _csvHandler.ProcessCsvAsync(file);
            (bool, string) xmlResult = await _xmlHandler.ProcessXmlAsync(file);
            if (xmlResult.Item1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return BadRequest(xmlResult.Item2);
            }
        }
    }
}
