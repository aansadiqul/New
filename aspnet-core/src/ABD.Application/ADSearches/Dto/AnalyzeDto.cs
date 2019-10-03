using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ABD.ADSearches.Dto
{
    public class AnalyzeDto
    {
        public string FirstValue { get; set; }
        public string SecondValue { get; set; }
        public string Description { get; set; }
        public string Records { get; set; }
    }
}
