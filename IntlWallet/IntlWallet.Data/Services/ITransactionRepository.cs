using IntlWallet.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntlWallet.Data.Services
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetTransactionsByWalletId(string walletId);
        Task<Transaction> GetTransactionByTransactionId(string transactionId);
        Task<IEnumerable<Transaction>> GetAllTransactions();
        Task<bool> AddTransaction(Transaction model);
    }
}
