using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cito.Cat.Algorithms.Pairwise.Models;
using Cito.Cat.Core.Helpers;
using Ims.Cat.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Cito.Cat.Algorithms.Pairwise.Tests
{
    [ExcludeFromCodeCoverage]
    public class CatProcessorTests
    {
        private ILogger<CatProcessorTests> _logger;
        [SetUp]
        public void SetUp()
        {
            _logger = NullLogger<CatProcessorTests>.Instance;
        }

        [Test]
        public void ValidateSectionConfiguration_NegativeSamplingIterations_Fail()
        {
            // arrange
            var sectionConfig = new SectionConfiguration
            {
                SamplingIterations = -1,
                ComparisonsPerItem = 1,
                ItemIds = new[] {"1", "2"},
                ThetaStart = new double[] {0, 0}
            };

            // act
            var result = PairwiseCatProcessor.Validator.ParseSectionConfiguration(sectionConfig.ToJson(), _logger);

            // assert
            Assert.True(result.IsError());
        }

        [Test]
        public void ValidateSectionConfiguration_NegativeComparisonsPerItem_Fail()
        {
            // arrange
            var sectionConfig = new SectionConfiguration
            {
                SamplingIterations = 1,
                ComparisonsPerItem = -1,
                ItemIds = new[] {"1", "2"},
                ThetaStart = new double[] {0, 0}
            };
            // act
            var result = PairwiseCatProcessor.Validator.ParseSectionConfiguration(sectionConfig.ToJson(), _logger);
            // assert
            Assert.True(result.IsError());
        }

        [Test]
        public void ValidateSectionConfiguration_MoreItemsThanThetas_Fail()
        {
            // arrange
            var sectionConfig = new SectionConfiguration
            {
                SamplingIterations = 1,
                ComparisonsPerItem = -1,
                ItemIds = new[] {"1", "2", "3"},
                ThetaStart = new double[] {0, 0}
            };
            // act
            var result = PairwiseCatProcessor.Validator.ParseSectionConfiguration(sectionConfig.ToJson(), _logger);
            // assert
            Assert.True(result.IsError());
        }

        [Test]
        public void ValidateSectionConfiguration_Correct_Success()
        {
            // arrange
            var sectionConfig = new SectionConfiguration
            {
                SamplingIterations = 1,
                ComparisonsPerItem = 1,
                ItemIds = new[] {"1", "2", "3"},
                ThetaStart = new double[] {0, 0, 0}
            };
            //var base64 = sectionConfig.ToJson().Base64Encode();
            // act
            var result = PairwiseCatProcessor.Validator.ParseSectionConfiguration(sectionConfig.ToJson(), _logger);
            // assert
            Assert.True(result.IsRight);
        }

        [Test]
        public void SessionState_Serialize_Success()
        {
            // arrange
            var state = new PairwiseSessionState
            {
                PairsData = new List<PairsData> {new PairsData {Player1 = 0, Player2 = 1, Win1 = 1, Win2 = 0}},
                PosteriorEstimates = new[] {new double[] {1, 2}, new double[] {3, 4}},
                CatSessionIdentifier = "derp",
                ThetaLast = new double[] {0, 0}
            };
            // act & assert
            Assert.DoesNotThrow(() => state.ToJson());
        }

        [Test]
        public void InitializeSession_ValidSection_Success()
        {
            // arrange
            var thetaStart = new double[] {0, 0, 0, 0, 0, 0};
            var sectionConfig = new SectionConfiguration
            {
                SamplingIterations = 1,
                ComparisonsPerItem = 1,
                // total number of comparisons:  (int) Math.Round((_sectionConfig.ItemCount * _sectionConfig.ComparisonsPerItem) / 2.0),
                ComparisonsTotal = (int) Math.Round((6 * 1) / 2.0),
                ItemIds = new[] {"A2+_item01", "A2+_item02", "A2+_item03", "B2_item01", "B2_item02", "B2_item03"},
                ThetaStart = thetaStart
            };
            //var sectionConfigBase64 = sectionConfig.ToJson().Base64Encode();
            var sessionIdentifier = "66A5D7E1-608B-4272-9EB6-B9D381835F74";
            var sessionInitRequest = new SessionDType
            {
                //SectionIdentifier = "67773552-5557-4CE6-AC3C-8657AEDAF33A",
                //SessionIdentifier = sessionIdentifier,
                PriorData = new List<KeyValuePairDType>
                {
                    new() {Key = "thetaStart", Value = thetaStart.ToJson()},
                    new() {Key = "priorMu", Value = 0.ToJson()},
                    new() {Key = "priorSigma", Value = 1.ToJson()}
                }
            };

            // act
            var processor = new PairwiseCatProcessor( sectionConfig.ToJson());
            var catResponse = processor.InitializeSession(sessionInitRequest, sessionIdentifier)
                .Result;

            // assert
            Assert.That(catResponse.StageLength == 2);
            Assert.That(catResponse.NextItemIds.Length == 2);
            //Assert.That(catResponse.NewSessionState ==
            //            "ew0KICAic2Vzc2lvbklkZW50aWZpZXIiOiAiNjZBNUQ3RTEtNjA4Qi00MjcyLTlFQjYtQjlEMzgxODM1Rjc0IiwNCiAgImNvbXBhcmlzb25Ub3RhbCI6IDIsDQogICJwYWlyc0RhdGEiOiBbXSwNCiAgInRoZXRhTGFzdCI6IFsNCiAgICAwLA0KICAgIDAsDQogICAgMA0KICBdLA0KICAicG9zdGVyaW9yRXN0aW1hdGVzIjogbnVsbA0KfQ==");
            //var json = catResponse.NewSessionState.Base64Decode();
            Assert.IsTrue(catResponse.NewSessionState.Base64Decode()
                .TryFromJson<PairwiseSessionState>(out var sessionState));
            Assert.IsNotNull(sessionState);
            Assert.That(sessionState.CatSessionIdentifier == sessionIdentifier);
            Assert.IsFalse(sessionState.PairsData.Any());
        }

        [Test]
        public void GetNextItems_ValidResults_Success()
        {
            // arrange
            var thetaStart = new double[] {0, 0, 0};
            var sectionConfig = new SectionConfiguration
            {
                SamplingIterations = 1,
                ComparisonsPerItem = 1,
                ItemIds = new[] {"1", "2", "3"},
                ThetaStart = thetaStart
            };
            //var sectionIdentifier = "67773552-5557-4CE6-AC3C-8657AEDAF33A";
            var sessionIdentifier = "66A5D7E1-608B-4272-9EB6-B9D381835F74";
            var sessionState = new PairwiseSessionState
            {
                CatSessionIdentifier = sessionIdentifier,
                ThetaLast = thetaStart
            }.ToJson().Base64Encode();
            var nextItemsRequest =
                GetNextItemsRequest(sessionState, "2", "3", "1", "0", 20);
            //var json = nextItemsRequest.ToJson();
            // act
            var processor = new PairwiseCatProcessor(sectionConfig.ToJson());
            var catResponse = processor.GetNextItems(nextItemsRequest).Result;

            // assert
            Assert.That(catResponse.StageLength == 2);
            Assert.That(catResponse.NextItemIds.Length == 2);
            Assert.That(catResponse.NextItemIds[0] != catResponse.NextItemIds[1]);
            //Assert.That(catResponse.NewSessionState == "ew0KICAiU2Vzc2lvbklkZW50aWZpZXIiOiAiNjZBNUQ3RTEtNjA4Qi00MjcyLTlFQjYtQjlEMzgxODM1Rjc0IiwNCiAgIkNvbXBhcmlzb25Ub3RhbCI6IDIsDQogICJQYWlyc0RhdGEiOiBbXSwNCiAgIlRoZXRhTGFzdCI6IFsNCiAgICAwLA0KICAgIDAsDQogICAgMA0KICBdDQp9");
            Assert.IsTrue(catResponse.NewSessionState.Base64Decode()
                .TryFromJson<PairwiseSessionState>(out var newSessionState));
            Assert.IsNotNull(newSessionState);
            Assert.That(newSessionState.CatSessionIdentifier == sessionIdentifier);
            Assert.That(newSessionState.PairsData.Count == 1);
            Assert.That(newSessionState.PairsData[0].Player1 != newSessionState.PairsData[0].Player2);
            Assert.That(newSessionState.ThetaLast.All(t => t != 0d));
        }

        private static ResultsDType GetNextItemsRequest(string sessionState, string item1Id, string item2Id,
            string item1Score, string item2Score, long duration)
        {
            var nextItemsRequest = new ResultsDType
            {
                //SectionIdentifier = sectionIdentifier,
                //SessionIdentifier = sessionIdentifier,
                SessionState = sessionState,
                AssessmentResult = new AssessmentResultDType
                {
                    ItemResult = new List<ItemResultDType>
                    {
                        new ItemResultDType
                        {
                            Identifier = item1Id,
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
                                        new ValueDType {Value = item1Score}
                                    }
                                },
                                new OutcomeVariableDType
                                {
                                    Identifier = "DURATION",
                                    Cardinality = OutcomeVariableDType.CardinalityEnum.SingleEnum,
                                    Value = new List<ValueDType>
                                    {
                                        new ValueDType {Value = duration.ToString()}
                                    }
                                }
                            }
                        },
                        new ItemResultDType
                        {
                            Identifier = item2Id,
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
                                        new ValueDType {Value = item2Score}
                                    }
                                },
                                new OutcomeVariableDType
                                {
                                    Identifier = "DURATION",
                                    Cardinality = OutcomeVariableDType.CardinalityEnum.SingleEnum,
                                    Value = new List<ValueDType>
                                    {
                                        new ValueDType {Value = duration.ToString()}
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return nextItemsRequest;
        }
    }
}