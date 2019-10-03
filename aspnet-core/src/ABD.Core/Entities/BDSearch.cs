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
    public class BDSearch : FullAuditedEntity<int>
    {
        [Column("SearchID")]
        [Key]
        public override int Id { get; set; }
        [StringLength(100)]
        public string SearchName { get; set; }
        public int? OriginalCount { get; set; }
        [StringLength(100)]
        public string CompanyName { get; set; }
        [StringLength(50)]
        public string CompanyBeginsContains { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? EmployeesFrom { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? EmployeesTo { get; set; }
        [StringLength(50)]
        public string CompanyAndOr { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? SalesFrom { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
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
    }
}
