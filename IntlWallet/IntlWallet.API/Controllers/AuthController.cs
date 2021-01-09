using IntlWallet.Data;
using IntlWallet.Data.Services;
using IntlWallet.DTOs;
using IntlWallet.Models;
using IntlWallet.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntlWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _ctx;
        private readonly IWalletRepository _walletRepository;

        public AuthController(UserManager<ApplicationUser> userManager, ILogger<AuthController> logger, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, AppDbContext ctx, IWalletRepository walletRepository)
        {
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
            _configuration = configuration;
            _ctx = ctx;
            _walletRepository = walletRepository;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserToSignUpDTO model)
        {
            ApplicationUser newUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.Email,
                Email = model.Email,
                LastName = model.LastName,
                FirstName = model.FirstName,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, model.Role);

                if(model.Role != "Admin")
                {
                    try
                    {
                        Wallet wallet = new Wallet
                        {
                            WalletId = Guid.NewGuid().ToString(),
                            ApplicationUserId = newUser.Id,
                            WalletCurrency = model.MainCurrency
                        };

                        await _walletRepository.AddWallet(wallet);
                    }
                    catch (Exception e)
                    {
                        await _userManager.DeleteAsync(newUser);
                        _logger.LogError(e.Message);
                        return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to add wallet" }));
                    }
                }
            }
            else
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = ModelState }));
            }

            return Ok(ResponseMessage.Message("Success! User created", data: new {newUser.Id }));
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(UserToSignInDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = ModelState }));

                var user = _userManager.Users.FirstOrDefault(x => x.Email == model.Email);

                if (user == null)
                {
                    return Unauthorized(ResponseMessage.Message("Unauthorized", errors: new { message = "Invalid credentials" }));
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                var userRoles = await _userManager.GetRolesAsync(user);
                if (result.Succeeded)
                {
                    LoginTokenDTO loginToken = new LoginTokenDTO();
                    loginToken.UserId = user.Id;
                    loginToken.Token = JwtTokenCreator.GetToken(user, _configuration, userRoles[0]);
                    return Ok(ResponseMessage.Message("Success", data: new { loginToken }));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Data processing error" }));
            }

            return Unauthorized(ResponseMessage.Message("Unauthorized", errors: new { message = "Invalid credentials" }));
        }
    }
}
