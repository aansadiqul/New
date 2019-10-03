using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ABD.ADSearches.Dto
{
    public class AnalyzeInput
    {
        public string AgencyQuery { get; set; }
        public string FirstValue { get; set; }
        public string SecondValue { get; set; }
        public string SicCodes { get; set; }
    }
}
