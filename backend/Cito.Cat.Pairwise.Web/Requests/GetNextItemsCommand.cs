using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cito.Cat.Core.Helpers;
using Cito.Cat.Pairwise.Web.Models;
using Cito.Cat.Service.Handlers;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Pairwise.Web.Requests
{
    public class GetNextItemsCommand : Command<NextItemsResponse>
    {
        public string TestSessionId { get; set; }
        public string SelectedItemIdentifier { get; set; }
        public long Duration { get; set; }
        private readonly NextItemsHandler _nextItemsHandler;
        private readonly IAsyncDocumentSession _documentSession;

        public GetNextItemsCommand(NextItemsHandler nextItemsHandler, IAsyncDocumentSession documentSession)
        {
            _nextItemsHandler = nextItemsHandler;
            _documentSession = documentSession;
        }

        protected override async Task<NextItemsResponse> DoExecute()
        {
            var testSession = await _documentSession.Include<TestSession>(s => s.TestSectionIdentifier)
                .LoadAsync<TestSession>(TestSessionId);
            if (testSession == null || testSession.Status == TestStatus.Finished)
            {
                throw new DomainException("Unknown start code.", false);
            }

            // get the pairwise section
            var section = await _documentSession.LoadAsync<TestSection>(testSession.TestSectionIdentifier);

            var response = await _nextItemsHandler.Get(
                testSession.GetNextItemsRequest(SelectedItemIdentifier, Duration), testSession.CatSectionIdentifier);


            testSession.ComparisonsDone++;
            testSession.SessionState = response.SessionState;
            testSession.NextItems = response.NextItems.ItemIdentifiers
                .Select(i => new KeyValuePair<string, Item>(i, section.Items[i]))
                .ToDictionary(x => x.Key, x => x.Value);

            if (response.StopConditionIsMet)
            {
                testSession.Status = TestStatus.Finished;
            }

            await _documentSession.StoreAsync(testSession);

            return new NextItemsResponse
            {
                NextItems = testSession.NextItems,
                ComparisonsTotal = testSession.ComparisonsTotal,
                ComparisonsDone = testSession.ComparisonsDone
            };
        }
    }
}