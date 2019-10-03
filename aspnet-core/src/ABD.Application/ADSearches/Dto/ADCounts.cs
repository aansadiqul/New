using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ABD.ADSearches.Dto
{
    public class ADCounts
    {
        public string AgencyListCount { get; set; }
        public string ADContactsCount { get; set; }
        public string ADEmailCount { get; set; }
    }
}
