using System.Linq;
using System.Threading.Tasks;
using Cito.Cat.Core.Helpers;
using Cito.Cat.Core.Interfaces;
using Cito.Cat.Core.Models;
using Cito.Cat.Core.Models.Section;
using Ims.Cat.Models;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Service.Handlers
{
    public class NextItemsHandler
    {
        private readonly CatOptions _catOptions;
        private readonly IAsyncDocumentSession _asyncDocumentSession;
        private readonly ILoggerFactory _loggerFactory;

        public NextItemsHandler(CatOptions catOptions, IAsyncDocumentSession asyncDocumentSession,
            ILoggerFactory loggerFactory)
        {
            _catOptions = catOptions;
            _asyncDocumentSession = asyncDocumentSession;
            _loggerFactory = loggerFactory;
        }

        public async Task<SubmitResultsResponseBodyDType> Get(ResultsDType request, string sectionIdentifier)
        {
            var catSection = await _asyncDocumentSession.LoadAsync<CatSection>(sectionIdentifier);
            var catProcessor = CatHelper.GetProcessor(_catOptions, catSection.SectionConfiguration, _loggerFactory,
                _asyncDocumentSession);

            var validationResult = IsValidRequest(request, catProcessor);
            if (validationResult.ResultCode != ResultCodes.Ok)
            {
                throw new DomainException($"{validationResult.ResultCode}: {validationResult.Description}", true);
            }

            var catResponse = await catProcessor.GetNextItems(request);

            var response = new SubmitResultsResponseBodyDType
            {
                NextItems = new NextItemSetDType
                    {ItemIdentifiers = catResponse.NextItemIds.ToList(), StageLength = catResponse.StageLength},
                EstimatedAbility = catResponse.Estimation,
                StandardError = catResponse.StandardError,
                BankPercentage = catResponse.BankPercentage,
                StopConditionIsMet = catResponse.StopConditionIsMet,
                StopDescription = catResponse.StopDescription,
                SessionState = catResponse.NewSessionState,
                AssessmentResult = catResponse.AssessmentResult
            };

            return response;
        }

        private static ValidationResult IsValidRequest(ResultsDType request, ICatProcessor catProcessor)
        {
            var validationResult = catProcessor.ValidateNextItemsRequest(request);
            if (validationResult.Any())
            {
                return new ValidationResult
                {
                    ResultCode = ResultCodes.UnableToProcess,
                    Description =
                        $"The validation of the NextItemsRequest resulted in errors: {string.Join(", ", validationResult)}"
                };
            }

            return new ValidationResult
            {
                ResultCode = ResultCodes.Ok
            };
        }
    }
}