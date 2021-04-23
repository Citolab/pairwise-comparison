using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cito.Cat.Algorithms.Pairwise.Models;
using Cito.Cat.Core.Helpers;
using Cito.Cat.Core.Models.Section;
using Cito.Cat.Pairwise.Web.Models;
using Cito.Cat.Service.Handlers;
using Ims.Cat.Models;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Pairwise.Web.Requests
{
    public class GetTestSessionWithStartCodeCommand : Command<StartSessionResponse>
    {
        public string StartCode { get; set; }
        private readonly CatSessionHandler _catSessionHandler;
        private readonly IAsyncDocumentSession _documentSession;

        public GetTestSessionWithStartCodeCommand(CatSessionHandler catSessionHandler,
            IAsyncDocumentSession documentSession)
        {
            _catSessionHandler = catSessionHandler;
            _documentSession = documentSession;
        }

        protected override async Task<StartSessionResponse> DoExecute()
        {
            // validate start code and get corresponding test section
            var testSession = await _documentSession.Query<TestSession>()
                .Include(s => s.CatSectionIdentifier)
                .Include(s => s.TestSectionIdentifier)
                .FirstOrDefaultAsync(s => s.StartCode == StartCode && s.Status != TestStatus.Finished);
            if (testSession == null)
            {
                throw new DomainException("Unknown start code.", false);
            }

            // get the cat section
            var catSection = await _documentSession.LoadAsync<CatSection>(testSession.CatSectionIdentifier);
            var sectionConfig = catSection.SectionConfiguration.FromJson<SectionConfiguration>();
            // get the pairwise section
            var section = await _documentSession.LoadAsync<TestSection>(testSession.TestSectionIdentifier);

            // initialize test session
            // List<string> itemIdentifiers;
            // Dictionary<string, string> nextItems;
            // string sessionState;
            if (testSession.Status == TestStatus.NotStarted)
            {
                var response = await _catSessionHandler.Initialize(new SessionDType(), testSession.CatSectionIdentifier);
                testSession.CatSessionIdentifier = response.SessionIdentifier;
                testSession.NextItems = response.NextItems.ItemIdentifiers
                    .Select(i => new KeyValuePair<string, Item>(i, section.Items[i]))
                    .ToDictionary(x => x.Key, x => x.Value);
                testSession.SessionState = response.SessionState;
                testSession.Status = TestStatus.Started;
                testSession.ComparisonsTotal = sectionConfig.ComparisonsTotal;
                testSession.ComparisonsDone = 0;
                await _documentSession.StoreAsync(testSession);
            }

            return new StartSessionResponse
            {
                Name = testSession.Name,
                TestSessionIdentifier = testSession.Id
            };
        }
    }
}