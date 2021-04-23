# rm(list=ls())
# setwd("~/SimulatieBSA")
library(MASS)

# sampling functions
#------------------------------------------------------------------------
# function to sample Zij values
sampleZ <- function(theta, dat) {
  # theta = current theta estimate values
  # dat = data table: column 1-2 the object pairings; column 3-4 wins for object 1-2; ...
  Zmean <- theta[dat[,1]]-theta[dat[,2]]
  cdf0 <- pnorm(0, mean=Zmean)
  r_unif <- runif(length(Zmean))
  win <- dat[,3]
  cdf_sample <- (cdf0 * (1-win) + (1-cdf0) * win) * r_unif + cdf0 * win
  Zij <- qnorm(cdf_sample, mean=Zmean)
  # Zij <- rnorm(length(Zmean),Zmean,1)
  return(Zij)
}

#------------------------------------------------------------------------
# function to sample theta values
sampleTheta <- function (dat, theta=rep(0, nstud), nstud=length(theta), prior_mu=0, prior_sigma=1) {
  # dat = data table: column 1-2 the object pairings; column 3-4 wins for object 1-2; column 5 sampled Zij; ...
  # theta = vector of sampled theta in previous iteration
  # prior_mu = prior mean
  # prior_sigma = prior SD
  
  # compute mean and variance for every theta and sample from the posterior distribution
  #theta_new <- rep(NA, nstud) # THT not used
  post_mu <- rep(prior_mu, nstud)
  post_sigma <- diag(rep(prior_sigma, nstud))
  for (i in 1:nstud) {
    ncomp_i <- nrow(dat[dat$player1==i|dat$player2==i,])
    tempsum <- prior_mu/(prior_sigma^2)
    tempsum <- tempsum + sum(dat[dat$player1==i,"Zij"], theta[dat[dat$player1==i,"player2"]],
                             -dat[dat$player2==i,"Zij"], theta[dat[dat$player2==i,"player1"]])
    post_mu[i] <- tempsum/(ncomp_i+1/prior_sigma)
    post_sigma[i,i] <- 1/(ncomp_i+1/prior_sigma)
    # theta_new[i] <- rnorm(1, post_mu, post_sigma)
  }
  theta_new <- mvrnorm(mu = post_mu, Sigma=post_sigma)
  
  # Renormalize theta
  theta_new <- theta_new - mean(theta_new)
}

#------------------------------------------------------------------------
# function for MCMC sampling
mcmcPCprobit <- function(dat, nstud, niter=5000, burnin=100, theta_start=rep(0, nstud),
                         prior_mu=0, prior_sigma=1) {
  # dat = data table: column 1-2 the object pairings; column 3-4 wins for object 1-2; ...
  #       pair object numbers in ascending order over columns
  # nstud = number of objects in sample
  # niter = number of MCMC iterations after estimated burn-in
  # burnin = expected number of iterations of burn-in period
  # theta_start = startvalues for latent variables
  
  nit <- niter + burnin
  #dat <- as.data.frame(dat)
  dat$Zij <- NA
  theta_log <- matrix(NA,nrow=nit+1,ncol=nstud)
  theta_log[1,] <- theta_start
  # theta_log[1,] <- rnorm(nstud,0,2)
  Zij_log <- matrix(NA,nrow=nit+1,ncol=nrow(dat))
  colnames(Zij_log) <- paste(dat[,1],dat[,2],sep=",")
  start_time <- Sys.time()
  for (it in 1:nit) {
    # 1. Bepaal de conditionele posteriors o.b.v. de huidige data
    
    # 2. Kies startwaarden voor de parameters: 0 voor alle theta_i
    theta_est <- theta_log[it,]
    
    # 3. Simuleer trekkingen uit Z|theta,X waarden
    dat$Zij <- sampleZ(theta_est, dat)
    
    Zij_log[it+1,] <- dat$Zij
    
    # 4. Simuleer trekkingen uit de conditionele posterior van theta| Z
    theta_est <- sampleTheta(dat,theta_log[it, ],prior_mu = prior_mu,prior_sigma = prior_sigma)
    
    theta_log[it+1,] <- theta_est
    # print(it)
  }
  end_time <- Sys.time()
  print(end_time-start_time)
  # compute summarystatistics after burn-in
  if (niter == 1) {
    # posterior mean per theta
    theta_est <- theta_log[it+1,]
    # posterior sd per theta
    theta_sd <- rep(NA, length(theta_est))
    # posterior sd of theta's
    sd_theta <- sd(theta_log[-(1:(burnin+1)),])
  } else {
    # posterior mean per theta
    theta_est <- apply(theta_log[-(1:(burnin+1)),],2,mean)
    # posterior sd per theta
    theta_sd <- apply(theta_log[-(1:(burnin+1)),],2,sd)
    # posterior sd of theta's
    sd_theta <- mean(apply(theta_log[-(1:(burnin+1)),],1,sd))
  }
  
  return(list(theta_log=theta_log, Zij_log=Zij_log, theta_est=theta_est, theta_sd=theta_sd))
  #, sd_theta=sd_theta))
} # +/- .5 minutes for 5000 iterations & n=30 & ncomp=1

