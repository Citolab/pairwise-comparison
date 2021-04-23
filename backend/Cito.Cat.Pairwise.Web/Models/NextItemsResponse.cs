using System.Collections.Generic;

namespace Cito.Cat.Pairwise.Web.Models
{
    public class NextItemsResponse
    {
        public Dictionary<string,Item> NextItems { get; set; }
        public int ComparisonsTotal { get; set; }
        public int ComparisonsDone { get; set; }
    }
}