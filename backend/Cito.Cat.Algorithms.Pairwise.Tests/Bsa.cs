using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Accord.Math;
using Accord.Statistics.Distributions.Univariate;
using Cito.Cat.Algorithms.Pairwise.Models;

namespace Cito.Cat.Algorithms.Pairwise.Tests
{
    [ExcludeFromCodeCoverage]
    public class Bsa
    {
        /// <summary>
        /// Bayesian selection algorithm
        /// </summary>
        /// <param name="nstud">Number of students</param>
        /// <param name="ncomp">Number of comparisons</param>
        /// <param name="nrater">Number of raters</param>
        /// <param name="ptrue">Probability table for comparisons</param>
        /// <param name="collectedData">Collected data</param>
        /// <param name="niter">Number of mcmc sampling iterations</param>
        /// <param name="burnin">Number of burn in iterations</param>
        /// <param name="thetaStart">Initial thetas</param>
        /// <param name="priorMu">Prior mean</param>
        /// <param name="priorSigma">Prior SD</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static (double[,] estimates, PairsData[] pairsData) Bsa2Restr(int nstud, int ncomp, int nrater,
            double[,] ptrue, PairsData[] collectedData = null, int niter = 5000, int burnin = 100,
            double[] thetaStart = null, double priorMu = 0, double priorSigma = 1)
        {
            var helper = new McMcSamplingHelper();
            // initialize thetaStart if it's null
            thetaStart ??= Enumerable.Repeat(0d, nstud).ToArray();

            // # compute total number of comparisons
            // comptot <- round(nstud * ncomp / 2)
            var compTotal = (int) Math.Round(nstud * ncomp / 2.0);

            // # initial dat matrix
            //dat <- as.data.frame(matrix(NA, comptot, 4))
            // names(dat) <- list("player1","player2","win1","win2")
            var pairsData = new List<PairsData>(); // using a growing list instead of an initialized array
            //var pairsData = new PairsData[compTotal];
            // for (int i = 0; i < compTotal; i++)
            // {
            //     pairsData[i] = new PairsData {Player1 = 1, Player2 = 1, Win1 = 1, Win2 = 1, Zij = 0};
            // }

            // # compute number of comparisons to be made/collected
            // comptocol <- comptot - nrow(datcol)
            // THT: moved outside of the if-statement because it's needed in the outer scope
            var compToCollect = compTotal - (collectedData?.Length ?? 0);
            var thetaLast = thetaStart;
            if (collectedData != null)
            {
                if (compToCollect == 0)
                {
                    Console.WriteLine("Number of comparisons was already reached.");
                    return (null, null);
                }

                if (compToCollect < 0)
                {
                    throw new Exception("More comparisons already collected than total number of comparisons");
                }

                // dat[1:nrow(datcol),] <- datcol
                pairsData.AddRange(collectedData);

                // out <- mcmcPCprobit(dat[is.na(dat[,1])==F,], nstud, niter=niter, burnin=burnin, theta_start = theta_start, prior_mu=prior_mu, prior_sigma=prior_sigma)
                var (thetaLog, _, _, _) =
                    helper.McmcPcProbit(pairsData.ToArray(), nstud, niter, burnin, thetaStart, priorMu, priorSigma);
                // out <- out$theta_log[nrow(out$theta_log),]
                thetaLast = thetaLog.GetRow(thetaLog.Rows() - 1);
            }

            // pair <- selectPair1restr(out,dat,nrater)
            var pair = McMcSamplingHelper.SelectPairRestr(thetaLast, pairsData.ToArray(), nrater);

            // # compare the students
            // win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
            var win1 = UniformContinuousDistribution.Random() <= ptrue[pair[0], pair[1]] ? 1 : 0;

            // # updata data
            // dat[nrow(datcol)+1,] <- c(pair,win1,1-win1)
            pairsData.Add(new PairsData {Player1 = pair[0], Player2 = pair[1], Win1 = win1, Win2 = 1 - win1});

            // for (x in 2:comptocol) {
            for (int i = 1; i <= compToCollect; i++)
            {
                // out <- mcmcPCprobit(dat[is.na(dat[,1])==F,], nstud, niter=niter, burnin=burnin, theta_start = theta_start, prior_mu=prior_mu, prior_sigma=prior_sigma)
                var (thetaLog, _, _, _) =
                    helper.McmcPcProbit(pairsData.ToArray(), nstud, niter, burnin, thetaStart, priorMu, priorSigma);
                // out <- out$theta_log[nrow(out$theta_log),]
                thetaLast = thetaLog.GetRow(thetaLog.Rows() - 1);
                // pair <- selectPair1restr(out,dat,nrater)
                pair = McMcSamplingHelper.SelectPairRestr(thetaLast, pairsData.ToArray(), nrater);

                // # compare the students
                // win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
                win1 = UniformContinuousDistribution.Random() <= ptrue[pair[0], pair[1]] ? 1 : 0;

                // # updata data
                // dat[sum(is.na(dat[,1])==F)+1,] <- c(pair,win1,1-win1)
                pairsData.Add(new PairsData {Player1 = pair[0], Player2 = pair[1], Win1 = win1, Win2 = 1 - win1});
            }

            // # compute posterior means and standard deviations
            // out <- mcmcPCprobit(dat, nstud, niter=5000, burnin=burnin, theta_start = theta_start, prior_mu=prior_mu, prior_sigma=prior_sigma)
            var (_, _, thetaEstimates, thetaSd) = helper.McmcPcProbit(
                pairsData.ToArray(), nstud, 5000, burnin, thetaStart, priorMu, priorSigma);
            // out <- cbind(theta_est=out$theta_est,theta_sd=out$theta_sd)
            var estimates = new double[thetaEstimates.Length, 2];
            estimates.SetColumn(0, thetaEstimates);
            estimates.SetColumn(1, thetaSd);


            // # return result
            // return(list(estimates=out, data=dat))
            return (estimates, pairsData.ToArray());
        }
    }
}