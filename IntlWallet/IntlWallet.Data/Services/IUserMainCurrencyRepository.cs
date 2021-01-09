using IntlWallet.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntlWallet.Data.Services
{
    public interface IUserMainCurrencyRepository
    {
        Task<UserMainCurrencyDetail> GetMainCurrencyByUserId(string userId);
        Task<bool> AddMainCurrency(UserMainCurrencyDetail model);
        Task<bool> UpdateMainCurrency(UserMainCurrencyDetail updateModel);
        Task<bool> DeleteMainCurrency(UserMainCurrencyDetail model);
    }
}
