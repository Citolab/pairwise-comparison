using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Accord.Math;
using Cito.Cat.Algorithms.Pairwise;
using Cito.Cat.Algorithms.Pairwise.Models;
using Cito.Cat.Pairwise.Web.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Raven.DependencyInjection;

namespace Cito.Cat.Pairwise.Web.Pages.Admin
{
    public class SessionsModel : PageModel
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly ILogger<SessionsModel> _logger;

        [BindProperty] public IList<TestSessionModel> Sessions { get; set; }


        public SessionsModel(IAsyncDocumentSession documentSession, ILoggerFactory loggerFactory)
        {
            _documentSession = documentSession;
            _logger = loggerFactory.CreateLogger<SessionsModel>();
        }

        public async Task<IActionResult> OnGet()
        {
            Sessions = await _documentSession.Query<TestSession>()
                .Select(s => new TestSessionModel
                {
                    Id = s.Id, Name = s.Name, Status = s.Status, ComparisonsDone = s.ComparisonsDone,
                    ComparisonsTotal = s.ComparisonsTotal, StartCode = s.StartCode
                })
                .ToListAsync();
            return Page();
        }

        //[ResponseCache(3600)]
        public async Task<IActionResult> OnGetExportPostProcessedSessions()
        {
            var setup = HttpContext.RequestServices.GetRequiredService<IOptions<RavenOptions>>().Value;
            using var documentStore =
                setup.GetDocumentStore(docStore => docStore.Conventions.MaxNumberOfRequestsPerSession = 1000);
            using var documentSession = documentStore.OpenAsyncSession();
            var finishedSessions = await documentSession.Query<TestSession>()
                .Include(s => s.TestSectionIdentifier).Where(s => s.Status == TestStatus.PostProcessed).ToListAsync();


            var csvs = new Dictionary<string, byte[]>();
            foreach (var session in finishedSessions)
            {
                var section = await documentSession.LoadAsync<TestSection>(session.TestSectionIdentifier);
                var itemList = section.Items.Values.ToList();
                itemList.ForEach(i => i.Text = string.Empty);
                var pairwiseSessionState = await documentSession.Query<PairwiseSessionState>()
                    .FirstOrDefaultAsync(s => s.CatSessionIdentifier == session.CatSessionIdentifier);
                if (pairwiseSessionState == null)
                {
                    continue;
                }

                var name = session.Name.Replace('/', '-');

                // items
                using (var itemsMemoryStream = new MemoryStream())
                {
                    using (var streamWriter = new StreamWriter(itemsMemoryStream))
                    {
                        using (var csvWriter = new CsvWriter(streamWriter,
                            new CsvConfiguration(CultureInfo.InvariantCulture) {Delimiter = ";"}))
                        {
                            csvWriter.WriteHeader<Item>();
                            await csvWriter.NextRecordAsync();
                            await csvWriter.WriteRecordsAsync(itemList);
                        }
                    }

                    csvs.Add($"{name}-items.csv", itemsMemoryStream.ToArray());
                }

                // pairsdata
                using (var pairsdataMemoryStream = new MemoryStream())
                {
                    using (var streamWriter = new StreamWriter(pairsdataMemoryStream))
                    {
                        using (var csvWriter = new CsvWriter(streamWriter,
                            new CsvConfiguration(CultureInfo.InvariantCulture) {Delimiter = ";"}))
                        {
                            csvWriter.WriteField("Player1");
                            csvWriter.WriteField("Player2");
                            csvWriter.WriteField("Win1");
                            csvWriter.WriteField("Win2");
                            csvWriter.WriteField("Duration");
                            await csvWriter.NextRecordAsync();
                            foreach (var pairsData in pairwiseSessionState.PairsData)
                            {
                                csvWriter.WriteField(itemList[pairsData.Player1].Id);
                                csvWriter.WriteField(itemList[pairsData.Player2].Id);
                                csvWriter.WriteField(pairsData.Win1);
                                csvWriter.WriteField(pairsData.Win2);
                                csvWriter.WriteField(pairsData.Duration);
                                await csvWriter.NextRecordAsync();
                            }
                        }
                    }

                    csvs.Add($"{name}-pairsdata.csv", pairsdataMemoryStream.ToArray());
                }

                // using (var thetaMemoryStream = new MemoryStream())
                // {
                //     using (var streamWriter = new StreamWriter(thetaMemoryStream))
                //     {
                //         using (var csvWriter = new CsvWriter(streamWriter,
                //             new CsvConfiguration(CultureInfo.InvariantCulture) {Delimiter = ";"}))
                //         {
                //             csvWriter.WriteField("item");
                //             csvWriter.WriteField("theta");
                //             await csvWriter.NextRecordAsync();
                //             for (int i = 0; i < section.Items.Count; i++)
                //             {
                //                 csvWriter.WriteField(itemList[i].Id);
                //                 csvWriter.WriteField(pairwiseSessionState.ThetaLast[i]);
                //                 await csvWriter.NextRecordAsync();
                //             }
                //         }
                //     }
                //
                //     csvs.Add($"{name}-theta.csv", thetaMemoryStream.ToArray());
                // }

                using (var posteriorEstimatesMemoryStream = new MemoryStream())
                {
                    using (var streamWriter = new StreamWriter(posteriorEstimatesMemoryStream))
                    {
                        using (var csvWriter = new CsvWriter(streamWriter,
                            new CsvConfiguration(CultureInfo.InvariantCulture) {Delimiter = ";"}))
                        {
                            csvWriter.WriteField("item");
                            csvWriter.WriteField("estimate");
                            csvWriter.WriteField("sd");
                            await csvWriter.NextRecordAsync();
                            for (var i = 0; i < section.Items.Count; i++)
                            {
                                csvWriter.WriteField(itemList[i].Id);
                                csvWriter.WriteField(pairwiseSessionState.PosteriorEstimates[0][i]);
                                csvWriter.WriteField(pairwiseSessionState.PosteriorEstimates[1][i]);
                                await csvWriter.NextRecordAsync();
                            }
                        }
                    }

                    csvs.Add($"{name}-posterior-estimates.csv", posteriorEstimatesMemoryStream.ToArray());
                }
            }

            byte[] zipBytes;
            using (var zipMemoryStream = new MemoryStream())
            {
                using (var zipFile = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create))
                {
                    foreach (var csv in csvs)
                    {
                        var entry = zipFile.CreateEntry(csv.Key);
                        using (var streamWriter = new BinaryWriter(entry.Open()))
                        {
                            streamWriter.Write(csv.Value);
                        }
                    }
                }

                zipBytes = zipMemoryStream.ToArray();
            }

