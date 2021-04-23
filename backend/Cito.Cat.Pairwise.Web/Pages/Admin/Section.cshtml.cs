using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cito.Cat.Algorithms.Pairwise.Models;
using Cito.Cat.Core.Helpers;
using Cito.Cat.Core.Models.Section;
using Cito.Cat.Pairwise.Web.Models;
using Cito.Cat.Pairwise.Web.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Pairwise.Web.Pages.Admin
{
    public class SectionModel : PageModel
    {
        private readonly IAsyncDocumentSession _documentSession;
        [BindProperty] public TestSectionModel TestSectionModel { get; set; }
        [BindProperty] public IList<TestSessionModel> TestSessions { get; set; }
        [Range(1, 25)] [BindProperty] public int NumberOfStartCodes { get; set; }

        public SectionModel(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<IActionResult> OnGet(string id)
        {
            //var section = await _documentSession.Include<TestSection>(s => s.CatSectionId).LoadAsync<TestSection>(id);
            var section = await _documentSession.Query<TestSection>().Where(s => s.Id == id)
                .Select(s => new {s.Title, s.CatSectionId}).FirstOrDefaultAsync();
            var catSection = await _documentSession.LoadAsync<CatSection>(section.CatSectionId);
            var sectionConfig = catSection.SectionConfiguration.FromJson<SectionConfiguration>();
            TestSectionModel = new TestSectionModel
            {
                Id = id,
                Title = section.Title,
                ItemCount = sectionConfig.ItemCount
            };
            TestSessions = await _documentSession.Query<TestSession>().Where(s => s.TestSectionIdentifier == id)
                .Select(s => new TestSessionModel
                {
                    Id = s.Id, Name = s.Name, Status = s.Status, ComparisonsDone = s.ComparisonsDone,
                    ComparisonsTotal = s.ComparisonsTotal, StartCode = s.StartCode
                })
                .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPost(string id)
        {
            if (!ModelState.IsValid)
            {
                return await OnGet(id);
            }
            var notification = new AddStartCodesNotification(_documentSession)
            {
                TestSectionId = id,
                NumberOfStartCodes = NumberOfStartCodes
            };
            await notification.Execute();

            return RedirectToPage("./Section", new {id});
        }

        public Task<RedirectToPageResult> OnGetResultsBySection(string id)
        {
            return Task.FromResult(RedirectToPage("./Section", new {id}));
        }
    }
}