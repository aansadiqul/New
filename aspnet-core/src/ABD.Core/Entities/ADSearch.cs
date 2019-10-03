using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ABD.Authorization.Users;

namespace ABD.Entities
{
    public class ADSearch : FullAuditedEntity
    {
        [Column("QueryID")]
        [Key]
        public override int Id { get; set; }
        [StringLength(255)]
        public string QueryName { get; set; }
        public string TypeField { get; set; }
        public string TypeCriteria { get; set; }
        [StringLength(255)]
        public string PVolume { get; set; }
        [StringLength(255)]
        public string EmpSize { get; set; }
        [StringLength(255)]
        public string PEmpCriteria { get; set; }
        [StringLength(255)]
        public string CommPercent { get; set; }
        public string AgencyManagement { get; set; }
        [StringLength(255)]
        public string AgencyMgntCriteria { get; set; }
        public string CompanyLines { get; set; }
        [StringLength(255)]
        public string Country { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string SICCodes { get; set; }
        public bool? Active { get; set; }
        public string TitleSearch { get; set; }
        [StringLength(255)]
        public string TitleSearchCriteria { get; set; }
        public string LinesSearch { get; set; }
        [StringLength(255)]
        public string LinesSearchCriteria { get; set; }
        public string MSA { get; set; }
        public string msaConcatenated { get; set; }
        public string countyConcatenated { get; set; }
        public string Affiliations { get; set; }
        public string ExcludeState { get; set; }
        public string ExcludeCountiesList { get; set; }
        public string Zip { get; set; }
        public string ExcludeZip { get; set; }
        public string AreaCode { get; set; }
        public string ExcludeAreaCode { get; set; }
        public string ExcludeMSA { get; set; }
        public string SICIDS { get; set; }
        public string CountyIDs { get; set; }
        public string ExcludeCountyIds { get; set; }
        [StringLength(255)]
        public string RevenueCriteria { get; set; }
        [StringLength(255)]
        public string RevenueValue { get; set; }
        public string QueryCriteria { get; set; }
        [StringLength(255)]
        public string CompanyName { get; set; }
        [StringLength(255)]
        public string CompanyNameType { get; set; }

        [StringLength(2)]
        public string MinorityOwned { get; set; }

        [StringLength(255)]
        public string CarrierManageCrieteria { get; set; }
    }
}
