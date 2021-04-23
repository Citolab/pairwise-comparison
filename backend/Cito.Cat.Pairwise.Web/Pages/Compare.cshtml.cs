using System.Collections.Generic;
using System.Threading.Tasks;
using Cito.Cat.Core.Helpers;
using Cito.Cat.Pairwise.Web.Helpers;
using Cito.Cat.Pairwise.Web.Models;
using Cito.Cat.Pairwise.Web.Requests;
using Cito.Cat.Service.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Pairwise.Web.Pages
{
    [Authorize]
    public class CompareModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly CatSessionHandler _catSessionHandler;
        private readonly NextItemsHandler _nextItemsHandler;
        private readonly IAsyncDocumentSession _documentSession;

        [BindProperty] public string SelectedItemIdentifier { get; set; }
        [BindProperty] public long Duration { get; set; }
        [BindProperty] public Dictionary<string, Item> Items { get; set; }
        [BindProperty] public int ComparisonsTotal { get; set; }
        [BindProperty] public int ComparisonsDone { get; set; }

        public CompareModel(ILogger<IndexModel> logger, CatSessionHandler catSessionHandler,
            IAsyncDocumentSession documentSession, NextItemsHandler nextItemsHandler)
        {
            _logger = logger;
            _catSessionHandler = catSessionHandler;
            _documentSession = documentSession;
            _nextItemsHandler = nextItemsHandler;
        }

        public async Task<IActionResult> OnGet()
        {
            if (User.GetUserRole() == UserRole.Admin)
            {
                await SignOut();
                return RedirectToPage("./Index");
            }
            var loggedInTestSessionId = User.GetTestSessionId();
            var testSession = await _documentSession.LoadAsync<TestSession>(loggedInTestSessionId);
            Items = testSession.NextItems;
            ComparisonsTotal = testSession.ComparisonsTotal;
            ComparisonsDone = testSession.ComparisonsDone;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var loggedInTestSessionId = User.GetTestSessionId();

            try
            {
                var command = new GetNextItemsCommand(_nextItemsHandler, _documentSession)
                {
                    TestSessionId = loggedInTestSessionId,
                    SelectedItemIdentifier = SelectedItemIdentifier,
                    Duration = Duration
                };
                var response = await command.Execute();
                Items = response.NextItems;
                ComparisonsDone = response.ComparisonsDone;
                ComparisonsTotal = response.ComparisonsTotal;
            }
            catch (DomainException)
            {
                return RedirectToPage("./Index");
            }


            return Page();
        }

        public async Task<IActionResult> OnGetExit()
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