#------------------------------------------------------------------------
selectPair1 <- function(theta) {
  # theta = sample from posterior multivariate theta distribution
  
  # determine number of objects
  n <- length(theta)
  # create data frame with unique pairs
  pairmatrix <- lower.tri(matrix(1,n,n))*1
  pairs <- which(pairmatrix==1,arr.ind=T)
  colnames(pairs) <- c("player1","player2")
  # add theta values of the pairs to the data frame
  pairs <- as.data.frame(cbind(pairs,theta1=theta[pairs[,1]],theta2=theta[pairs[,2]]))
  # compute Zij values
  pairs$Zij <- pairs[,3] - pairs[,4]
  # compute absolute Zij values
  pairs$Zij_abs <- abs(pairs$Zij)
  # select the pair with the lowest absolute Zij values
  pairs_eligible <- pairs[pairs$Zij_abs==min(pairs$Zij_abs),1:2]
  if (length(pairs$Zij_abs==min(pairs$Zij_abs)) > 1) {
    pair <- pairs_eligible[sample(nrow(pairs_eligible),1),]
  } else{
    pair <- pairs_eligible
  }
  return(unlist(pair))
}

#------------------------------------------------------------------------
selectPair1restr <- function(theta, dat, nrater) {
  # theta = sample from posterior multivariate theta distribution
  # dat = data table: column 1-2 the object pairings; column 3-4 wins for object 1-2; ...
  # nrater = number of raters
  
  # determine number of objects
  n <- length(theta)
  # create data frame with unique pairs
  pairmatrix <- lower.tri(matrix(1,n,n))*1
  allpairs <- which(pairmatrix==1,arr.ind=T)[,2:1]
  pairsnot <- names(table(paste(dat[,1],dat[,2])))[table(paste(dat[,1],dat[,2]))==nrater]
  pairs <- allpairs[(paste(allpairs[,1],allpairs[,2]) %in% pairsnot)==0,]
  if (length(dim(pairs))<2) {
    pairs <- t(pairs)
  }
  colnames(pairs) <- c("player1","player2")
  # add theta values of the pairs to the data frame
  pairs <- as.data.frame(cbind(pairs,theta1=theta[pairs[,1]],theta2=theta[pairs[,2]]))
  # compute Zij values
  pairs$Zij <- pairs[,3] - pairs[,4]
  # compute absolute Zij values
  pairs$Zij_abs <- abs(pairs$Zij)
  # select the pair with the lowest absolute Zij values
  pairs_eligible <- pairs[pairs$Zij_abs==min(pairs$Zij_abs),1:2]
  if (length(pairs$Zij_abs==min(pairs$Zij_abs)) > 1) {
    pair <- pairs_eligible[sample(nrow(pairs_eligible),1),]
  } else{
    pair <- pairs_eligible
  }
  return(unlist(pair))
}

