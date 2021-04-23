namespace Ims.Cat.Models
{
    /// <summary>
    /// Includes some extra Cito-specific field that will be moved to OutcomeVariables.
    /// </summary>
    public partial class SubmitResultsResponseBodyDType
    {
        #region TODO: Put these in OutcomeVariables!

        /// <summary>
        /// Cito-specific addition. Will be moved to an OutcomeVariable.
        /// </summary>
        public double EstimatedAbility { get; set; }

        /// <summary>
        /// Cito-specific addition. Will be moved to an OutcomeVariable.
        /// </summary>
        public double StandardError { get; set; }

        /// <summary>
        /// Cito-specific addition. Will be moved to an OutcomeVariable.
        /// </summary>
        public double BankPercentage { get; set; }

        /// <summary>
        /// Cito-specific addition. Will be moved to an OutcomeVariable.
        /// </summary>
        public bool StopConditionIsMet { get; set; }

        /// <summary>
        /// Cito-specific addition. Will be moved to an OutcomeVariable.
        /// </summary>
        public string StopDescription { get; set; }

        #endregion
    }
}