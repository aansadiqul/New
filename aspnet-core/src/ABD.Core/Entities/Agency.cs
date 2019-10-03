using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("AgencyDB")]
    public class Agency : Entity
    {
        [StringLength(128)]
        public string Account { get; set; }
        [StringLength(128)]
        public string Address1 { get; set; }
        [StringLength(128)]
        public string Address2 { get; set; }
        [StringLength(128)]
        public string City { get; set; }
        [StringLength(128)]
        public string State { get; set; }
        [StringLength(128)]
        public string PostalCode { get; set; }
        [StringLength(128)]
        public string County { get; set; }
        [StringLength(128)]
        public string TimeZone { get; set; }
        [StringLength(128)]
        public string Country { get; set; }
        [StringLength(128)]
        public string Division { get; set; }
        [StringLength(128)]
        public string MainPhone { get; set; }
        [StringLength(128)]
        public string PhoneExtension { get; set; }
        [StringLength(128)]
        public string Fax { get; set; }
        [StringLength(128)]
        public string TollFree { get; set; }
        [StringLength(128)]
        public string WebAddress { get; set; }
        [StringLength(128)]
        public string Email { get; set; }
        [StringLength(128)]
        public string Type { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? Revenue { get; set; }
        public int? PercentComm { get; set; }
        public int? Employees { get; set; }
        [StringLength(128)]
        public string SpecialAffiliation { get; set; }
        [StringLength(128)]
        public string AgencyManagement { get; set; }
        [StringLength(128)]
        public string Producer { get; set; }
        [StringLength(128)]
        public string LastName { get; set; }
        [StringLength(128)]
        public string FirstName { get; set; }
        [StringLength(128)]
        public string Suffix { get; set; }
        [StringLength(128)]
        public string Title { get; set; }
        [StringLength(128)]
        public string Mobile { get; set; }
        [StringLength(128)]
        public string CEmail { get; set; }
        [Required]
        [Column(TypeName = "char(12)")]
        public string AccountId { get; set; }
        [StringLength(128)]
        public string SpLines { get; set; }
        [StringLength(128)]
        public string TitleSearch { get; set; }
        [StringLength(128)]
        public string LineSearch { get; set; }
        [StringLength(128)]
        public string Msa { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public double? PremiumVolume { get; set; }
        [Column(TypeName = "char(12)")]
        public string DunsNum { get; set; }
        [StringLength(128)]
        public string CEmail2 { get; set; }
        [StringLength(128)]
        public string LinkedUrl { get; set; }
        [StringLength(128)]
        public string BranchIndicator { get; set; }
        public int? Num_Locations { get; set; }
        [StringLength(128)]
        public string TwitterUrl { get; set; }
        [StringLength(128)]
        public string FacebookUrl { get; set; }
        [StringLength(128)]
        public string GoogleplusUrl { get; set; }
        [StringLength(128)]
        public string YoutubeUrl { get; set; }
        [StringLength(128)]
        public string Linkedin { get; set; }
        [StringLength(128)]
        public string CountyCode { get; set; }
        [StringLength(128)]
        public string CTwitterUrl { get; set; }
        [StringLength(128)]
        public string CFacebookUrl { get; set; }
        [StringLength(128)]
        public string CGoogleplusUrl { get; set; }
        [StringLength(128)]
        public string CYoutubeUrl { get; set; }
        [StringLength(128)]
        public string CLinkedin { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public int? GeoCodeStatus { get; set; }
        
    }
}
