using System;
using Transaction_Processing.Models;

namespace TransactionsWebApp.Data.Repositories
{
    public interface ITransactionRepository : IRepository<TransactionCsv, Int32>
    {

    }
}