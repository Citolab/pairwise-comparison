using System;
using System.Linq;
using System.Threading.Tasks;
using Cito.Cat.Core.Helpers;
using Cito.Cat.Core.Interfaces;
using Cito.Cat.Core.Models;
using Cito.Cat.Core.Models.Section;
using Cito.Cat.Core.Models.Session;
using Ims.Cat.Models;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Service.Handlers
{
    public class CatSessionHandler
    {
        private readonly IAsyncDocumentSession _asyncDocumentSession;
        private readonly ILoggerFactory _loggerFactory;
        private readonly CatOptions _catOptions;

        public CatSessionHandler(CatOptions catOptions, IAsyncDocumentSession asyncDocumentSession, ILoggerFactory loggerFactory)
        {
            _catOptions = catOptions;
            _asyncDocumentSession = asyncDocumentSession;
            _loggerFactory = loggerFactory;
        }

        private ValidationResult IsValidRequest(SessionDType request, string sectionIdentifier)
        {
            if (string.IsNullOrWhiteSpace(request.PersonalNeedsAndPreferences))
            {
                // it's an optional parameter
            }
            else
            {
                //TODO: validate against schema if supplied    
            }
            

            if (string.IsNullOrWhiteSpace(request.Demographics))
            {
                // it's an optional parameter
            }
            else
            {
                //TODO: validate against schema if supplied
            }

            // NOTE: The validity of prior data will be checked during the session initialization in the cat processor
            //                  because the validation depends on the initialization request.
            //var priorDataValidationResult = catProcessor.ValidatePriorData(request.PriorData);
            // if (priorDataValidationResult.Any())
            // {
            //     return new ValidationResult
            //     {
            //         ResultCode = ResultCodes.UnableToProcess,
            //         Description =
            //             $"The validation of PriorData resulted in errors: {string.Join(", ", priorDataValidationResult)}"
            //     };
            // }

            if (_asyncDocumentSession.LoadAsync<CatSection>(sectionIdentifier).Result == null)
            {
                return new ValidationResult
                {
                    ResultCode = ResultCodes.NotFound,
                    Description = "No section with that sectionIdentifier"
                };
            }

            return new ValidationResult
            {
                ResultCode = ResultCodes.Ok
            };
        }

        public async Task<CreateSessionResponseBodyDType> Initialize(SessionDType request, string sectionIdentifier,
            string sessionIdentifier = null)
        {
            request.PadBase64Strings();

            var validationResult = IsValidRequest(request, sectionIdentifier);
            if (validationResult.ResultCode != ResultCodes.Ok)
            {
                throw new DomainException($"{validationResult.ResultCode}: {validationResult.Description}", true);
            }

            sessionIdentifier ??= Guid.NewGuid().ToShortGuidString();

            var catSection = await _asyncDocumentSession.LoadAsync<CatSection>(sectionIdentifier);
            
            var catProcessor = CatHelper.GetProcessor(_catOptions, catSection.SectionConfiguration, _loggerFactory, _asyncDocumentSession);
            var catResponse = await catProcessor.InitializeSession(request, sessionIdentifier);

            var response = new CreateSessionResponseBodyDType
            {
                SessionIdentifier = sessionIdentifier,
                NextItems = new NextItemSetDType
                    {ItemIdentifiers = catResponse.NextItemIds.ToList(), StageLength = catResponse.StageLength},
                SessionState = catResponse.NewSessionState
            };
            return response;
        }

        public void Remove(string id)
        {
            // nothing to remove (yet)
        }
    }
}