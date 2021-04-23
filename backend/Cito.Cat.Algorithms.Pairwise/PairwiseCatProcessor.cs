using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accord.Math;
using Cito.Cat.Algorithms.Pairwise.Models;
using Cito.Cat.Core.Helpers;
using Cito.Cat.Core.Interfaces;
using Cito.Cat.Core.Models.Session;
using Ims.Cat.Models;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Algorithms.Pairwise
{
    public class PairwiseCatProcessor : CatProcessor
    {
        private readonly SectionConfiguration _sectionConfiguration;
        private PairwiseSessionState _sessionState;
        private CatResponse _catResponse;
        private string SerializedSessionState => _sessionState.ToJson().Base64Encode();
        private readonly McMcSamplingHelper _helper;
        public static ICatProcessorInputValidator Validator => new PairwiseValidator();

        public PairwiseCatProcessor(string sectionConfiguration, ILoggerFactory loggerFactory = null,
            IAsyncDocumentSession asyncDocumentSession = null) : base(sectionConfiguration, loggerFactory,
            asyncDocumentSession)
        {
            Description = "Next items selection in pairs using Bayesian networks.";
            Logger = loggerFactory?.CreateLogger<PairwiseCatProcessor>();
            _helper = new McMcSamplingHelper();
            var errorOrSectionConfiguration = Validator.ParseSectionConfiguration(sectionConfiguration, Logger);
            if (errorOrSectionConfiguration.IsError())
            {
                var errors = ((Error) errorOrSectionConfiguration).Message;
                throw new DomainException($"Error(s) occurred while parsing the section configuration: {errors}", true);
            }

            _sectionConfiguration = (SectionConfiguration) errorOrSectionConfiguration;
        }

        public override async Task<CatResponse> InitializeSession(SessionDType sessionInitRequest,
            string sessionIdentifier)
        {
            // check if there's prior data with initial theta values
            var thetaStartPriorData = sessionInitRequest.PriorData?.FirstOrDefault(p => p.Key == "thetaStart");
            if (thetaStartPriorData != null && thetaStartPriorData.Value.TryFromJson<double[]>(out var thetaStart))
            {
                _sectionConfiguration.ThetaStart = thetaStart;
            }

            // check if there's prior data with priorMu
            var priorMuPriorData = sessionInitRequest.PriorData?.FirstOrDefault(p => p.Key == "priorMu");
            if (priorMuPriorData != null && priorMuPriorData.Value.TryFromJson<double>(out var priorMu))
            {
                _sectionConfiguration.PriorMu = priorMu;
            }

            // check if there's prior data with priorSigma
            var priorSigmaPriorData = sessionInitRequest.PriorData?.FirstOrDefault(p => p.Key == "priorSigma");
            if (priorSigmaPriorData != null && priorSigmaPriorData.Value.TryFromJson<double>(out var priorSigma))
            {
                _sectionConfiguration.PriorSigma = priorSigma;
            }

            _sessionState = new PairwiseSessionState
            {
                CatSessionIdentifier = sessionIdentifier,
                //ComparisonTotal =
                //    (int) Math.Round((_sectionConfig.ItemCount * _sectionConfig.ComparisonsPerItem) / 2.0),
                ThetaLast = _sectionConfiguration.ThetaStart
            };

            _catResponse = new CatResponse {StageLength = 2};
            await GetInitialItems();
            _catResponse.NewSessionState = SerializedSessionState;

            await UpdateSession();
            return _catResponse;
        }


        private async Task UpdateSession()
        {
            if (DocumentSession == null)
            {
                return;
            }

            var savedState = await DocumentSession.Query<PairwiseSessionState>()
                .FirstOrDefaultAsync(s => s.CatSessionIdentifier == _sessionState.CatSessionIdentifier);
            if (savedState != null)
            {
                savedState.PairsData = _sessionState.PairsData;
                savedState.PosteriorEstimates = _sessionState.PosteriorEstimates;
                savedState.ThetaLast = _sessionState.ThetaLast;
                await DocumentSession.StoreAsync(savedState);
            }
            else
            {
                await DocumentSession.StoreAsync(_sessionState);
            }
        }

        private Task GetInitialItems() => Task.Run(() =>
        {
            var pair = McMcSamplingHelper.SelectPairRestr(_sectionConfiguration.ThetaStart,
                _sessionState.PairsData.ToArray(),
                1);
            _catResponse.NextItemIds = new[]
                {_sectionConfiguration.ItemIds[pair[0]], _sectionConfiguration.ItemIds[pair[1]]};
        });

        public override async Task<CatResponse> GetNextItems(ResultsDType nextItemsRequest)
        {
            _catResponse = new CatResponse {StageLength = 2, AssessmentResult = nextItemsRequest.AssessmentResult};

            _sessionState = nextItemsRequest.SessionState.Base64Decode().FromJson<PairwiseSessionState>();
            UpdateSessionStateWithResults(nextItemsRequest.AssessmentResult);
            var (thetaLog, _, _, _) =
                _helper.McmcPcProbit(_sessionState.PairsData.ToArray(), _sectionConfiguration.ItemCount,
                    1, 100, _sectionConfiguration.ThetaStart, _sectionConfiguration.PriorMu,
                    _sectionConfiguration.PriorSigma);
            var thetaLast = thetaLog.GetRow(thetaLog.Rows() - 1);

            CheckStopCondition();
            if (_catResponse.StopConditionIsMet)
            {
                await GetPosteriorMeansAndStandardDeviations();
                _catResponse.NewSessionState = SerializedSessionState;
                await UpdateSession();
                return _catResponse;
            }

            var pair = McMcSamplingHelper.SelectPairRestr(thetaLast, _sessionState.PairsData.ToArray(), 1);
            _sessionState.ThetaLast = thetaLast;
            _catResponse.NextItemIds = new[]
                {_sectionConfiguration.ItemIds[pair[0]], _sectionConfiguration.ItemIds[pair[1]]};
            _catResponse.NewSessionState = SerializedSessionState;

            await UpdateSession();
            return _catResponse;
        }

        public override List<string> GetAllItemIds()
        {
            return _sectionConfiguration.ItemIds.ToList();
        }

        private Task GetPosteriorMeansAndStandardDeviations() => Task.Run(() =>
        {
            // Check for sane parameters, otherwise this takes too long.
            if (_sectionConfiguration.ItemCount > 50 && _sectionConfiguration.ComparisonsPerItem > 1)
            {
                return;
            }

            var (_, _, thetaEstimates, thetaSd) = _helper.McmcPcProbit(
                _sessionState.PairsData.ToArray(), _sectionConfiguration.ItemCount, 5000, 100,
                _sectionConfiguration.ThetaStart,
                _sectionConfiguration.PriorMu, _sectionConfiguration.PriorSigma);
            var estimates = new double[2][];
            estimates[0] = new double[thetaEstimates.Length];
            estimates[1] = new double[thetaEstimates.Length];
            estimates.SetRow(0, thetaEstimates);
            estimates.SetRow(1, thetaSd);
            _sessionState.PosteriorEstimates = estimates;
        });

        private void CheckStopCondition()
        {
            var comparisonTotal =
                (int) Math.Round((_sectionConfiguration.ItemCount * _sectionConfiguration.ComparisonsPerItem) / 2.0);
            if (comparisonTotal == _sessionState.PairsData.Count)
            {
                _catResponse.StopConditionIsMet = true;
                _catResponse.StopDescription = "All necessary pairwise comparisons have been made.";
                _catResponse.NextItemIds = new string[] { };
                _catResponse.StageLength = 0;
            }
        }

        private void UpdateSessionStateWithResults(AssessmentResultDType results)
        {
            if (results == null) return;

            if (results.ItemResult.Count != 2)
            {
                // do nothing, 2 item results are expected: one for each of the two items in the pair
                return;
            }

            foreach (var itemResult in results.ItemResult)
            {
                var scoreOutcomeVariable = itemResult.OutcomeVariables.FirstOrDefault(o => o.Identifier == "SCORE");
                if (scoreOutcomeVariable == null)
                {
                    // do nothing, 2 scores are expected: one for each of the two items in the pair
                    return;
                }
            }

            var item1Id = results.ItemResult[0].Identifier;
            var item2Id = results.ItemResult[1].Identifier;
            var item1Score = int.Parse(results.ItemResult[0].OutcomeVariables
                .First(o => o.Identifier == "SCORE")
                .Value[0].Value);
            item1Score = item1Score > 0 ? 1 : 0;
            var duration = long.Parse(results.ItemResult[0].OutcomeVariables
                .First(o => o.Identifier == "DURATION")
                .Value[0].Value);

            var item1Index = _sectionConfiguration.ItemIds.IndexOf(item1Id);
            var item2Index = _sectionConfiguration.ItemIds.IndexOf(item2Id);

            var newPairsData = new PairsData
            {
                Player1 = item1Index,
                Player2 = item2Index,
                Win1 = item1Score,
                Win2 = 1 - item1Score,
                Duration = duration
            };
            _sessionState.PairsData.Add(newPairsData);
        }

        public override List<string> ValidatePriorData(List<KeyValuePairDType> priorData)
        {
            return new();
        }

        public override List<string> ValidateNextItemsRequest(ResultsDType nextItemsRequest)
        {
            return new();
        }
    }
}