using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transaction_Processing.Models;

namespace TransactionsWebApp.Helpers.Utilities
{
    public interface IXmlHandler
    {
        void Log(Transaction trans, string valMsg, IFormFile file, int counter);
        Task<(bool, string)> ProcessXmlAsync(IFormFile file);
        (bool, List<string>) ValidateTransaction(Transaction trans, IFormFile file, int counter);
    }
}