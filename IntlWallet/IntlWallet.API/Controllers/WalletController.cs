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
    public class WalletController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<WalletController> _logger;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserMainCurrencyRepository _userMainCurrencyRepository;

        public WalletController(UserManager<ApplicationUser> userManager, IWalletRepository walletRepository, ILogger<WalletController> logger, ITransactionRepository transactionRepository, IUserMainCurrencyRepository userMainCurrencyRepository)
        {
            _userManager = userManager;
            _walletRepository = walletRepository;
            _logger = logger;
            _transactionRepository = transactionRepository;
            _userMainCurrencyRepository = userMainCurrencyRepository;
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

        [Authorize(Roles = "Admin")]
        [HttpPost("{Id}/fund-wallet")]
        public async Task<IActionResult> FundWallet([FromBody] FundWalletDTO model, string Id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = ModelState }));

            var wallet = await _walletRepository.GetWalletByWalletId(Id);
            if (wallet == null)
                return NotFound(ResponseMessage.Message("Not found", new { message = $"Wallet with id {Id} was not found" }));

            Transaction transaction = new Transaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                TransactionType = "Credit",
                WalletId = wallet.WalletId,
                Amount = model.Amount,
                TransactionCurrency = model.TransactionCurrency
            };

            try
            {
                await _transactionRepository.AddTransaction(transaction);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to add transaction" }));
            }

            try
            {
                var walletOwner = await _userManager.FindByIdAsync(wallet.ApplicationUserId);
                if (walletOwner == null)
                    return NotFound(ResponseMessage.Message("Not found", new { message = $"User with id {wallet.ApplicationUserId} was not found" }));

                UserMainCurrencyDetail mainCurrencyDetail;
                try
                {
                    mainCurrencyDetail = await _userMainCurrencyRepository.GetMainCurrencyByUserId(walletOwner.Id);
                }
                catch (Exception e)
                {
                    await _transactionRepository.DeleteTransaction(transaction);
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source" }));
                }
                
                var mainCurrency = mainCurrencyDetail.MainCurrency;

                var convertedAmount = await CurrencyConverter.ConvertCurrency(model.TransactionCurrency, mainCurrency, model.Amount);

                wallet.Balance += convertedAmount;
                await _walletRepository.UpdateWallet(wallet);
                return Ok(ResponseMessage.Message("Success! Wallet funded"));
            }
            catch (Exception e)
            {
                await _transactionRepository.DeleteTransaction(transaction);
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to fund wallet" }));
            }
        }

        [Authorize(Roles = "Noob, Elite")]
        [HttpPost("fund-wallet")]
        public async Task<IActionResult> FundWallet([FromBody] FundWalletDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = ModelState }));

            var loggedInUser = await _userManager.GetUserAsync(User);
            if (loggedInUser == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            var loggedInUserRoles = await _userManager.GetRolesAsync(loggedInUser);

            Transaction transaction = null;

            if (loggedInUserRoles[0] == "Noob")
            {
                var wallets = await _walletRepository.GetWalletsByUserId(loggedInUser.Id);
                var wallet = wallets.ToList()[0];

                transaction = new Transaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    TransactionType = "Credit",
                    WalletId = wallet.WalletId,
                    TransactionStatus = "pending",
                    Amount = model.Amount,
                    TransactionCurrency = model.TransactionCurrency
                };

                try
                {
                    await _transactionRepository.AddTransaction(transaction);
                    return Ok(ResponseMessage.Message($"Success! Transaction with id {transaction.TransactionId} created. Funding approval pending"));
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to add transaction" }));
                }
            }
            else
            {
                var wallets = await _walletRepository.GetWalletsByUserId(loggedInUser.Id);
                var wallet = wallets.FirstOrDefault(x => x.WalletCurrency == model.TransactionCurrency);

                if (wallet == null)
                {
                    try
                    {
                        Wallet newWallet = new Wallet
                        {
                            WalletId = Guid.NewGuid().ToString(),
                            ApplicationUserId = loggedInUser.Id,
                            WalletCurrency = model.TransactionCurrency
                        };

                        await _walletRepository.AddWallet(newWallet);

                        wallet = await _walletRepository.GetWalletByWalletId(newWallet.WalletId);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                        return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to add wallet" }));
                    }
                }

                transaction = new Transaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    TransactionType = "Credit",
                    WalletId = wallet.WalletId,
                    Amount = model.Amount,
                    TransactionCurrency = model.TransactionCurrency
                };

                try
                {
                    await _transactionRepository.AddTransaction(transaction);
                }
                catch (Exception e)
                {
                    await _walletRepository.DeleteWallet(wallet);
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to add transaction" }));
                }

                try
                {
                    wallet.Balance += model.Amount;
                    await _walletRepository.UpdateWallet(wallet);
                    return Ok(ResponseMessage.Message("Success! Wallet funded"));
                }
                catch (Exception e)
                {
                    await _transactionRepository.DeleteTransaction(transaction);
                    await _walletRepository.DeleteWallet(wallet);
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to fund wallet" }));
                }
            }
        }

        [Authorize(Roles = "Noob, Elite")]
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = ModelState }));

            var loggedInUser = await _userManager.GetUserAsync(User);
            if (loggedInUser == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            var loggedInUserRoles = await _userManager.GetRolesAsync(loggedInUser);

            Transaction transaction;

            if (loggedInUserRoles[0] == "Noob")
            {
                var wallets = await _walletRepository.GetWalletsByUserId(loggedInUser.Id);
                var wallet = wallets.ToList()[0];

                transaction = new Transaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    TransactionType = "Debit",
                    WalletId = wallet.WalletId,
                    TransactionStatus = "pending",
                    Amount = model.Amount,
                    TransactionCurrency = model.TransactionCurrency
                };

                try
                {
                    await _transactionRepository.AddTransaction(transaction);
                    return Ok(ResponseMessage.Message($"Success! Transaction with id {transaction.TransactionId} created. Withdrawal approval pending"));
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to add transaction" }));
                }
            }
            else
            {
                var wallets = await _walletRepository.GetWalletsByUserId(loggedInUser.Id);
                var wallet = wallets.FirstOrDefault(x => x.WalletCurrency == model.TransactionCurrency);

                string transactionCurrency = model.TransactionCurrency;
                if (wallet == null)
                {
                    try
                    {
                        var mainCurrencyDetail = await _userMainCurrencyRepository.GetMainCurrencyByUserId(loggedInUser.Id);
                        var mainCurrency = mainCurrencyDetail.MainCurrency;
                        wallet = await _walletRepository.GetWalletByWalletCurrency(mainCurrency);
                        transactionCurrency = mainCurrency;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                        return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source" }));
                    }
                }

                transaction = new Transaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    TransactionType = "Debit",
                    WalletId = wallet.WalletId,
                    Amount = model.Amount,
                    TransactionCurrency = model.TransactionCurrency
                };

                try
                {
                    await _transactionRepository.AddTransaction(transaction);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to add transaction" }));
                }

                try
                {
                    var amount = await CurrencyConverter.ConvertCurrency(model.TransactionCurrency, transactionCurrency, model.Amount);
                    if(wallet.Balance >= amount)
                    {
                        wallet.Balance -= amount;

                        await _walletRepository.UpdateWallet(wallet);
                        return Ok(ResponseMessage.Message("Success! Withdrawal successful"));
                    }
                    else
                    {
                        return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Insufficient funds" }));
                    }
                }
                catch (Exception e)
                {
                    await _transactionRepository.DeleteTransaction(transaction);
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to fund wallet" }));
                }
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetWallets(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
                return BadRequest(ResponseMessage.Message("Invalid input", errors: new { message = "Id should not be null or empty or whitespace" }));

            try
            {
                var wallets = await _walletRepository.GetWalletsByUserId(Id);
                List<WalletToReturnDTO> walletsToReturn = new List<WalletToReturnDTO>();

                foreach (var wallet in wallets)
                {
                    var walletToReturn = new WalletToReturnDTO
                    {
                        WalletId = wallet.WalletId,
                        ApplicationUserId = wallet.ApplicationUserId,
                        WalletCurrency = wallet.WalletCurrency,
                        Balance = wallet.Balance
                    };

                    walletsToReturn.Add(walletToReturn);
                }

                return Ok(ResponseMessage.Message("Success", data: walletsToReturn));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source" }));
            }
        }
    }
}
