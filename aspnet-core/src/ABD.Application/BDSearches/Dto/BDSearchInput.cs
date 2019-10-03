using System;
using System.Collections.Generic;
using System.Text;
using Abp.AutoMapper;
using ABD.Entities;
namespace ABD.BDSearches.Dto
{
    [AutoMapTo(typeof(BDSearch))]
    public class BDSearchInput
    {        
        public string SearchName { get; set; }
        public bool IsNewQuery { get; set; }
        public int? OriginalCount { get; set; }        
        public string CompanyName { get; set; }      
        public string CompanyBeginsContains { get; set; }
        public decimal? EmployeesFrom { get; set; }
        public decimal? EmployeesTo { get; set; }       
        public string CompanyAndOr { get; set; }
        public decimal? SalesFrom { get; set; }
        public decimal? SalesTo { get; set; }        
        public string Locations { get; set; }       
        public string Manufacturing { get; set; }        
        public string State { get; set; }
        public string IncludeCountyIds { get; set; }
        public string County { get; set; }
        public string Zip { get; set; }
        public string MSA { get; set; }
        public string AreaCode { get; set; }
        public string SIC { get; set; }
        public string SICIDs { get; set; }       
        public string XDatesMonth { get; set; }
        public string IL_SlectedStates { get; set; }
        public string IL_EXSlectedStates { get; set; }
        public string GetAllStates { get; set; }
        public string Exclude_State { get; set; }
        public string Exclude_County { get; set; }
        public string Exclude_Zip { get; set; }
        public string Exclude_MSA { get; set; }
        public string Exclude_AreaCode { get; set; }
        public string SQLQuery { get; set; }      
        public string Queryfilename { get; set; }
        public long? UserId { get; set; }
    }
}
