using IntlWallet.Data.Services;
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
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<WalletController> _logger;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserMainCurrencyRepository _userMainCurrencyRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransactionController(ITransactionRepository transactionRepository, ILogger<WalletController> logger, IWalletRepository walletRepository, IUserMainCurrencyRepository userMainCurrencyRepository, UserManager<ApplicationUser> userManager)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
            _walletRepository = walletRepository;
            _userMainCurrencyRepository = userMainCurrencyRepository;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{status}")]
        public async Task<IActionResult> GetTransactions(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest(ResponseMessage.Message("Invalid input", errors: new { message = "Status should not be null or empty or whitespace" }));

            if (status != "pending" || status != "approved")
                return BadRequest(ResponseMessage.Message("Invalid input", errors: new { message = "Status should be either pending or approved" }));

            try
            {
                var transactions = await _transactionRepository.GetTransactionsByStatus(status);
                return Ok(ResponseMessage.Message("Success", data: transactions));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source" }));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{Id}/approve")]
        public async Task<IActionResult> ApproveTransaction(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
                return BadRequest(ResponseMessage.Message("Invalid input", errors: new { message = "Id should not be null or empty or whitespace" }));

            Transaction transaction;
            try
            {
                transaction = await _transactionRepository.GetTransactionByTransactionId(Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source" }));
            }

            if (transaction.TransactionStatus == "approved")
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Transaction is already approved" }));

            Wallet wallet;
            UserMainCurrencyDetail mainCurrencyDetail;
            try
            {
                wallet = await _walletRepository.GetWalletByWalletId(transaction.WalletId);
                mainCurrencyDetail = await _userMainCurrencyRepository.GetMainCurrencyByUserId(wallet.ApplicationUserId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source" }));
            }

            try
            {
                var mainCurrency = mainCurrencyDetail.MainCurrency;

                var convertedAmount = await CurrencyConverter.ConvertCurrency(transaction.TransactionCurrency, mainCurrency, transaction.Amount);

                if(transaction.TransactionType == "Credit")
                {
                    wallet.Balance += convertedAmount;
                }
                else
                {
                    if (wallet.Balance >= convertedAmount)
                    {
                        wallet.Balance -= convertedAmount;
                    }
                    else
                    {
                        return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Insufficient funds" }));
                    }
                }
                await _walletRepository.UpdateWallet(wallet);

                try
                {
                    transaction.TransactionStatus = "approved";
                    await _transactionRepository.UpdateTransaction(transaction);
                }
                catch (Exception)
                {
                    if (transaction.TransactionType == "Credit")
                    {
                        wallet.Balance -= convertedAmount;
                    }
                    else
                    {
                        wallet.Balance += convertedAmount;
                    }

                    await _walletRepository.UpdateWallet(wallet);

                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to approve transaction" }));
                }
            }
            catch (Exception e)
            {
                await _transactionRepository.DeleteTransaction(transaction);
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to fund wallet" }));
            }

            return Ok(ResponseMessage.Message("Success! Transaction approved"));
        }
    }
}
