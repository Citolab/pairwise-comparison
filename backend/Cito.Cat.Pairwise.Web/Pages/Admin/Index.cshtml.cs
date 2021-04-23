using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Cito.Cat.Pairwise.Web.Helpers;
using Cito.Cat.Pairwise.Web.Models;
using Cito.Cat.Pairwise.Web.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cito.Cat.Pairwise.Web.Pages.Admin
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        [Required(ErrorMessage = "De gebruikersnaam is verplicht.")]
        [BindProperty]
        public string Username { get; set; }

        [Required(ErrorMessage = "Het wachtwoord is verplicht.")]
        [DataType(DataType.Password)]
        [BindProperty]
        public string Password { get; set; }


        [BindProperty] public bool LoginFailure { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (User.GetUserRole() != UserRole.Admin)
            {
                await SignOut();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSignIn()
        {
            var command = new LoginCommand {Username = Username, Password = Password};
            var loginResult = await command.Execute();
            if (loginResult.Success)
            {
                await SignIn(loginResult);
                return RedirectToPage("./Index");
            }

            LoginFailure = true;
            return Page();
        }

        private async Task SignIn(LoginResult loginResult)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginResult.FullName),
                new Claim(ClaimTypes.NameIdentifier, loginResult.UserId),
                new Claim(ClaimTypes.Role, UserRole.Admin.ToString())
            };

            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        public async Task<IActionResult> OnGetSignOut()
        {
            await SignOut();
            return RedirectToPage("./Index");
        }

        private async Task SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}