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

        public UserController(UserManager<ApplicationUser> userManager, IUserMainCurrencyRepository userMainCurrencyRepository, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _userMainCurrencyRepository = userMainCurrencyRepository;
            _logger = logger;
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

            return Ok(ResponseMessage.Message("Success", data: new { message = "Updated Successfully!" }));
        }
    }
}
