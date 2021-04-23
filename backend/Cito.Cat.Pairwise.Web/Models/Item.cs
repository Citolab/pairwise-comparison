namespace Cito.Cat.Pairwise.Web.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string Category { get; set; }
        public string EstimatedReadingLevel { get; set; }
        public string Source { get; set; }
        public string License { get; set; }
        public int MeijerinkOrigineel { get; set; }
        public int LeesindexAOrigineel { get; set; }
        public int MeijerinkIngekort { get; set; }
        public int LeesindexAIngekort { get; set; }
        
        public string Text { get; set; }

        public Item()
        {
        }

        public Item(string text)
        {
            Text = text;
        }

        public Item(string id, string text) : this(text)
        {
            Id = id;
        }
    }
}