#------------------------------------------------------------------------
BSA1 <- function(nstud, ncomp, ptrue, niter=5000, burnin=100, theta_start=rep(0, nstud)) {
  ### NO RESTRICTION UNIQUE COMPARISONS!!! ###
  
  # nstud = number of students/persons to be compared
  # ncomp = mean number of comparisons per student
  # ptrue = matrix of true win probabilities
  
  # compute total number of comparisons
  comptot <- round(nstud * ncomp / 2)
  # initial dat matrix
  dat <- as.data.frame(matrix(NA, comptot, 4))
  names(dat) <- list("player1","player2","win1","win2")
  
  # select first pair
  pair <- selectPair1(theta_start)
  
  # compare the students
  win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
  
  # updata data
  dat[1,] <- c(pair,win1,1-win1)
  
  for (x in 2:comptot) {
    # start student selection
    # print(x)
    # sample theta
    out <- mcmcPCprobit(dat[is.na(dat[,1])==F,], nstud, 
                        niter=niter, burnin=burnin, theta_start = theta_start,
                        prior_mu=prior_mu, prior_sigma=prior_sigma)
    out <- out$theta_log[nrow(out$theta_log),]
    
    pair <- selectPair1(out)
    
    # compare the students
    win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
    
    # updata data
    dat[x,] <- c(pair,win1,1-win1)
  }
  
  # compute posterior means and standard deviations
  out <- mcmcPCprobit(dat, nstud, niter=5000, burnin=burnin, theta_start = theta_start,
                      prior_mu=prior_mu, prior_sigma=prior_sigma)
  out <- cbind(theta_est=out$theta_est,theta_sd=out$theta_sd)
  
  # return result
  return(list(estimates=out, data=dat))
}

#------------------------------------------------------------------------
BSA1restr <- function(nstud, ncomp, nrater, ptrue, 
                      niter=5000, burnin=100, theta_start=rep(0, nstud),
                      prior_mu=0, prior_sigma=1) {
  # nstud = number of students/persons to be compared
  # ncomp = mean number of comparisons per student
  # nrater = number of raters
  # ptrue = matrix of true win probabilities
  
  # compute total number of comparisons
  comptot <- round(nstud * ncomp / 2)
  # initial dat matrix
  dat <- as.data.frame(matrix(NA, comptot, 4))
  names(dat) <- list("player1","player2","win1","win2")
  
  # select first pair
  pair <- selectPair1restr(theta_start,dat,nrater)
  
  # compare the students
  win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
  
  # updata data
  dat[1,] <- c(pair,win1,1-win1)
  
  for (x in 2:comptot) {
    # start student selection
    # print(x)
    # sample theta
    out <- mcmcPCprobit(dat[is.na(dat[,1])==F,], nstud, 
                        niter=niter, burnin=burnin, theta_start = theta_start,
                        prior_mu=prior_mu, prior_sigma=prior_sigma)
    out <- out$theta_log[nrow(out$theta_log),]
    
    pair <- selectPair1restr(out,dat,nrater)
    
    # compare the students
    win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
    
    # updata data
    dat[x,] <- c(pair,win1,1-win1)
  }
  
  # compute posterior means and standard deviations
  out <- mcmcPCprobit(dat, nstud, niter=5000, burnin=burnin, theta_start = theta_start,
                      prior_mu=prior_mu, prior_sigma=prior_sigma)
  out <- cbind(theta_est=out$theta_est,theta_sd=out$theta_sd)
  
  # return result
  return(list(estimates=out, data=dat))
}

