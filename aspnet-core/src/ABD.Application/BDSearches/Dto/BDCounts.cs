using System;
using System.Collections.Generic;
using System.Text;
using ABD.Domain.Dtos;
namespace ABD.BDSearches.Dto
{
    public class BDCounts
    {
        public long BusinessListCount { get; set; }
        public long BDXDateListCount { get; set; }
        public List<XDateBreakdownDto> BDXDateBreakDown { get; set; }
    }
}
