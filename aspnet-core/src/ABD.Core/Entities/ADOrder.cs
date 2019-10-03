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
    public class ADOrder : FullAuditedEntity<int>
    {
        [Column("OrderID")]
        [Key]
        public override int Id { get; set; }
        public int? OrderType { get; set; }
        public int? TranType { get; set; }
        [StringLength(64)]
        public string SUserId { get; set; }
        [StringLength(128)]
        public string Description { get; set; }
        public int? RecordCount { get; set; }
        [StringLength(255)]
        public string BillFName { get; set; }
        [StringLength(255)]
        public string BillLName { get; set; }
        [StringLength(255)]
        public string BillCompany { get; set; }
        [StringLength(255)]
        public string BillAddress { get; set; }
        [StringLength(255)]
        public string BillCity { get; set; }
        [StringLength(255)]
        public string BillState { get; set; }
        [StringLength(255)]
        public string BillCountry { get; set; }
        [StringLength(255)]
        public string BillZip { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? OrderValue { get; set; }
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
        public string CompanyLines { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string SICCodes { get; set; }
        public string AccountIds { get; set; }
        [StringLength(255)]
        public string TransactionID { get; set; }
        public bool? Active { get; set; }
        public bool? IsCtPurchased { get; set; }
        public int? CtCount { get; set; }
        public string TitleSearch { get; set; }
        public string TitleSearchCriteria { get; set; }
        public string LinesSearch { get; set; }
        public string LinesSearchCriteria { get; set; }
        public string MSA { get; set; }
        public string OrderNotes { get; set; }
        public string CardNumber { get; set; }
        public string ACHAccountName { get; set; }
        public string ACHRouting { get; set; }
        public string ACHAccount { get; set; }
        public string ACHCheck { get; set; }
        public string AgencyMgntCriteria { get; set; }
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
        public string RevenueValue { get; set; }
        public int? QueryID { get; set; }
        public string OverRideNotes { get; set; }
        [StringLength(255)]
        public string CheckNo { get; set; }
        [StringLength(255)]
        public string SalesUser { get; set; }
        public string QueryCriteria { get; set; }
    }
}