#------------------------------------------------------------------------
BSA1restrRater <- function(nstud, ncomp, nrater, ptrue, theta, noise=0, 
                           niter=5000, burnin=100, theta_start=rep(0, nstud),
                           prior_mu=0, prior_sigma=1) {
  # nstud = number of students/persons to be compared
  # ncomp = mean number of comparisons per student
  # nrater = number of raters
  # ptrue = matrix of true win probabilities
  # theta = vector of true thetas
  # noise = one noisy rater yes (1) or no (0)
  
  if (noise == 0) {
    result <- BSA1restr(nstud, ncomp, nrater, ptrue, niter, burnin, theta_start,
                        prior_mu=prior_mu, prior_sigma=prior_sigma)
    # return result
    return(result)
    
  } else {
    # compute total number of comparisons
    comptot <- round(nstud * ncomp / 2)
    # initial dat matrix
    dat <- as.data.frame(matrix(NA, comptot, 4))
    names(dat) <- list("player1","player2","win1","win2")
    
    # compute noisy ptrue # as 'old' ragree = .8
    pnoise <- sqrt(1-.8)
    theta_n <- pnoise*rnorm(nstud,0,1) + (1-pnoise)*theta
    ptrue_n <- matrix(NA,nstud,nstud)
    for (i in 1:nstud) {
      # fill rows with win probabilities, columns then contain loss probabilities
      ptrue_n[i,-i] <- pnorm(theta_n[i]-theta_n[-i])
    }
    
    # select first pair
    pair <- selectPair1restr(theta_start,dat,nrater)
    
    # compare the students
    win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
    
    # updata data
    dat[1,] <- c(pair,win1,1-win1)
    
    for (x in 2:comptot) {
      # start student selection
      # print(x)
      # sample theta
      out <- mcmcPCprobit(dat[is.na(dat[,1])==F,], nstud, 
                          niter=niter, burnin=burnin, theta_start = theta_start,
                          prior_mu=prior_mu, prior_sigma=prior_sigma)
      out <- out$theta_log[nrow(out$theta_log),]
      
      pair <- selectPair1restr(out,dat,nrater)
      
      # select good or noisy rater (1 noisy rater)
      goodrater <- runif(1,0,1) >= 1/nrater
      if (goodrater == T) {
        # compare the students
        win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
      } else {
        # compare the students
        win1 <- 1*(runif(1,0,1) <= ptrue_n[pair[1],pair[2]])
      }
      
      # updata data
      dat[x,] <- c(pair,win1,1-win1)
    }
    
    # compute posterior means and standard deviations
    out <- mcmcPCprobit(dat, nstud, niter=5000, burnin=burnin, theta_start = theta_start,
                        prior_mu=prior_mu, prior_sigma=prior_sigma)
    out <- cbind(theta_est=out$theta_est,theta_sd=out$theta_sd)
    
    # return result
    return(list(estimates=out, data=dat))
  }
}

#-- THT
thtest <- function() {
  #library(data.table)
  nstud <- 30
  theta <- rnorm(nstud,0,1) 
  theta <- theta-mean(theta)
  
  ptrue <- matrix(NA,nstud,nstud)
  for (i in 1:nstud) {
    # fill rows with win probabilities, columns then contain loss probabilities
    ptrue[i,-i] <- pnorm(theta[i]-theta[-i])
  }
  
  theta[1]-theta[-1]
  theta[1]
  
  ncomp<-1
  nrater <-1
  
  res <-BSA2restr(nstud,ncomp,nrater,ptrue, niter=1)
  
  # cat flow
  
  dat <- data.frame(matrix(1,round((nstud*ncomp)/2) , 4))
  names(dat) <- c("player1","player2","win1","win2")
    # compute one theta
  sampleZ(theta, dat)
  
  system.time({
    out <- mcmcPCprobit(na.omit(dat),30,niter=10000)
    theta <- tail(out$theta_log,n=1)
    pair <- selectPair1restr(theta,dat,nrater)
  })
}

iter <- c(10,20,30,50,70,100)
seconds <- c(1,2.6,5,16,38,110)

plot(iter~seconds,type="l")

tmp <- data.frame(iter, seconds)
tmp$pred1 <- predict(lm(seconds ~ poly(iter,3), data=tmp))
library(ggplot2)
p1 <- ggplot(tmp, aes(x=iter, y=seconds)) + geom_line() + geom_point() + geom_hline(aes(yintercept=0))
print(p1)
p1 +  geom_line(aes(y = pred1), color="red")

## extrapolate based on model
pred <- data.frame(iter=10:1000)
pred$seconds <- predict(lm(seconds ~ poly(iter, 3), data=tmp),newdata=pred)
plot(pred, type="l")
p1 +  geom_line(color="red", data=pred)

# aantal uur voor 200 items
pred[191,2] / 3600
#-- /THT

