using IntlWallet.Data.Services;
using IntlWallet.DTOs;
using IntlWallet.Models;
using IntlWallet.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntlWallet.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserMainCurrencyRepository _userMainCurrencyRepository;
        private readonly ILogger<UserController> _logger;
        private readonly IWalletRepository _walletRepository;

        public UserController(UserManager<ApplicationUser> userManager, IUserMainCurrencyRepository userMainCurrencyRepository, ILogger<UserController> logger, IWalletRepository walletRepository)
        {
            _userManager = userManager;
            _userMainCurrencyRepository = userMainCurrencyRepository;
            _logger = logger;
            _walletRepository = walletRepository;
        }

        [HttpPatch("{Id}/promote")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Promote(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Invalid Id" }));

            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
                return NotFound(ResponseMessage.Message("Not found", new { message = $"User with id {Id} was not found" }));

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles[0] == "Elite")
                return BadRequest(ResponseMessage.Message("Bad request", new { message = "User is Elite and cannot be promoted" }));


            await _userManager.RemoveFromRoleAsync(user, userRoles[0]);
            await _userManager.AddToRoleAsync(user, "Elite");

            return Ok(ResponseMessage.Message("Success! User promoted"));
        }

        [HttpPatch("{Id}/demote")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Demote(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Invalid Id" }));

            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
                return NotFound(ResponseMessage.Message("Not found", new { message = $"User with id {Id} was not found" }));

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles[0] == "Noob")
                return BadRequest(ResponseMessage.Message("Bad request", new { message = "User is Noob and cannot be demoted" }));


            await _userManager.RemoveFromRoleAsync(user, userRoles[0]);
            await _userManager.AddToRoleAsync(user, "Noob");

            return Ok(ResponseMessage.Message("Success! User demoted"));
        }

        [HttpPatch("{Id}/change-main-currency")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeMainCurrency([FromBody] ChangeMainCurrencyDTO model, string Id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = ModelState }));

            if (string.IsNullOrWhiteSpace(Id))
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Invalid Id" }));

            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
                return NotFound(ResponseMessage.Message("Not found", new { message = $"User with id {Id} was not found" }));

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles[0] == "Admin")
                return BadRequest(ResponseMessage.Message("Bad request", new { message = "User is an Admin and has no main currency" }));

            var isValid = CurrencyConverter.ValidateCurrencyInput(model.NewMainCurrency);
            if(!isValid)
                return BadRequest(ResponseMessage.Message("Error", errors: new { message = "Invalid currency input" }));

            UserMainCurrencyDetail mainCurrencyDetails;
            try
            {
                mainCurrencyDetails = await _userMainCurrencyRepository.GetMainCurrencyByUserId(user.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source" }));
            }

            try
            {
                mainCurrencyDetails.MainCurrency = model.NewMainCurrency;
                await _userMainCurrencyRepository.UpdateMainCurrency(mainCurrencyDetails);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to update main currency" }));
            }

            if(userRoles[0] == "Noob")
            {
                var wallets = await _walletRepository.GetWalletsByUserId(user.Id);
                var wallet = wallets.ToList()[0];
                var oldCurrency = wallet.WalletCurrency;

                try
                {
                    wallet.WalletCurrency = model.NewMainCurrency;

                    decimal amount;
                    try
                    {
                        amount = await CurrencyConverter.ConvertCurrency(oldCurrency, model.NewMainCurrency, wallet.Balance);
                    }
                    catch (Exception e)
                    {
                        return BadRequest(ResponseMessage.Message("Error", errors: new { message = e.Message }));
                    }

                    wallet.Balance = amount;
                    await _walletRepository.UpdateWallet(wallet);
                }
                catch (Exception e)
                {
                    mainCurrencyDetails.MainCurrency = oldCurrency;
                    await _userMainCurrencyRepository.UpdateMainCurrency(mainCurrencyDetails);

                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to update wallet" }));
                }
            }

            return Ok(ResponseMessage.Message("Success", data: new { message = "Updated Successfully!" }));
        }
    }
}
