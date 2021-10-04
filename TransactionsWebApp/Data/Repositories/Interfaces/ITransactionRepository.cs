using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transaction_Processing.Models;

namespace TransactionsWebApp.Data.Repositories
{
    public interface ITransactionRepository : IRepository<BaseTransaction, Int32>
    {
        Task<BaseTransaction> GetTransaction(string id);
        Task<IEnumerable<BaseTransaction>> GetAllTransactions();
        Task<IEnumerable<BaseTransaction>> Search(string currency, DateTime? start, DateTime? end, string status);
    }
}