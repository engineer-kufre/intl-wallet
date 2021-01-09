using IntlWallet.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntlWallet.Data.Services
{
    public class UserMainCurrencyRepository : IUserMainCurrencyRepository
    {
        private readonly AppDbContext _ctx;

        public UserMainCurrencyRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> AddMainCurrency(UserMainCurrencyDetail model)
        {
            await _ctx.UserMainCurrencyDetails.AddAsync(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteMainCurrency(UserMainCurrencyDetail model)
        {
            _ctx.UserMainCurrencyDetails.Remove(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<UserMainCurrencyDetail> GetMainCurrencyByUserId(string userId)
        {
            return await _ctx.UserMainCurrencyDetails.FirstOrDefaultAsync(x => x.ApplicationUserId == userId);
        }

        public async Task<bool> UpdateMainCurrency(UserMainCurrencyDetail updateModel)
        {
            _ctx.UserMainCurrencyDetails.Update(updateModel);
            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}