            var timestamp = $"{DateTime.Now:s}".Replace(":", "");
            return new FileStreamResult(new MemoryStream(zipBytes), "application/x-zip-compressed")
            {
                FileDownloadName = $"export-{timestamp}.zip"
            };
        }

        public async Task<IActionResult> OnPostComputePosteriorEstimatesForFinishedSessions()
        {
            var finishedSessions = await _documentSession.Query<TestSession>()
                .Include(s => s.TestSectionIdentifier).Where(s => s.Status == TestStatus.Finished).ToListAsync();
            foreach (var session in finishedSessions)
            {
                await ComputerPosteriorEstimates(session);
            }

            return RedirectToPage("Sessions");
        }

        public async Task<IActionResult> OnPostComputePosteriorEstimates(string testSessionId)
        {
            var session = await _documentSession.Include<TestSession>(s => s.TestSectionIdentifier)
                .LoadAsync<TestSession>(testSessionId);
            await ComputerPosteriorEstimates(session);
            return RedirectToPage("Sessions");
        }

        private async Task ComputerPosteriorEstimates(TestSession session)
        {
            var section = await _documentSession.LoadAsync<TestSection>(session.TestSectionIdentifier);
            var pairwiseSessionState = await _documentSession.Query<PairwiseSessionState>()
                .FirstOrDefaultAsync(s => s.CatSessionIdentifier == session.CatSessionIdentifier);
            if (pairwiseSessionState == null)
            {
                return;
            }

            _logger.LogInformation("Starting postprocessing {SessionName}...", session.Name);
            var stopWatch = Stopwatch.StartNew();
            var helper = new McMcSamplingHelper();
            var (_, _, thetaEstimates, thetaSd) =
                helper.McmcPcProbit(pairwiseSessionState.PairsData.ToArray(), section.Items.Count);
            stopWatch.Stop();
            var secondsTaken = stopWatch.ElapsedMilliseconds / 1000;
            _logger.LogInformation("Done postprocessing {SessionName}. Took {Seconds}s", session.Name, secondsTaken);
            var estimates = new double[2][];
            estimates[0] = new double[thetaEstimates.Length];
            estimates[1] = new double[thetaEstimates.Length];
            estimates.SetRow(0, thetaEstimates);
            estimates.SetRow(1, thetaSd);
            pairwiseSessionState.PosteriorEstimates = estimates;

            session.Status = TestStatus.PostProcessed;
            await _documentSession.StoreAsync(pairwiseSessionState);
            await _documentSession.StoreAsync(session);
        }
    }
}