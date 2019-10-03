using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ABD.ADSearches.Dto
{
    public class ADCountsDto
    {
        public ADCounts ADCounts { get; set; }
        public ADQueries ADQueries { get; set; }
    }
}
