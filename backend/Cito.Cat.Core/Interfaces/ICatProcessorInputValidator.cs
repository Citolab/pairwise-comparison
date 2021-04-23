using System.Collections.Generic;
using Ims.Cat.Models;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;

namespace Cito.Cat.Core.Interfaces
{
    public interface ICatProcessorInputValidator
    {
        Either<Error, ISectionConfiguration> ParseSectionConfiguration(string sectionConfiguration, ILogger logger);

        List<string> ValidatePriorData(List<KeyValuePairDType> priorData, ILogger logger);

        List<string> ValidateNextItemsRequest(ResultsDType nextItemsRequest, ISectionConfiguration sectionConfiguration,
            ILogger logger);
    }
}