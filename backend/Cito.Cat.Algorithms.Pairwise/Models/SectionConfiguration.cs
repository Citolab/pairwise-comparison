using System.Collections.Generic;
using System.Linq;
using Cito.Cat.Core.Interfaces;

namespace Cito.Cat.Algorithms.Pairwise.Models
{
    public class SectionConfiguration: ISectionConfiguration
    {
        /// <summary>
        /// Title of this section.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Number of items in the section/
        /// </summary>
        public int ItemCount => ItemIds.Length;

        /// <summary>
        /// Number of pairwise comparisons that need to be made for each item.
        /// </summary>
        public int ComparisonsPerItem { get; set; }

        /// <summary>
        /// Number of mcmc sampling iterations after each comparison. 
        /// </summary>
        public int SamplingIterations { get; set; } = 1;

        /// <summary>
        /// Total number of comparisons to be made.
        /// </summary>
        public int ComparisonsTotal { get; set; }

        /// <summary>
        /// Initial theta values
        /// </summary>
        public double[] ThetaStart { get; set; }

        /// <summary>
        /// Prior mean.
        /// </summary>
        public double PriorMu { get; set; }

        /// <summary>
        /// Prior SD.
        /// </summary>
        public double PriorSigma { get; set; } = 1;

        /// <summary>
        /// Item identifiers.
        /// </summary>
        public string[] ItemIds { get; set; }

        public SectionConfiguration()
        {
            ItemIds = new string[] { };
            ThetaStart = Enumerable.Repeat(0d, ItemCount).ToArray();
        }
    }
}