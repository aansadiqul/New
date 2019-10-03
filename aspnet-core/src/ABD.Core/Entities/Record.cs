using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Castle.MicroKernel.SubSystems.Conversion;

namespace ABD.Entities
{
    [Table("RECORDS")]
    public class Record : Entity<string>
    {
        [Column("DunsNumber", TypeName = "char(12)")]
        [Key]
        public override string Id { get; set; }
        [StringLength(50)]
        public string Company { get; set; }
        [StringLength(50)]
        public string TradeName { get; set; }
        [StringLength(50)]
        public string Address { get; set; }
        [StringLength(50)]
        public string City { get; set; }
        [StringLength(2)]
        public string State { get; set; }
        [StringLength(9)]
        public string Zip { get; set; }
        [StringLength(2)]
        public string StateCode { get; set; }
        [StringLength(3)]
        public string CountyCode { get; set; }
        [StringLength(4)]
        public string CityCode { get; set; }
        [StringLength(3)]
        public string SMSACode { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(50)]
        public string CEOName { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(2)]
        public string MiddleInitial { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(3)]
        public string Suffix { get; set; }
        [StringLength(10)]
        public string Prefix { get; set; }
        [StringLength(50)]
        public string Title { get; set; }
        [StringLength(4)]
        public string MRCCode { get; set; }
        [StringLength(1)]
        public string BranchIndicator { get; set; }
        [StringLength(1)]
        public string Manufacturing { get; set; }
        [StringLength(1)]
        public string PopulationCode { get; set; }
        [StringLength(20)]
        public string LineOfBusiness { get; set; }
        [StringLength(20)]
        public string SIC1 { get; set; }
        [StringLength(20)]
        public string SalesVolume { get; set; }
        [StringLength(20)]
        public string EmployeesTotal { get; set; }
        [StringLength(20)]
        public string EmployeesHere { get; set; }
        [StringLength(20)]
        public string SIC { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        public decimal? Employees { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        public decimal? Sales { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        public decimal? AreaCode { get; set; }
        [StringLength(20)]
        public string StateCounty { get; set; }
        [StringLength(100)]
        public string CountyName { get; set; }
        [StringLength(200)]
        public string WorkersCompCarrier { get; set; }
        [StringLength(11)]
        public string DunsNumberFM { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        public decimal? WorkersCompMonth { get; set; }
        public DateTime? WorkersCompXDate { get; set; }
        [StringLength(10)]
        public string SIC8 { get; set; }
        [StringLength(100)]
        public string SIC8Description { get; set; }
        [StringLength(3)]
        public string Zip3 { get; set; }
        [StringLength(5)]
        public string Zip5 { get; set; }
        [StringLength(100)]
        public string SIC6Desc{ get; set; }
        [StringLength(6)]
        public string SIC6 { get; set; }
        [StringLength(5)]
        public string SMSAStateCode { get; set; }
        [StringLength(50)]
        public string Latitude { get; set; }
        [StringLength(50)]
        public string Longitude { get; set; }
    }
}