#------------------------------------------------------------------------
BSA2restr <- function(nstud, ncomp, nrater, ptrue, datcol=NULL, 
                      niter=5000, burnin=100, theta_start=rep(0, nstud),
                      prior_mu=0, prior_sigma=1) {
  # nstud = number of students/persons to be compared
  # ncomp = mean number of comparisons per student
  # nrater = number of raters
  # ptrue = matrix of true win probabilities
  # datcol = data frame with collected data
  
  # compute total number of comparisons
  comptot <- round(nstud * ncomp / 2)
  # initial dat matrix
  dat <- as.data.frame(matrix(NA, comptot, 4))
  names(dat) <- list("player1","player2","win1","win2")
  if (is.null(datcol)==F) {
    # compute number of comparisons to be made/collected
    comptocol <- comptot - nrow(datcol)
    if (comptocol == 0) {
      stop("number of comparisons was already reached")
    } else if (comptocol < 0) {
      stop("ERROR: more comparisons already collected than total number of comparisons")
    }
    dat[1:nrow(datcol),] <- datcol
    
    out <- mcmcPCprobit(dat[is.na(dat[,1])==F,], nstud, 
                        niter=niter, burnin=burnin, theta_start = theta_start,
                        prior_mu=prior_mu, prior_sigma=prior_sigma)
    out <- out$theta_log[nrow(out$theta_log),]
    
    pair <- selectPair1restr(out,dat,nrater)
    
    # compare the students
    win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
    
    # updata data
    dat[nrow(datcol)+1,] <- c(pair,win1,1-win1)
  } else {
    # compute number of comparisons to be made/collected
    comptocol <- comptot
    # select first pair
    pair <- selectPair1restr(theta_start,dat,nrater)
    
    # compare the students
    win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
    
    # updata data
    dat[1,] <- c(pair,win1,1-win1)
  }
  
  for (x in 2:comptocol) {
    # start student selection
    # print(x)
    # sample theta
    print(x)
    out <- mcmcPCprobit(dat[is.na(dat[,1])==F,], nstud, 
                        niter=niter, burnin=burnin, theta_start = theta_start,
                        prior_mu=prior_mu, prior_sigma=prior_sigma)
    out <- out$theta_log[nrow(out$theta_log),]
    
    pair <- selectPair1restr(out,dat,nrater)
    
    # compare the students
    win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
    
    # updata data
    dat[sum(is.na(dat[,1])==F)+1,] <- c(pair,win1,1-win1)
  }
  
  # compute posterior means and standard deviations
  out <- mcmcPCprobit(dat, nstud, niter=5000, burnin=burnin, theta_start = theta_start,
                      prior_mu=prior_mu, prior_sigma=prior_sigma)
  out <- cbind(theta_est=out$theta_est,theta_sd=out$theta_sd)
  
  # return result
  return(list(estimates=out, data=dat))
}




#------------------------------------------------------------------------
BSA2restrRater <- function(nstud, ncomp, nrater, ptrue, theta, noise=0, datcol=NULL, 
                           niter=5000, burnin=100, theta_start=rep(0, nstud),
                           prior_mu=0, prior_sigma=1) {
  # nstud = number of students/persons to be compared
  # ncomp = mean number of comparisons per student
  # nrater = number of raters
  # ptrue = matrix of true win probabilities
  # theta = vector of true thetas
  # noise = one noisy rater yes (1) or no (0)
  # datcol = data frame with collected data
  
  if (noise == 0) {
    result <- BSA2restr(nstud, ncomp, nrater, ptrue, datcol, niter, burnin, theta_start,
                        prior_mu=prior_mu, prior_sigma=prior_sigma)
    # return result
    return(result)
    
  } else {
    # compute noisy ptrue # as 'old' ragree = .8
    pnoise <- sqrt(1-.8)
    theta_n <- pnoise*rnorm(nstud,0,1) + (1-pnoise)*theta
    ptrue_n <- matrix(NA,nstud,nstud)
    for (i in 1:nstud) {
      # fill rows with win probabilities, columns then contain loss probabilities
      ptrue_n[i,-i] <- pnorm(theta_n[i]-theta_n[-i])
    }
    
    # compute total number of comparisons
    comptot <- round(nstud * ncomp / 2)
    # initial dat matrix
    dat <- as.data.frame(matrix(NA, comptot, 4))
    names(dat) <- list("player1","player2","win1","win2")
    if (is.null(datcol)==F) {
      # compute number of comparisons to be made/collected
      comptocol <- comptot - nrow(datcol)
      if (comptocol == 0) {
        stop("number of comparisons was already reached")
      } else if (comptocol < 0) {
        stop("ERROR: more comparisons already collected than total number of comparisons")
      }
      dat[1:nrow(datcol),] <- datcol
      
      out <- mcmcPCprobit(dat[is.na(dat[,1])==F,], nstud, 
                          niter=niter, burnin=burnin, theta_start = theta_start,
                          prior_mu=prior_mu, prior_sigma=prior_sigma)
      out <- out$theta_log[nrow(out$theta_log),]
      
      pair <- selectPair1restr(out,dat,nrater)
      
      # compare the students
      win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
      
      # updata data
      dat[nrow(datcol)+1,] <- c(pair,win1,1-win1)
    } else {
      # compute number of comparisons to be made/collected
      comptocol <- comptot
      # select first pair
      pair <- selectPair1restr(theta_start,dat,nrater)
      
      # compare the students
      win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
      
      # updata data
      dat[1,] <- c(pair,win1,1-win1)
    }
    
    for (x in 2:comptocol) {
      # start student selection
      # print(x)
      # sample theta
      out <- mcmcPCprobit(dat[is.na(dat[,1])==F,], nstud, 
                          niter=niter, burnin=burnin, theta_start = theta_start,
                          prior_mu=prior_mu, prior_sigma=prior_sigma)
      out <- out$theta_log[nrow(out$theta_log),]
      
      pair <- selectPair1restr(out,dat,nrater)
      
      # select good or noisy rater (1 noisy rater)
      goodrater <- runif(1,0,1) >= 1/nrater
      if (goodrater == T) {
        # compare the students
        win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
      } else {
        # compare the students
        win1 <- 1*(runif(1,0,1) <= ptrue_n[pair[1],pair[2]])
      }
      
      # updata data
      dat[sum(is.na(dat[,1])==F)+1,] <- c(pair,win1,1-win1)
    }
    
    # compute posterior means and standard deviations
    out <- mcmcPCprobit(dat, nstud, niter=5000, burnin=burnin, theta_start = theta_start,
                        prior_mu=prior_mu, prior_sigma=prior_sigma)
    out <- cbind(theta_est=out$theta_est,theta_sd=out$theta_sd)
    
    # return result
    return(list(estimates=out, data=dat))
  }
}

