using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Statistics;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Distributions.Univariate;
using Cito.Cat.Algorithms.Pairwise.Models;
// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming

namespace Cito.Cat.Algorithms.Pairwise
{
    public class McMcSamplingHelper
    {
        /// <summary>
        /// Sample Zij values
        /// </summary>
        /// <param name="thetaEstimates">Current theta estimate values</param>
        /// <param name="pairsData">data table:  column 1-2 the object pairings; column 3-4 wins for object 1-2; ...</param>
        public static IEnumerable<double> SampleZ(double[] thetaEstimates, PairsData[] pairsData)
        {
            // Zmean <- theta[dat[,1]]-theta[dat[,2]]
            var zmean = pairsData.Select(d => thetaEstimates[d.Player1] - thetaEstimates[d.Player2]).ToArray();
            // cdf0 <- pnorm(0, mean=Zmean)
            var cdf0 = zmean.Select(z => new NormalDistribution(z).DistributionFunction(0));
            //  r_unif <- runif(length(Zmean))
            var rUnif = Enumerable.ToArray(UniformContinuousDistribution.Random(zmean.Length));
            // win <- dat[,3]
            var wins = pairsData.Select(p => p.Win1).ToArray();
            // cdf_sample <- (cdf0 * (1-win) + (1-cdf0) * win) * r_unif + cdf0 * win
            var cdfSample = cdf0.Select((cdf, index) =>
                (cdf * (1 - wins[index]) + (1 - cdf) * wins[index]) * rUnif[index] + cdf * wins[index]);
            // Zij <- qnorm(cdf_sample, mean=Zmean)
            var zij = cdfSample.Select(
                (s, index) => new NormalDistribution(zmean[index]).InverseDistributionFunction(s));
            return zij;
        }

        private int? _nstud;
        private double? _priorMu;
        private double[] _postMu;
        private double? _priorSigma;
        private double[,] _postSigma;

