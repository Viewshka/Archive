using System;
using System.Linq;
using System.Threading.Tasks;
using Archive.WebUI.Models;
using Archive.Core.Entities.Identity;
using Archive.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Archive.WebUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = registerModel.UserName,
                UserName = registerModel.UserName,
                NormalizedUserName = registerModel.UserName.ToUpper(),
                EmailConfirmed = true,
                FirstName = registerModel.FirstName,
                MiddleName = registerModel.MiddleName,
                LastName = registerModel.LastName,
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Сотрудник");
                await _signInManager.SignInAsync(user, false);
                return Ok("Регистрация прошла успешно!");
            }

            return BadRequest(result.Errors);
        }
    }
}