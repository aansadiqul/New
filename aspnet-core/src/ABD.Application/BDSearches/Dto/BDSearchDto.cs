using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using ABD.Authorization.Users;
using ABD.Entities;

namespace ABD.BDSearches.Dto
{
    [AutoMapFrom(typeof(BDSearch))]
    public class BDSearchDto : FullAuditedEntityDto
    {
        [StringLength(100)]
        public string SearchName { get; set; }
        public int? OriginalCount { get; set; }
        [StringLength(100)]
        public string CompanyName { get; set; }
        [StringLength(50)]
        public string CompanyBeginsContains { get; set; }
        public decimal? EmployeesFrom { get; set; }
        public decimal? EmployeesTo { get; set; }
        [StringLength(50)]
        public string CompanyAndOr { get; set; }
        public decimal? SalesFrom { get; set; }
        public decimal? SalesTo { get; set; }
        [StringLength(500)]
        public string Locations { get; set; }
        [StringLength(500)]
        public string Manufacturing { get; set; }
        [StringLength(3000)]
        public string State { get; set; }
        public string County { get; set; }
        public string Zip { get; set; }
        public string MSA { get; set; }
        public string AreaCode { get; set; }
        public string SIC { get; set; }
        public string SICIDs { get; set; }
        [StringLength(100)]
        public string XDatesMonth { get; set; }
        public string Exclude_State { get; set; }
        public string Exclude_County { get; set; }
        public string Exclude_Zip { get; set; }
        public string Exclude_MSA { get; set; }
        public string Exclude_AreaCode { get; set; }
        public string SQLQuery { get; set; }
        [StringLength(255)]
        public string Queryfilename { get; set; }
        public long? UserId { get; set; }
    }
}
