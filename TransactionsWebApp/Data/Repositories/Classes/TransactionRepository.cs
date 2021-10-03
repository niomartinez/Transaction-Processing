using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Transaction_Processing.Models;

namespace TransactionsWebApp.Data.Repositories
{
    public class TransactionRepository : RepositoryBase<Transaction, Int32>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
