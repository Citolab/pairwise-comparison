namespace Cito.Cat.Pairwise.Web.Models
{
    public class TestSessionModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string StartCode { get; set; }
        public TestStatus Status { get; set; }
        public int ComparisonsTotal { get; set; }
        public int ComparisonsDone { get; set; }
    }
}