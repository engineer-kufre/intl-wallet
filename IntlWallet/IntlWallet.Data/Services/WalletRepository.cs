using IntlWallet.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntlWallet.Data.Services
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _ctx;

        public WalletRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> AddWallet(Wallet model)
        {
            await _ctx.Wallets.AddAsync(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteWallet(Wallet model)
        {
            _ctx.Wallets.Remove(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Wallet>> GetAllWallets()
        {
            var allWallets = await _ctx.Wallets.ToListAsync();
            return allWallets;
        }

        public async Task<Wallet> GetWalletByWalletCurrency(string walletCurrency)
        {
            return await _ctx.Wallets.FirstOrDefaultAsync(x => x.WalletCurrency == walletCurrency);
        }

        public async Task<Wallet> GetWalletByWalletId(string walletId)
        {
            return await _ctx.Wallets.FirstOrDefaultAsync(x => x.WalletId == walletId);
        }

        public async Task<IEnumerable<Wallet>> GetWalletsByUserId(string userId)
        {
            var userWallets = await _ctx.Wallets.Where(x => x.ApplicationUserId == userId).ToListAsync();
            return userWallets;
        }

        public async Task<bool> UpdateWallet(Wallet updateModel)
        {
            _ctx.Wallets.Update(updateModel);
            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}
