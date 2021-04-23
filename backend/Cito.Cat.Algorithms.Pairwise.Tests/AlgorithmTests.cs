using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using Accord.Math;
using Accord.Statistics;
using Accord.Statistics.Distributions.Univariate;
using Cito.Cat.Algorithms.Pairwise.Models;
using CsvHelper;
using CsvHelper.Configuration;
using NUnit.Framework;

namespace Cito.Cat.Algorithms.Pairwise.Tests
{
    [ExcludeFromCodeCoverage]
    public class AlgorithmTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestSampleZ()
        {
            var nstud = 4;
            var theta = new NormalDistribution().Generate(nstud);
            var meanTheta = theta.Mean();
            theta = theta.Select(t => t - meanTheta).ToArray();

            var pairsData = new PairsData[4];
            pairsData[0] = new PairsData {Player1 = 0, Player2 = 1, Win1 = 1, Win2 = 0};
            pairsData[1] = new PairsData {Player1 = 0, Player2 = 2, Win1 = 1, Win2 = 0};
            pairsData[2] = new PairsData {Player1 = 1, Player2 = 3, Win1 = 0, Win2 = 1};
            pairsData[3] = new PairsData {Player1 = 1, Player2 = 2, Win1 = 0, Win2 = 1};

            var sampleZ = McMcSamplingHelper.SampleZ(theta, pairsData).ToList();

            for (int i = 0; i < pairsData.Length; i++)
            {
                Console.WriteLine($"index {i} {pairsData[i].Win1} {sampleZ[i]}");

                Assert.That(pairsData[i].Win1 == 1 ? sampleZ[i] > 0 : true);
                Assert.That(pairsData[i].Win1 == 0 ? sampleZ[i] < 0 : true);
            }

            //Assert.That(sampleZ.All(s => s != default));
        }

