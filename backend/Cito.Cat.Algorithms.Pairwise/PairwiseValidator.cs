using System;
using System.Collections.Generic;
using System.Linq;
using Cito.Cat.Algorithms.Pairwise.Models;
using Cito.Cat.Core.Helpers;
using Cito.Cat.Core.Interfaces;
using Ims.Cat.Models;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;

namespace Cito.Cat.Algorithms.Pairwise
{
    public class PairwiseValidator : ICatProcessorInputValidator
    {
        public Either<Error, ISectionConfiguration> ParseSectionConfiguration(string sectionConfiguration,
            ILogger logger)
        {
            var result = new List<string>();

            if (!sectionConfiguration.TryFromJson<SectionConfiguration>(out var config))
            {
                const string errorMessage = "The provided configuration was invalid.";
                logger?.LogError(errorMessage);
                result.Add(errorMessage);
            }
            else
            {
                if (config.ItemCount == 0)
                {
                    result.Add("Configuration contained no items.");
                }

                if (config.ItemCount != config.ThetaStart.Length)
                {
                    result.Add("The initial theta value list length is not equal to the number of items.");
                }

                if (config.ComparisonsPerItem <= 0)
                {
                    result.Add("Comparisons per item should be a positive number.");
                }

                if (config.SamplingIterations <= 0)
                {
                    result.Add("Sampling iterations should be a positive number.");
                }
            }

            if (result.Any())
            {
                return Error.New(result.Enumerate());
            }

            return config;
        }

        public List<string> ValidatePriorData(List<KeyValuePairDType> priorData, ILogger logger)
        {
            return new();
        }

        public List<string> ValidateNextItemsRequest(ResultsDType nextItemsRequest, ISectionConfiguration sectionConfiguration, ILogger logger)
        {
            return new();
        }
    }
}