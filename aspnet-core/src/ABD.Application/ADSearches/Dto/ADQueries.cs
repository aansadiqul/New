using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ABD.ADSearches.Dto
{
    public class ADQueries
    {
        public string AgencyQuery { get; set; }
        public string SQLQuery { get; set; }
        public string ADCountQuery { get; set; }
        public string ADContactsCountQuery { get; set; }
        public string ADEmailCountQuery { get; set; }
    }
}
