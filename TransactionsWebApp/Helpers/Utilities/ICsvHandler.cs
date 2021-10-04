using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transaction_Processing.Models;

namespace TransactionsWebApp.Helpers.Utilities
{
    public interface ICsvHandler
    {
        public Task<(bool, string)> ProcessCsvAsync(IFormFile file);
        public (bool, List<string>) ValidateTransaction(BaseTransaction trans, IFormFile file, int counter);
        public void Log(BaseTransaction trans, string valMsg, IFormFile file, int counter);
    }
}
