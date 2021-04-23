using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Cito.Cat.Core.Helpers;
using Cito.Cat.Pairwise.Web.Models;
using Cito.Cat.Pairwise.Web.Requests;
using Cito.Cat.Service.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Pairwise.Web.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly CatSessionHandler _catSessionHandler;
        private readonly IAsyncDocumentSession _documentSession;

        [Required(ErrorMessage = "Please enter a start code.")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Minimum number of characters is 4 and the maximum is 10.")]
        [BindProperty]
        public string StartCode { get; set; }


        public IndexModel(ILogger<IndexModel> logger, CatSessionHandler catSessionHandler,
            IAsyncDocumentSession documentSession)
        {
            _logger = logger;
            _catSessionHandler = catSessionHandler;
            _documentSession = documentSession;
        }

        public async Task<IActionResult> OnGet(bool reset1234)
        {
            if (reset1234)
            {
                var session1234 = await _documentSession.Query<TestSession>()
                    .Where(s => s.StartCode == "1234").FirstOrDefaultAsync();
                if (session1234 != null)
                {
                    session1234.Status = TestStatus.NotStarted;
                    session1234.ComparisonsDone = 0;
                    await _documentSession.StoreAsync(session1234);
                }

                var session0000 = await _documentSession.Query<TestSession>()
                    .Where(s => s.StartCode == "0000").FirstOrDefaultAsync();
                if (session0000 != null)
                {
                    session0000.Status = TestStatus.NotStarted;
                    session0000.ComparisonsDone = 0;
                    await _documentSession.StoreAsync(session0000);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var command = new GetTestSessionWithStartCodeCommand(_catSessionHandler, _documentSession)
                {StartCode = StartCode};
            try
            {
                var result = await command.Execute();
                await SignIn(result);
                return RedirectToPage("./Compare");
            }
            catch (DomainException e)
            {
                ModelState.AddModelError("StartCode", string.Join(", ", e.Message));
            }

            return Page();
        }


        private async Task SignIn(StartSessionResponse startSessionResponse)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, startSessionResponse.Name),
                new Claim(ClaimTypes.NameIdentifier, startSessionResponse.TestSessionIdentifier),
                new Claim(ClaimTypes.Role, "Candidate")
            };

            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private async Task SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}