using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cito.Cat.Algorithms.Pairwise.Models;
using Cito.Cat.Core.Helpers;
using Cito.Cat.Core.Models.Section;
using Cito.Cat.Pairwise.Web.Models;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Pairwise.Web.Requests
{
    public class AddStartCodesNotification : Notification
    {
        public string TestSectionId { get; set; }
        public int NumberOfStartCodes { get; set; }
        private readonly IAsyncDocumentSession _documentSession;

        public AddStartCodesNotification(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        protected override async Task DoExecute()
        {
            var section = await _documentSession.Query<TestSection>().Where(s => s.Id == TestSectionId)
                .Select(s => new {s.Title, s.CatSectionId}).FirstOrDefaultAsync();
            var catSection = await _documentSession.LoadAsync<CatSection>(section.CatSectionId);
            var sectionConfig = catSection.SectionConfiguration.FromJson<SectionConfiguration>();
            var existingStartCodes = await _documentSession.Query<TestSession>()
                .Where(s => s.Status != TestStatus.Finished).Select(s => s.StartCode).ToListAsync();

            var startCodes = GenerateStartCodes(NumberOfStartCodes, 5, existingStartCodes).ToArray();

            for (int i = 0; i < NumberOfStartCodes; i++)
            {
                await _documentSession.StoreAsync(new TestSession
                {
                    TestSectionIdentifier = TestSectionId,
                    CatSectionIdentifier = section.CatSectionId,
                    Name = "-",
                    Status = TestStatus.NotStarted,
                    ComparisonsDone = 0,
                    ComparisonsTotal = sectionConfig.ComparisonsTotal,
                    StartCode = startCodes[i]
                });
            }
        }

        /// <summary>
        ///  Generate start codes.
        /// </summary>
        /// <param name="numberOfStartCodes">Number of start codes to generate.</param>
        /// <param name="length">Number of characters per start code.</param>
        /// <param name="existingStartCodes"></param>
        /// <returns></returns>
        private static IEnumerable<string> GenerateStartCodes(int numberOfStartCodes, int length,
            ICollection<string> existingStartCodes)
        {
            if (length > 8)
            {
                throw new DomainException("Maximum start code length is 8", true);
            }

            var result = new List<string>();
            for (int i = 0; i < numberOfStartCodes; i++)
            {
                string newStartCode;
                do
                {
                    newStartCode = Path.GetRandomFileName().Replace(".", "")
                        .Substring(0, length)
                        .ToUpperInvariant();
                } while (result.Contains(newStartCode) && existingStartCodes.Contains(newStartCode));

                result.Add(newStartCode);
            }

            return result;
        }
    }
}