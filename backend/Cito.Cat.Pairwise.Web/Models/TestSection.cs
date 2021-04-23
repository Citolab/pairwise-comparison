using System.Collections.Generic;

namespace Cito.Cat.Pairwise.Web.Models
{
    public class TestSection
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string CatSectionId { get; set; }
        public Dictionary<string,Item> Items { get; set; }
    }
}