        /// <summary>
        /// Sample theta values
        /// </summary>
        /// <param name="pairsData">data table: column 1-2 the object pairings; column 3-4 wins for object 1-2; column 5 sampled Zij; </param>
        /// <param name="thetas">vector of sampled theta in previous iteration</param>
        /// <param name="nstud">Number of students</param>
        /// <param name="priorMu">prior mean</param>
        /// <param name="priorSigma">prior SD</param>
        public double[] SampleTheta(PairsData[] pairsData, double[] thetas, int nstud, double priorMu = 0,
            double priorSigma = 1)
        {
            // compute prior variance
            // prior_sigma2 <- prior_sigma^2
            var priorSigma2 = Math.Pow(priorSigma, 2);
            // compute mean and variance for every theta and sample from the posterior distribution
            // post_mu <- rep(prior_mu, nstud)
            if (!_nstud.HasValue || _nstud != nstud)
            {
                _nstud = nstud;
                if (!_priorMu.HasValue || _priorMu != priorMu)
                {
                    _priorMu = priorMu;
                    _postMu = Enumerable.Repeat(priorMu, nstud).ToArray();
                }


                // post_sigma <- diag(rep(prior_sigma, nstud))
                if (!_priorSigma.HasValue || _priorSigma != priorSigma)
                {
                    _priorSigma = priorSigma;
                    _postSigma = Matrix.Diagonal(Enumerable.Repeat(priorSigma, nstud).ToArray());
                }
            }

            // geparallelliseerd, omdat alles in de for-loop zo te zien atomair is
            // (geen onderlinge afhankelijkheden tussen iteraties)
            Parallel.For(0, nstud, i =>
            {
                // ncomp_i <- nrow(dat[dat$player1==i|dat$player2==i,])
                var ncomp_i = pairsData.Count(p => p.Player1 == i || p.Player2 == i);
                //  tempsum <- prior_mu/(prior_sigma^2)
                var tempSum = priorMu / priorSigma2;
                // tempsum <- tempsum + sum(dat[dat$player1==i,"Zij"], theta[dat[dat$player1==i,"player2"]],
                //                          -dat[dat$player2==i,"Zij"], theta[dat[dat$player2==i,"player1"]])
                var player2WherePlayer1IsCurrentStud =
                    pairsData.Where(p => p.Player1 == i).Select(p => p.Player2).ToArray();
                var player1WherePlayer2IsCurrentStud =
                    pairsData.Where(p => p.Player2 == i).Select(p => p.Player1).ToArray();
                var player2ThetaSum = player2WherePlayer1IsCurrentStud.Select(index => thetas[index]).Sum();
                var player1ThetaSum = player1WherePlayer2IsCurrentStud.Select(index => thetas[index]).Sum();
                tempSum += pairsData.Sum(p => p.Player1 == i && p.Zij.HasValue ? p.Zij.Value : 0)
                           + player2ThetaSum
                           - pairsData.Sum(p => p.Player2 == i && p.Zij.HasValue ? p.Zij.Value : 0)
                           + player1ThetaSum;
                // post_mu[i] <- tempsum/(ncomp_i+1/prior_sigma^2)
                _postMu[i] = tempSum / (ncomp_i + 1 / priorSigma2); // i-1 because indexes in c# are zero-based 
                // post_sigma[i,i] <- 1/(ncomp_i+1/prior_sigma^2)
                _postSigma[i, i] = 1 / (ncomp_i + 1 / priorSigma2); // i-1 because indexes in c# are zero-based
            });


            // for (var i = 1; i <= nstud; i++)
            // {
            //     // ncomp_i <- nrow(dat[dat$player1==i|dat$player2==i,])
            //     var ncomp_i = pairsData.Count(p => p.Player1 == i || p.Player2 == i);
            //     //  tempsum <- prior_mu/(prior_sigma^2)
            //     var tempSum = priorMu / Math.Pow(priorSigma, 2);
            //     // tempsum <- tempsum + sum(dat[dat$player1==i,"Zij"], theta[dat[dat$player1==i,"player2"]],
            //     //                          -dat[dat$player2==i,"Zij"], theta[dat[dat$player2==i,"player1"]])
            //     tempSum += pairsData.Sum(p => p.Player1 == i ? (p.Zij ?? 0) : 0)
            //                + thetas.Select((t, index) =>
            //                        pairsData.Where(p => p.Player1 == i).Select(p => p.Player2).Contains(index) ? t : 0)
            //                    .Sum()
            //                - pairsData.Sum(p => p.Player2 == i ? (p.Zij ?? 0) : 0)
            //                + thetas.Select((t, index) =>
            //                        pairsData.Where(p => p.Player2 == i).Select(p => p.Player1).Contains(index) ? t : 0)
            //                    .Sum();
            //     // post_mu[i] <- tempsum/(ncomp_i+1/prior_sigma)
            //     postMu[i - 1] = tempSum / (ncomp_i + 1 / priorSigma); // i-1 because indexes in c# are zero-based 
            //     // post_sigma[i,i] <- 1/(ncomp_i+1/prior_sigma)
            //     postSigma[i - 1, i - 1] = 1 / (ncomp_i + 1 / priorSigma); // i-1 because indexes in c# are zero-based
            // }


            // theta_new <- mvrnorm(mu = post_mu, Sigma=post_sigma)
            var thetaNew = new MultivariateNormalDistribution(_postMu, _postSigma).Generate();
            // Renormalize theta
            // theta_new <- theta_new - mean(theta_new)
            var meanThetaNew = thetaNew.Mean();
            thetaNew = thetaNew.Select(t => t - meanThetaNew).ToArray();
            return thetaNew;
        }

