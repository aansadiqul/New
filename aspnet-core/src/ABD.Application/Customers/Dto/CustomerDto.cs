using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ABD.Entities;
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text;
using Abp.Authorization.Users;
using Abp.Auditing;

namespace ABD.Customers.Dto
{
    [AutoMapFrom(typeof(UserProfile))]
    public class CustomerDto : FullAuditedEntityDto
    {
        public const int MaxLength = 255;
        public const int MinLength = 128;

        public long UserId { get; set; }       
        [Required]
        [StringLength(MaxLength)]
        public string FName { get; set; }
        [Required]
        [StringLength(MaxLength)]
        public string LName { get; set; }
        [StringLength(MaxLength)]
        public string CompanyName { get; set; }
        [Required]
        [StringLength(MaxLength)]
        public string Address { get; set; }
        [Required]
        [StringLength(MinLength)]
        public string City { get; set; }
        [StringLength(MinLength)]
        public string State { get; set; }
        [Required]
        [StringLength(MinLength)]
        public string Country { get; set; }
        [Required]
        [StringLength(MinLength)]
        public string Zip { get; set; }
        [Required]
        [StringLength(MinLength)]
        public string Telephone { get; set; }
        [StringLength(MinLength)]
        public string Fax { get; set; }
        [StringLength(MinLength)]
        public string Mobile { get; set; }
        [Required]
        [StringLength(MinLength)]
        public string Email { get; set; }
        [StringLength(MinLength)]
        public string Web { get; set; }
        [StringLength(MaxLength)]
        public string BillFName { get; set; }
        [StringLength(MaxLength)]
        public string BillLName { get; set; }
        [StringLength(MaxLength)]
        public string BillCompany { get; set; }
        [StringLength(MaxLength)]
        public string BillAddress { get; set; }
        [StringLength(MinLength)]
        public string BillCity { get; set; }
        [StringLength(MinLength)]
        public string BillState { get; set; }
        [StringLength(MinLength)]
        public string BillCountry { get; set; }
        [StringLength(MinLength)]
        public string BillZip { get; set; }
        public bool? Active { get; set; }
        [StringLength(MaxLength)]
        public string SUSERID { get; set; }
        public DateTime? ADActiveDate { get; set; }
        public DateTime? ADExpiresDate { get; set; }
        public int? ByPassCard { get; set; }
        public bool? TCAgreed { get; set; }
        public DateTime? TCDate { get; set; }
        public DateTime? BDActiveDate { get; set; }
        public int? BDCredits { get; set; }
        public DateTime? CCActiveDate { get; set; }
        public DateTime? CCExpiresDate { get; set; }
        public int? CCAmount { get; set; }
        public int? SubType { get; set; }
        public bool? QUpdates { get; set; }
        [StringLength(400)]
        public string NOTES { get; set; }
        public int? AgencyRec { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? AgencyRecPrice { get; set; }
        public int? ContactRec { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? ContactRecPrice { get; set; }
        public string ImagePath { get; set; }
        public bool? IsSalesPerson { get; set; }
    }
}
