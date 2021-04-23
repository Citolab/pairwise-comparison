using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Cito.Cat.Algorithms.Pairwise.Models
{
    public class PairwiseSessionState
    {
        public string CatSessionIdentifier { get; set; }

        //public int ComparisonTotal { get; set; }

        public List<PairsData> PairsData { get; set; }
        
        /// <summary>
        /// Last set of thetas
        /// </summary>
        public double[] ThetaLast { get; set; }

        public double[][] PosteriorEstimates { get; set; }

        public PairwiseSessionState()
        {
            PairsData = new List<PairsData>();
        }
    }
}