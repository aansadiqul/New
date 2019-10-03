using System;
using System.Collections.Generic;
using System.Text;
using Abp.AutoMapper;
using ABD.Entities;

namespace ABD.ADSearches.Dto
{
    [AutoMapTo(typeof(ADSearch))]
    public class ADSearchInput
    {
        public string QueryCriteria { get; set; }
        public string QueryName { get; set; }
        public string Affiliations { get; set; }
        public string AgencyManagement { get; set; }
        public string AgencyMgntCriteria { get; set; }
        public string AreaCode { get; set; }
        public string CommPercent { get; set; }
        public string CompanyLines { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNameType { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public string countyConcatenated { get; set; }
        public string CountyIDs { get; set; }
        public string EmpSize { get; set; }
        public string ExcludeAreaCode { get; set; }
        public string ExcludeCountiesList { get; set; }
        public string ExcludeCountyIds { get; set; }
        public string ExcludeMSA { get; set; }
        public string ExcludeState { get; set; }
        public string ExcludeZip { get; set; }
        public string IncludeCountyIds { get; set; }
        public string LinesSearch { get; set; }
        public string LinesSearchCriteria { get; set; }
        public string MSA { get; set; }
        public string msaConcatenated { get; set; }
        public string PEmpCriteria { get; set; }
        public string PVolume { get; set; }
        public string RevenueCriteria { get; set; }
        public string RevenueValue { get; set; }
        public string SICCodes { get; set; }
        public string SICIDS { get; set; }
        public string State { get; set; }
        public string TitleSearch { get; set; }
        public string TitleSearchCriteria { get; set; }
        public string TypeCriteria { get; set; }
        public string TypeField { get; set; }
        public string Zip { get; set; }
        public bool? isRetail { get; set; }
        public bool? isWholesale { get; set; }

        public string MinorityOwned  { get; set; }


        public string CarrierManageCrieteria { get; set; }


    }
}
