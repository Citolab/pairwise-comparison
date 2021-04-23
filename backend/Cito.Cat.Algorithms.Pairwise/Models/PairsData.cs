namespace Cito.Cat.Algorithms.Pairwise.Models
{
    public struct PairsData
    {
        public int Player1 { get; set; }
        public int Player2 { get; set; }
        public int Win1 { get; set; }
        public int Win2 { get; set; }
        /// <summary>
        /// Duration in seconds
        /// </summary>
        public long Duration { get; set; }
        public double? Zij { get; set; }
    }
}