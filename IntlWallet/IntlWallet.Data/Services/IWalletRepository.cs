using IntlWallet.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntlWallet.Data.Services
{
    public interface IWalletRepository
    {
        Task<IEnumerable<Wallet>> GetWalletsByUserId(string userId);
        Task<Wallet> GetWalletByWalletId(string walletId);
        Task<Wallet> GetWalletByWalletCurrency(string walletCurrency);
        Task<IEnumerable<Wallet>> GetAllWallets();
        Task<bool> AddWallet(Wallet model);
        Task<bool> UpdateWallet(Wallet updateModel);
        Task<bool> DeleteWallet(Wallet model);
    }
}
