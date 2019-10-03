using System;
using System.Collections.Generic;
using System.Text;
using ABD.Domain.Dtos;

namespace ABD.ADSearches.Dto
{
    public class AdBuyNames
    {
        public RecordPriceDto RecordPrice { get; set; }
        public List<ADName> ADNames { get; set; }
    }
}
