using System;
using System.Collections.Generic;
using System.Linq;
using Ims.Cat.Models;

namespace Cito.Cat.Pairwise.Web.Models
{
    public class TestSession
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string StartCode { get; set; }
        public string TestSectionIdentifier { get; set; }
        public string CatSectionIdentifier { get; set; }
        public string CatSessionIdentifier { get; set; }
        public TestStatus Status { get; set; }
        public string SessionState { get; set; }
        public Dictionary<string, Item> NextItems { get; set; }
        public int ComparisonsTotal { get; set; }
        public int ComparisonsDone { get; set; }

        public ResultsDType GetNextItemsRequest(string itemWin, long duration)
        {
            var nextItemsRequest = new ResultsDType
            {
                SessionState = SessionState,
                AssessmentResult = new AssessmentResultDType
                {
                    ItemResult = NextItems.Select(i => new ItemResultDType
                    {
                        Identifier = i.Key,
                        Datestamp = DateTime.Now,
                        SessionStatus = ItemResultDType.SessionStatusEnum.FinalEnum,
                        OutcomeVariables = new List<OutcomeVariableDType>
                        {
                            new OutcomeVariableDType
                            {
                                Identifier = "SCORE",
                                Cardinality = OutcomeVariableDType.CardinalityEnum.SingleEnum,
                                Value = new List<ValueDType>
                                {
                                    new ValueDType {Value = i.Key == itemWin ? "1" : "0"}
                                }
                            },
                            new OutcomeVariableDType
                            {
                                Identifier = "DURATION",
                                Cardinality = OutcomeVariableDType.CardinalityEnum.SingleEnum,
                                BaseType = OutcomeVariableDType.BaseTypeEnum.DurationEnum,
                                Value = new List<ValueDType>
                                {
                                    new ValueDType {Value = duration.ToString()}
                                }
                            }
                        }
                    }).ToList()
                }
            };
            return nextItemsRequest;
        }
    }

    public enum TestStatus
    {
        NotStarted = 0,
        Started = 1,
        Resumed = 2,
        Finished = 3,
        PostProcessed = 4
    }
}