        /// <summary>
        /// MCMC sampling (Markov chain Monte Carlo)
        /// </summary>
        /// <param name="pairsData">data table: column 1-2 the object pairings; column 3-4 wins for object 1-2;
        /// pair object numbers in ascending order over columns</param>
        /// <param name="nstud">Number of students (number of objects in sample)</param>
        /// <param name="niter">number of MCMC iterations after estimated burn-in</param>
        /// <param name="burnin">expected number of iterations of burn-in period</param>
        /// <param name="thetaStart">start values for latent variables</param>
        /// <param name="priorMu">prior mean</param>
        /// <param name="priorSigma">prior SD</param>
        public (double[,] thetaLog, double[,] zijLog, double[] thetaEstimates, double[] thetaSd) McmcPcProbit(
            PairsData[] pairsData, int nstud, int niter = 5000, int burnin = 100, double[] thetaStart = null,
            double priorMu = 0, double priorSigma = 1)
        {
            // initialize thetaStart if it's null
            thetaStart ??= Enumerable.Repeat(0d, nstud).ToArray();

            // nit <- niter + burnin
            var nit = niter + burnin;
            // dat$Zij <- NA
            Array.ForEach(pairsData, p => p.Zij = null);
            // theta_log <- matrix(NA,nrow=nit+1,ncol=nstud)
            var thetaLog = Matrix.Create(nit + 1, nstud, default(double));
            // theta_log[1,] <- theta_start
            thetaLog.SetRow(0, thetaStart);
            // Zij_log <- matrix(NA,nrow=nit+1,ncol=nrow(dat))
            var zijLog = Matrix.Create(nit + 1, nstud, default(double));

            double[] thetaEstimates;

            for (int it = 0; it < nit; it++)
            {
                // # 1. Bepaal de conditionele posteriors o.b.v. de huidige data

                // # 2. Kies startwaarden voor de parameters: 0 voor alle theta_i
                // theta_est <- theta_log[it,]
                thetaEstimates = thetaLog.GetRow(it);
                // # 3. Simuleer trekkingen uit Z|theta,X waarden
                // dat$Zij <- sampleZ(theta_est, dat)
                var zij = SampleZ(thetaEstimates, pairsData).ToArray();
                for (var j = 0; j < zij.Length; j++)
                {
                    pairsData[j].Zij = zij[j];
                }

                // Zij_log[it+1,] <- dat$Zij
                //zijLog.SetRow(it, zij);

                // # 4. Simuleer trekkingen uit de conditionele posterior van theta| Z
                // theta_est <- sampleTheta(dat,theta_log[it, ],prior_mu = prior_mu,prior_sigma = prior_sigma)
                thetaEstimates = SampleTheta(pairsData, thetaLog.GetRow(it), nstud, priorMu, priorSigma);
                //  theta_log[it+1,] <- theta_est
                thetaLog.SetRow(it + 1, thetaEstimates);
            }

            // # compute summary statistics after burn-in
            double[] thetaSd;
            //double sdTheta;
            if (niter == 1)
            {
                // # posterior mean per theta
                //     theta_est <- theta_log[it+1,]
                thetaEstimates = thetaLog.GetRow(nit);
                // # posterior sd per theta
                //     theta_sd <- rep(NA, length(theta_est))
                thetaSd = Enumerable.Repeat(default(double), thetaEstimates.Length).ToArray();
                // # posterior sd of theta's
                //     sd_theta <- sd(theta_log[-(1:(burnin+1)),])
                //sdTheta = thetaLog.GetRow(thetaLog.Rows() - 1).StandardDeviation(); // sd of the last row
            }
            else
            {
                // # posterior mean per theta
                //     theta_est <- apply(theta_log[-(1:(burnin+1)),],2,mean)
                // http://accord-framework.net/docs/html/M_Accord_Statistics_Measures_Mean_2.htm
                // The dimension along which the means will be calculated. Pass 0 to compute a row vector containing
                // the mean of each column, or 1 to compute a column vector containing the mean of each row. Default value is 0. 
                var thetaLogWithoutBurnin = thetaLog.Get(burnin, thetaLog.Rows(), 0, nstud);
                thetaEstimates = thetaLogWithoutBurnin.Mean(0);

                // # posterior sd per theta
                //     theta_sd <- apply(theta_log[-(1:(burnin+1)),],2,sd)
                thetaSd = thetaLogWithoutBurnin.StandardDeviation();
                // # posterior sd of theta's
                //     sd_theta <- mean(apply(theta_log[-(1:(burnin+1)),],1,sd))
                //sdTheta = thetaLogWithoutBurnin.Transpose().StandardDeviation().Mean();
            }

            // return(list(theta_log=theta_log, Zij_log=Zij_log, theta_est=theta_est, theta_sd=theta_sd))
            return (thetaLog, zijLog, thetaEstimates, thetaSd);
        }

