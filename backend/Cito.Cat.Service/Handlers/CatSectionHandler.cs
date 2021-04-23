using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Cito.Cat.Core.Helpers;
using Cito.Cat.Core.Interfaces;
using Cito.Cat.Core.Models;
using Cito.Cat.Core.Models.Section;
using Ims.Cat.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Service.Handlers
{
    public class CatSectionHandler
    {
        private readonly IAsyncDocumentSession _asyncDocumentSession;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IGoodTime _dateTimeProvider;
        private readonly CatOptions _catOptions;

        public CatSectionHandler(IGoodTime dateTimeProvider, CatOptions catOptions,
            IAsyncDocumentSession asyncDocumentSession, ILoggerFactory loggerFactory)
        {
            _dateTimeProvider = dateTimeProvider;
            _catOptions = catOptions;
            _asyncDocumentSession = asyncDocumentSession;
            _loggerFactory = loggerFactory;
        }

        public async Task<CreateSectionResponseBodyDType> Initialize(SectionDType request)
        {
            request.PadBase64Strings();

            var validationResult = IsValidRequest(request);
            if (validationResult.ResultCode != ResultCodes.Ok)
            {
                throw new DomainException($"{validationResult.ResultCode}: {validationResult.Description}", true);
            }

            // calculate the hash of the request and see if we've got the section already
            var hash = request.GetMD5Hash();

            var catSectionId = _asyncDocumentSession.Query<CatSection>()
                .FirstOrDefaultAsync(catSection => catSection.Hash == hash)
                .Result?.Id;

            if (string.IsNullOrWhiteSpace(catSectionId))
            {
                var catSection = new CatSection
                {
                    Id = Guid.Parse(hash).ToString(),
                    QtiMetadata = request.QtiMetadata,
                    QtiUsagedata = request.QtiUsagedata,
                    SectionConfiguration = request.SectionConfiguration,
                    Hash = hash
                };

                if (!string.IsNullOrWhiteSpace(request.SectionConfiguration))
                {
                    catSection.SectionConfiguration = request.SectionConfiguration.Base64Decode();
                }

                if (!string.IsNullOrWhiteSpace(catSection.QtiUsagedata))
                {
                    catSection.QtiUsagedata = catSection.QtiUsagedata.Base64Decode();
                }

                await _asyncDocumentSession.StoreAsync(catSection);
                catSectionId = catSection.Id;
            }

            var response = new CreateSectionResponseBodyDType
            {
                SectionIdentifier = catSectionId,
                //ValidationResult = new ValidationResult {ResultCode = ResultCodes.Ok}
            };
            return response;
        }

        public async Task<GetSectionResponseBodyDType> Get(string id)
        {
            var catSection = await _asyncDocumentSession.LoadAsync<CatSection>(id);

            var catProcessor = CatHelper.GetProcessor(_catOptions, catSection.SectionConfiguration, _loggerFactory,
                _asyncDocumentSession);

            var itemIds = catProcessor.GetAllItemIds();

            return new GetSectionResponseBodyDType
            {
                Items = new ItemSetDType {ItemIdentifiers = itemIds},
                Section = new SectionDType
                {
                    QtiMetadata = catSection.QtiMetadata,
                    QtiUsagedata = catSection.QtiUsagedata?.Base64Encode(),
                    SectionConfiguration = catSection.SectionConfiguration.Base64Encode()
                }
            };
        }

        public int Delete(Guid id)
        {
            try
            {
                _asyncDocumentSession.Delete(id.ToString());
                return ResultCodes.Ok;
            }
            catch (InvalidDataException)
            {
                return ResultCodes.NotFound;
            }
            catch (Exception)
            {
                return ResultCodes.Error;
            }
        }

        private ValidationResult IsValidRequest(SectionDType request)
        {
            // NOTE: for request.QtiMetadata: The validity of metadata was checked when the session init request was deserialized

            if (!CanParseUsagedata(request))
            {
                return new ValidationResult
                {
                    ResultCode = ResultCodes.UnableToProcess,
                    Description = "Invalid parameter value for qtiUsagedata"
                };
            }

            var validationResult = ValidateSectionConfiguration(request);
            if (validationResult.Any())
            {
                return new ValidationResult
                {
                    ResultCode = ResultCodes.UnableToProcess,
                    Description =
                        $"The validation of the SectionConfiguration resulted in errors: {string.Join(", ", validationResult)}"
                };
            }

            return new ValidationResult
            {
                ResultCode = ResultCodes.Ok
            };
        }

        private bool CanParseUsagedata(SectionDType request)
        {
            if (string.IsNullOrEmpty(request.QtiUsagedata))
            {
                return true; // assuming it is not a required field
            }

            if (!request.QtiUsagedata.TryBase64Decode(out var usageXml))
            {
                return false;
            }

            var serializer = new GenericXmlSerializer<UsageDataType>();
            var textReader = new StringReader(usageXml);
            using var reader = XmlReader.Create(textReader);
            var result = serializer.CanDeserialize(reader);

            return result;
        }

        private List<string> ValidateSectionConfiguration(SectionDType request)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(request.SectionConfiguration))
            {
                result.Add("The section configuration was an empty string or null.");
                return result;
            }

            if (!request.SectionConfiguration.TryBase64Decode(out var json))
            {
                result.Add("Section configuration could not be base64 decoded.");
                return result;
            }

            try
            {
                var catProcessor = CatHelper.GetProcessor(_catOptions, json, _loggerFactory, _asyncDocumentSession);
            }
            catch (Exception e)
            {
                result.Add(e.Message);
            }

            return result;
        }
    }
}