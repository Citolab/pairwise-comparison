using System.Collections.Generic;
using System.Threading.Tasks;
using Cito.Cat.Core.Models.Session;
using Ims.Cat.Models;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Core.Interfaces
{
    public abstract class CatProcessor : ICatProcessor
    {
        public string Description { get; protected set; }
        protected ISectionConfiguration SectionConfiguration;
        protected readonly IAsyncDocumentSession DocumentSession;
        protected ILogger Logger;

        //protected CatProcessorStatus Status;
        public List<string> ValidationErrors = new();

        protected CatProcessor(string sectionConfiguration, ILoggerFactory loggerFactory, IAsyncDocumentSession documentSession)
        {
            DocumentSession = documentSession;
            Description = "Base class for CAT processors.";
        }

        public abstract Task<CatResponse> InitializeSession(SessionDType sessionInitRequest, string sessionIdentifier);
        public abstract Task<CatResponse> GetNextItems(ResultsDType nextItemsRequest);
        public abstract List<string> GetAllItemIds();
        public abstract List<string> ValidatePriorData(List<KeyValuePairDType> priorData);
        public abstract List<string> ValidateNextItemsRequest(ResultsDType nextItemsRequest);
    }
}