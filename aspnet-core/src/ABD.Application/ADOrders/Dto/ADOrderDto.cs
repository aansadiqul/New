using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ABD.Entities;

namespace ABD.ADOrders.Dto
{
    public class ADOrderDto : FullAuditedEntityDto
    {
        public int? OrderType { get; set; }
        public int? TranType { get; set; }
        public string SUserId { get; set; }
        public string Description { get; set; }
        public int? RecordCount { get; set; }
        public string BillFName { get; set; }
        public string BillLName { get; set; }
        public string BillCompany { get; set; }
        public string BillAddress { get; set; }
        public string BillCity { get; set; }
        public string BillState { get; set; }
        public string BillCountry { get; set; }
        public string BillZip { get; set; }
        public decimal? OrderValue { get; set; }
        public string TypeField { get; set; }
        public string TypeCriteria { get; set; }
        public string PVolume { get; set; }
        public string EmpSize { get; set; }
        public string PEmpCriteria { get; set; }
        public string CommPercent { get; set; }
        public string AgencyManagement { get; set; }
        public string CompanyLines { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string SICCodes { get; set; }
        public string AccountIds { get; set; }
        public string TransactionID { get; set; }
        public bool? Active { get; set; }
        public bool IsCtPurchased { get; set; }
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
        public string RevenueCriteria { get; set; }
        public string RevenueValue { get; set; }
        public int? QueryID { get; set; }
        public string OverRideNotes { get; set; }
        public string CheckNo { get; set; }
        public string SalesUser { get; set; }
        public string QueryCriteria { get; set; }
    }
}