#------------------------------------------------------------------------
# semi-random selection algorithm (SSA)
SSA <- function(nstud, ncomp, nrater, ptrue, niter=5000, burnin=100, theta_start=rep(0,nstud),
                prior_mu=0, prior_sigma=1) {
  # nstud = number of students/persons to be compared
  # ncomp = mean number of comparisons per student
  # nrater = number of raters
  # ptrue = matrix of true win probabilities
  
  # compute total number of comparisons
  comptot <- round(nstud * ncomp / 2)
  # initial dat matrix
  dat <- as.data.frame(matrix(NA, comptot, 4))
  names(dat) <- list("player1","player2","win1","win2")
  
  # define all unique pairs
  pairmatrix <- lower.tri(matrix(1,nstud,nstud))*1
  allpairs <- which(pairmatrix==1,arr.ind=T)[,2:1]
  
  for (x in 1:comptot) {
    # start student selection
    pairsnot <- names(table(paste(dat[,1],dat[,2])))[table(paste(dat[,1],dat[,2]))==nrater]
    pairs <- allpairs[(paste(allpairs[,1],allpairs[,2]) %in% pairsnot)==0,]
    if (length(dim(pairs))<2) {
      pair <- pairs
    } else {
      pair <- pairs[sample(1:nrow(pairs),1),]
    }
    
    # compare the students
    win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
    
    # updata data
    dat[x,] <- c(pair,win1,1-win1)
  }
  
  # compute posterior means and standard deviations
  out <- mcmcPCprobit(dat, nstud, niter=niter, burnin=burnin, theta_start = theta_start,
                      prior_mu=prior_mu, prior_sigma=prior_sigma)
  out <- cbind(theta_est=out$theta_est,theta_sd=out$theta_sd)
  
  # return result
  return(list(estimates=out, data=dat))
}

