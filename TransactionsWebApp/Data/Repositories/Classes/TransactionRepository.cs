using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transaction_Processing.Models;
using TransactionsWebApp.Helpers.DateHelper;

namespace TransactionsWebApp.Data.Repositories
{
    public class TransactionRepository : RepositoryBase<BaseTransaction, Int32>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BaseTransaction>> GetAllTransactions()
        {
            return await Context.Transaction.ToListAsync();
        }

        public async Task<BaseTransaction> GetTransaction(string id)
        {
            return await Context.Transaction
                .FirstOrDefaultAsync(e => e.TransIdentifier == id);
        }

        public async Task<IEnumerable<BaseTransaction>> Search(string currency, DateTime? start, DateTime? end, string status)
        {
            IQueryable<BaseTransaction> query = Context.Transaction;

            if (!string.IsNullOrEmpty(currency))
            {
                query = query.Where(e => e.Currency.ToLower().Contains(currency.ToLower()));
            }
            if (start.HasValue && end.HasValue)
            {
                query = query.Where(e => e.TransDate.ToDate() >=start && e.TransDate.ToDate() <=end);
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(e => e.Status.ToLower().Contains(status.ToLower()));
            }
            return await query.ToListAsync();
        }
    }
}
