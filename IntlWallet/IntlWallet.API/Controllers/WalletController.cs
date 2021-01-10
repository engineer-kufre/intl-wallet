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
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<WalletController> _logger;

        public WalletController(UserManager<ApplicationUser> userManager, IWalletRepository walletRepository, ILogger<WalletController> logger)
        {
            _userManager = userManager;
            _walletRepository = walletRepository;
            _logger = logger;
        }

        [Authorize(Roles = "Elite")]
        [HttpPost("add-wallet")]
        public async Task<IActionResult> AddWallet([FromBody] WalletDTO model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            try
            {
                Wallet wallet = new Wallet
                {
                    WalletId = Guid.NewGuid().ToString(),
                    ApplicationUserId = user.Id,
                    WalletCurrency = model.WalletCurrency
                };

                await _walletRepository.AddWallet(wallet);
                return Ok(ResponseMessage.Message("Success", data: new { id = wallet.WalletId }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to add wallet" }));
            }
        }
    }
}
