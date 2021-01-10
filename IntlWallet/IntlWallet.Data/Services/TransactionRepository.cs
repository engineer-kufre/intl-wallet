using IntlWallet.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntlWallet.Data.Services
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _ctx;

        public TransactionRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> AddTransaction(Transaction model)
        {
            await _ctx.Transactions.AddAsync(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactions()
        {
            var allTransactions = await _ctx.Transactions.ToListAsync();
            return allTransactions;
        }

        public async Task<Transaction> GetTransactionByTransactionId(string transactionId)
        {
            return await _ctx.Transactions.FirstOrDefaultAsync(x => x.TransactionId == transactionId);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByWalletId(string walletId)
        {
            var userTransactions = await _ctx.Transactions.Where(x => x.WalletId == walletId).ToListAsync();
            return userTransactions;
        }
    }
}
