using System;
using Ims.Cat.Models;

namespace Cito.Cat.Core.Models.Section
{
    public class CatSection
    {
        public string Id { get; set; }

        public QTIMetadataDType QtiMetadata { get; set; }

        public string QtiUsagedata { get; set; }

        public string SectionConfiguration { get; set; }

        public string Hash { get; set; }
    }
}