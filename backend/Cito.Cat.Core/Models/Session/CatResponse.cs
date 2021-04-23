using System.Collections.Generic;
using Ims.Cat.Models;

namespace Cito.Cat.Core.Models.Session
{
    public class CatResponse
    {
        public double Estimation { get; set; }

        public double StandardError { get; set; }

        public double BankPercentage { get; set; }

        public string[] NextItemIds { get; set; }

        public int StageLength { get; set; }

        public bool StopConditionIsMet { get; set; }

        public string StopDescription { get; set; }

        public string NewSessionState { get; set; }

        public AssessmentResultDType AssessmentResult { get; set; }
        
        public void AddOutcome(string identifier, string value)
        {
            var outcomeVariable = new OutcomeVariableDType
            {
                Cardinality = OutcomeVariableDType.CardinalityEnum.SingleEnum,
                BaseType = OutcomeVariableDType.BaseTypeEnum.StringEnum,
                Identifier = identifier,
                Value = new List<ValueDType>
                {
                    new()
                    {
                        BaseType = ValueDType.BaseTypeEnum.StringEnum,
                        Value = value
                    }
                }
            };
            AssessmentResult?.TestResult?.OutcomeVariables?.Add(outcomeVariable);
        }
        
    }
}