#------------------------------------------------------------------------
# semi-random selection algorithm (SSA)
SSArater <- function(nstud, ncomp, nrater, ptrue, theta, noise=0, 
                     niter=5000, burnin=100, theta_start=rep(0, nstud),
                     prior_mu=0, prior_sigma=1) {
  # nstud = number of students/persons to be compared
  # ncomp = mean number of comparisons per student
  # nrater = number of raters
  # ptrue = matrix of true win probabilities
  # theta = vector of true thetas
  # noise = one noisy rater yes (1) or no (0)
  
  if (noise == 0) {
    result <- SSA(nstud, ncomp, nrater, ptrue, niter, burnin, theta_start,
                  prior_mu=prior_mu, prior_sigma=prior_sigma)
    # return result
    return(result)
    
  } else {
    # compute total number of comparisons
    comptot <- round(nstud * ncomp / 2)
    # initial dat matrix
    dat <- as.data.frame(matrix(NA, comptot, 4))
    names(dat) <- list("player1","player2","win1","win2")
    
    # compute noisy ptrue # as 'old' ragree = .8
    pnoise <- sqrt(1-.8)
    theta_n <- pnoise*rnorm(nstud,0,1) + (1-pnoise)*theta
    ptrue_n <- matrix(NA,nstud,nstud)
    for (i in 1:nstud) {
      # fill rows with win probabilities, columns then contain loss probabilities
      ptrue_n[i,-i] <- pnorm(theta_n[i]-theta_n[-i])
    }
    
    # define all unique pairs
    pairmatrix <- lower.tri(matrix(1,nstud,nstud))*1
    allpairs <- which(pairmatrix==1,arr.ind=T)[,2:1]
    
    for (x in 1:comptot) {
      # start student selection
      pairsnot <- names(table(paste(dat[,1],dat[,2])))[table(paste(dat[,1],dat[,2]))==nrater]
      pairs <- allpairs[(paste(allpairs[,1],allpairs[,2]) %in% pairsnot)==0,]
      if (length(dim(pairs))<2) {
        pair <- pairs
      } else {
        pair <- pairs[sample(1:nrow(pairs),1),]
      }
      
      # select good or noisy rater (1 noisy rater)
      goodrater <- runif(1,0,1) >= 1/nrater
      if (goodrater == T) {
        # compare the students
        win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
      } else {
        # compare the students
        win1 <- 1*(runif(1,0,1) <= ptrue_n[pair[1],pair[2]])
      }
      
      # updata data
      dat[x,] <- c(pair,win1,1-win1)
    }
    
    # compute posterior means and standard deviations
    out <- mcmcPCprobit(dat, nstud, niter=5000, burnin=burnin, theta_start = theta_start,
                        prior_mu=prior_mu, prior_sigma=prior_sigma)
    out <- cbind(theta_est=out$theta_est,theta_sd=out$theta_sd)
    
    # return result
    return(list(estimates=out, data=dat))
  }
}

#------------------------------------------------------------------------
# semi-random selection algorithm (SSA)
SSA2 <- function(nstud, ncomp, nrater, ptrue, datcol=NULL, niter=5000, burnin=100, 
                 theta_start=rep(0,nstud), prior_mu=0, prior_sigma=1) {
  # nstud = number of students/persons to be compared
  # ncomp = mean number of comparisons per student
  # nrater = number of raters
  # ptrue = matrix of true win probabilities
  # datcol = data frame with collected data
  
  # compute total number of comparisons
  comptot <- round(nstud * ncomp / 2)
  # initial dat matrix
  dat <- as.data.frame(matrix(NA, comptot, 4))
  names(dat) <- list("player1","player2","win1","win2")
  if (is.null(datcol)==F) {
    # compute number of comparisons to be made/collected
    comptocol <- comptot - nrow(datcol)
    if (comptocol == 0) {
      stop("number of comparisons was already reached")
    } else if (comptocol < 0) {
      stop("ERROR: more comparisons already collected than total number of comparisons")
    }
    dat[1:nrow(datcol),] <- datcol
  } else {
    comptocol <- comptot
  }
  
  # define all unique pairs
  pairmatrix <- lower.tri(matrix(1,nstud,nstud))*1
  allpairs <- which(pairmatrix==1,arr.ind=T)[,2:1]
  
  for (x in 1:comptocol) {
    # start student selection
    pairsnot <- names(table(paste(dat[,1],dat[,2])))[table(paste(dat[,1],dat[,2]))==nrater]
    pairs <- allpairs[(paste(allpairs[,1],allpairs[,2]) %in% pairsnot)==0,]
    if (length(dim(pairs))<2) {
      pair <- pairs
    } else {
      pair <- pairs[sample(1:nrow(pairs),1),]
    }
    
    # compare the students
    win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
    
    # updata data
    dat[sum(is.na(dat[,1])==F)+1,] <- c(pair,win1,1-win1)
  }
  
  # compute posterior means and standard deviations
  out <- mcmcPCprobit(dat, nstud, niter=niter, burnin=burnin, theta_start = theta_start,
                      prior_mu=prior_mu, prior_sigma=prior_sigma)
  out <- cbind(theta_est=out$theta_est,theta_sd=out$theta_sd)
  
  # return result
  return(list(estimates=out, data=dat))
}