        [Test]
        public void TestMcMcPcProbit()
        {
            var nstud = 30;
            var ncomp = 15;
            var nrater = 1;

            var theta = new NormalDistribution().Generate(nstud);
            var meanTheta = theta.Mean();
            theta = theta.Select(t => t - meanTheta).ToArray(); // normalize

            var ptrue = Matrix.Create(nstud, nstud, 0d);
            var distr = new NormalDistribution();
            for (int i = 0; i < nstud; i++)
            {
                // var row = Enumerable.Range(0, nstud)
                //     .Select(n => n != i ? distr.DistributionFunction(theta[i] - theta[n]) : 0);
                var row = new List<double>();
                // using a foreach instead of LINQ so we don't capture i in a closure (because of memory)
                foreach (var n in Enumerable.Range(0, nstud))
                {
                    row.Add(n == 1 ? 0 : distr.DistributionFunction(theta[i] - theta[n]));
                }

                ptrue.SetRow(i, row.ToArray());
            }

            var rows = (int) Math.Round((nstud * ncomp) / 2.0);
            var pairsData = new List<PairsData>(); // using a growing list instead of an initialized array
            for (int i = 1; i <= rows; i++)
            {
                // set thetaStart to 0
                var thetaStart = Enumerable.Repeat(0d, nstud).ToArray();
                // select pair as if all estimates are 0, (i.e., at random)
                var pair = McMcSamplingHelper.SelectPairRestr(thetaStart, pairsData.ToArray(), nrater);

                // # compare the students
                // win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
                var win1 = UniformContinuousDistribution.Random() <= ptrue[pair[0], pair[1]] ? 1 : 0;

                // # updata data
                // dat[nrow(datcol)+1,] <- c(pair,win1,1-win1)
                pairsData.Add(new PairsData { Player1 = pair[0], Player2 = pair[1], Win1 = win1, Win2 = 1 - win1 });
            }

            var stopWatch = Stopwatch.StartNew();
            var helper = new McMcSamplingHelper();
            var (thetaLog, _, thetaEstimates, thetaSd) = helper.McmcPcProbit(pairsData.ToArray(), 30, 5000);
            stopWatch.Stop();
            Console.WriteLine($"Run took {stopWatch.Elapsed}");
            var thetaRow = thetaLog.GetRow(thetaLog.Rows() - 1).ToList();
            Assert.That(thetaRow.Count == 30);
            Assert.That(thetaRow.All(t => t != default));
            Assert.That(thetaLog.Rows() == 5101);
            Assert.That(thetaEstimates.Length == nstud);
            Assert.That(thetaSd.Length == nstud);
            Assert.That(thetaEstimates.Mean() < Math.Pow(10, -6));

            // write to csv, find the files in
            // PairwiseCsharp\Cito.Cat.Algorithms.Pairwise.Tests\bin\Debug\netcoreapp3.1

            using (var streamWriter = new StreamWriter("TestMcMcPcProbit_pairsData.csv"))
            {
                using (var csvWriter = new CsvWriter(streamWriter,
                    new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" }))
                {
                    csvWriter.WriteHeader<PairsData>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecords(pairsData);
                }
            }
            
            using (var streamWriter = new StreamWriter("TestMcMcPcProbit_theta.csv"))
            {
                using (var csvWriter = new CsvWriter(streamWriter,
                    new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" }))
                {
                    for (int i = 0; i < nstud; i++)
                    {
                        csvWriter.WriteField(theta[i]);
                    }
                    csvWriter.NextRecord();
                }
            }
            
            using (var streamWriter = new StreamWriter("TestMcMcPcProbit_thetaLog.csv"))
            {
                using (var csvWriter = new CsvWriter(streamWriter,
                    new CsvConfiguration(CultureInfo.InvariantCulture) {Delimiter = ";"}))
                {
                    for (int i = 0; i < thetaLog.Rows(); i++)
                    {
                        for (int j = 0; j < nstud; j++)
                        {
                            csvWriter.WriteField(thetaLog[i, j]);
                        }

                        csvWriter.NextRecord();
                    }
                }
            }

            using (var streamWriter = new StreamWriter("TestMcMcPcProbit_thetaEst.csv"))
            {
                using (var csvWriter = new CsvWriter(streamWriter,
                    new CsvConfiguration(CultureInfo.InvariantCulture) {Delimiter = ";"}))
                {
                    csvWriter.WriteField("estimate");
                    csvWriter.WriteField("sd");
                    csvWriter.NextRecord();
                    for (int i = 0; i < nstud; i++)
                    {
                        csvWriter.WriteField(thetaEstimates[i]);
                        csvWriter.WriteField(thetaSd[i]);
                        csvWriter.NextRecord();
                    }
                }
            }
        }

        [Test]
        public void TestBsa2Restr()
        {
            var nstud = 50;
            var ncomp = 1;
            var nrater = 1;

            // theta <- rnorm(nstud,0,1) 
            // theta <- theta-mean(theta)
            var theta = new NormalDistribution().Generate(nstud);
            var meanTheta = theta.Mean();
            theta = theta.Select(t => t - meanTheta).ToArray(); // normalize

            // ptrue <- matrix(NA,nstud,nstud)
            // for (i in 1:nstud) {
            //     # fill rows with win probabilities, columns then contain loss probabilities
            //     ptrue[i,-i] <- pnorm(theta[i]-theta[-i])
            // }
            var ptrue = Matrix.Create(nstud, nstud, 0d);
            var distr = new NormalDistribution();
            for (int i = 0; i < nstud; i++)
            {
                // var row = Enumerable.Range(0, nstud)
                //     .Select(n => n != i ? distr.DistributionFunction(theta[i] - theta[n]) : 0);
                var row = new List<double>();
                // using a foreach instead of LINQ so we don't capture i in a closure (because of memory)
                foreach (var n in Enumerable.Range(0, nstud))
                {
                    row.Add(n == 1 ? 0 : distr.DistributionFunction(theta[i] - theta[n]));
                }

                ptrue.SetRow(i, row.ToArray());
            }

            var stopWatch = Stopwatch.StartNew();
            var result = Bsa.Bsa2Restr(nstud, ncomp, nrater, ptrue, niter: 1);
            stopWatch.Stop();
            Console.WriteLine($"Took {stopWatch.Elapsed}");
        }
    }
}