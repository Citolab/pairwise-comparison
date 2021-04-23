using System.Collections.Generic;
using System.Threading.Tasks;
using Cito.Cat.Pairwise.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Pairwise.Web.Pages.Admin
{
    public class SectionsModel : PageModel
    {
        private readonly IAsyncDocumentSession _documentSession;

        public SectionsModel(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        [BindProperty] public IList<TestSection> Sections { get; set; }

        public async Task OnGet()
        {
            Sections = await _documentSession.Query<TestSection>().ToListAsync();
        }
    }
}