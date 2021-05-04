using System.Threading.Tasks;
using Archive.Core.Entities.Identity;
using Archive.WebUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Archive.WebUI.Pages.Account
{
    public class Login : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public Login(ILogger<LoginModel> logger,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }

        [BindProperty] 
        public LoginModel LoginModel { get; set; }

        public string ReturnUrl { get; set; }

        [TempData] public string ErrorMessage { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);


            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    LoginModel.UserName,
                    LoginModel.Password,
                    LoginModel.RememberMe,
                    true);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Вход в систему: {LoginModel.UserName}");
                    return LocalRedirect(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning($"Попытка входа: аккаунт {LoginModel.UserName} временно заблокирован");
                    return RedirectToPage("./Lockout");
                }

                ModelState.AddModelError(string.Empty, "Неверный логин/пароль");
                return Page();
            }

            return Page();
        }
    }
}