        /// <summary>
        /// Select the next pair
        /// </summary>
        /// <param name="thetas">Sample from posterior multivariate theta distribution</param>
        /// <param name="pairsData">data table: column 1-2 the object pairings; column 3-4 wins for object 1-2; ...</param>
        /// <param name="nrater">Number of raters</param>
        /// <returns></returns>
        public static int[] SelectPairRestr(double[] thetas, PairsData[] pairsData, int nrater)
        {
            //return new[] {1, 1};
            //   
            // # determine number of objects
            // n <- length(theta)
            var n = thetas.Length;

            // Bepaal alle combinaties min de combinaties die al nrater maal getrokken zijn
            // # create data frame with unique pairs
            // pairmatrix <- lower.tri(matrix(1,n,n))*1
            // allpairs <- which(pairmatrix==1,arr.ind=T)[,2:1]
            // pairsnot <- names(table(paste(dat[,1],dat[,2])))[table(paste(dat[,1],dat[,2]))==nrater]
            // pairs <- allpairs[(paste(allpairs[,1],allpairs[,2]) %in% pairsnot)==0,]
            // if (length(dim(pairs))<2) {
            //     pairs <- t(pairs)
            // }
            // colnames(pairs) <- c("player1","player2")

            // generate list of all possible combinations
            var combinations = Enumerable.Range(0, n).ToArray().Combinations(2).ToList();
            //var combs = Matrix.Create(combinations);
            // get all pairs that have been fully rated (rated by nrater raters)
            var fullyRated = pairsData
                .GroupBy(p => new {p.Player1, p.Player2})
                .Where(g => g.Count() == nrater);
            // remove these pairs from the combinations list
            foreach (var pair in fullyRated)
            {
                combinations.RemoveAll(c => c[0] == pair.Key.Player1 && c[1] == pair.Key.Player2);
            }


            // # add theta values of the pairs to the data frame
            // pairs <- as.data.frame(cbind(pairs,theta1=theta[pairs[,1]],theta2=theta[pairs[,2]]))
            // # compute Zij values
            // pairs$Zij <- pairs[,3] - pairs[,4]
            // # compute absolute Zij values
            // pairs$Zij_abs <- abs(pairs$Zij)
            // # select the pair with the lowest absolute Zij values
            // pairs_eligible <- pairs[pairs$Zij_abs==min(pairs$Zij_abs),1:2]
            // if (length(pairs$Zij_abs==min(pairs$Zij_abs)) > 1) {
            //     pair <- pairs_eligible[sample(nrow(pairs_eligible),1),]
            // } else{
            //     pair <- pairs_eligible
            // }
            // return(unlist(pair))     

            var absZijValues = new double[combinations.Count];
            // create list with abs Zij values
            for (int i = 0; i < combinations.Count; i++)
            {
                absZijValues[i] = Math.Abs(thetas[combinations[i][0]] - thetas[combinations[i][1]]);
            }

            var minZijValue = absZijValues.Min();
            // select one randomly if there's more than one
            if (absZijValues.Count(v => v == minZijValue) > 1)
            {
                var indexes = absZijValues.Select((v, index) => v == minZijValue ? index : -1).Where(i => i != -1)
                    .ToArray();
                return combinations[indexes.Sample(1)[0]];
            }

            var index = absZijValues.IndexOf(minZijValue);
            return combinations[index];
        }
    }
}