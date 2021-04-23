using System.Linq;
using Ims.Cat.Models;
using Ims.Schemas.qti_result_v2p1;
using LanguageExt;
using LanguageExt.Common;

namespace Cito.Cat.Core.Helpers
{
    public static class Qti2Helper
    {
        public static Either<Error, string> GetOutcomeValue(AssessmentResultType assessmentResult,
            string outcomeIdentifier)
        {
            var outcome = assessmentResult?.testResult?.Items?.OfType<OutcomeVariableType>()
                .FirstOrDefault(v => v.identifier == outcomeIdentifier);
            if (outcome == null)
            {
                return Error.New($"OutcomeVariable with id '{outcomeIdentifier}' could not be found.");
            }

            if (outcome.value != null && outcome.value.Any())
            {
                return outcome.value[0].Value ?? string.Empty;
            }

            return Error.New($"The {outcomeIdentifier} outcome doesn't contain any values.");
        }

        public static Either<Error, double> GetScoreOutcomeValue(AssessmentResultType assessmentResult)
        {
            var errorOrValue = GetOutcomeValue(assessmentResult, "SCORE");
            if (errorOrValue.IsError())
            {
                return (Error) errorOrValue;
            }

            var value = (string) errorOrValue;

            if (string.IsNullOrWhiteSpace(value))
            {
                return Error.New("The score value was empty.");
            }

            if (!double.TryParse(value, out var score))
            {
                return Error.New($"The score value '{value.Trim()}' could not be parsed as a double.");
            }

            return score;
        }
    }
}