#------------------------------------------------------------------------
# semi-random selection algorithm (SSA)
SSA2rater <- function(nstud, ncomp, nrater, ptrue, theta, noise=0, datcol=NULL,
                     niter=5000, burnin=100, theta_start=rep(0, nstud),
                     prior_mu=0, prior_sigma=1) {
  # nstud = number of students/persons to be compared
  # ncomp = mean number of comparisons per student
  # nrater = number of raters
  # ptrue = matrix of true win probabilities
  # theta = vector of true thetas
  # noise = one noisy rater yes (1) or no (0)
  # datcol = data frame with collected data
  
  if (noise == 0) {
    result <- SSA2(nstud, ncomp, nrater, ptrue, datcol, niter, burnin, theta_start,
                   prior_mu=prior_mu, prior_sigma=prior_sigma)
    # return result
    return(result)
    
  } else {
    # compute total number of comparisons
    comptot <- round(nstud * ncomp / 2)
    # initial dat matrix
    dat <- as.data.frame(matrix(NA, comptot, 4))
    names(dat) <- list("player1","player2","win1","win2")
    if (is.null(datcol)==F) {
      # compute number of comparisons to be made/collected
      comptocol <- comptot - nrow(datcol)
      if (comptocol == 0) {
        stop("number of comparisons was already reached")
      } else if (comptocol < 0) {
        stop("ERROR: more comparisons already collected than total number of comparisons")
      }
      dat[1:nrow(datcol),] <- datcol
    } else {
      comptocol <- comptot
    }
    
    # compute noisy ptrue # as 'old' ragree = .8
    pnoise <- sqrt(1-.8)
    theta_n <- pnoise*rnorm(nstud,0,1) + (1-pnoise)*theta
    ptrue_n <- matrix(NA,nstud,nstud)
    for (i in 1:nstud) {
      # fill rows with win probabilities, columns then contain loss probabilities
      ptrue_n[i,-i] <- pnorm(theta_n[i]-theta_n[-i])
    }
    
    # define all unique pairs
    pairmatrix <- lower.tri(matrix(1,nstud,nstud))*1
    allpairs <- which(pairmatrix==1,arr.ind=T)[,2:1]
    
    for (x in 1:comptocol) {
      # start student selection
      pairsnot <- names(table(paste(dat[,1],dat[,2])))[table(paste(dat[,1],dat[,2]))==nrater]
      pairs <- allpairs[(paste(allpairs[,1],allpairs[,2]) %in% pairsnot)==0,]
      if (length(dim(pairs))<2) {
        pair <- pairs
      } else {
        pair <- pairs[sample(1:nrow(pairs),1),]
      }
      
      # select good or noisy rater (1 noisy rater)
      goodrater <- runif(1,0,1) >= 1/nrater
      if (goodrater == T) {
        # compare the students
        win1 <- 1*(runif(1,0,1) <= ptrue[pair[1],pair[2]])
      } else {
        # compare the students
        win1 <- 1*(runif(1,0,1) <= ptrue_n[pair[1],pair[2]])
      }
      
      # updata data
      dat[sum(is.na(dat[,1])==F)+1,] <- c(pair,win1,1-win1)
    }
    
    # compute posterior means and standard deviations
    out <- mcmcPCprobit(dat, nstud, niter=niter, burnin=burnin, theta_start = theta_start,
                        prior_mu=prior_mu, prior_sigma=prior_sigma)
    out <- cbind(theta_est=out$theta_est,theta_sd=out$theta_sd)
    
    # return result
    return(list(estimates=out, data=dat))
  }
}

#------------------------------------------------------------------------
### Scale Separation Reliability (SSR; following Bramley, 2015)
SSR <- function(theta_est, theta_se) {
  # step 1: observed sd of parameter values
  sd_obs <- sd(theta_est)
  # step 2: root mean square error of standard errors
  rmse <- sqrt(mean(theta_se^2))
  # step 3: true sd of parameter values
  if ((sd_obs^2-rmse^2) >= 0) {
    sd_true <- sqrt(sd_obs^2-rmse^2)
  } else {
    sd_true <- 0
  }
  # step 4: reliability
  ssr <- sd_true^2/sd_obs^2
  return(ssr)
}
