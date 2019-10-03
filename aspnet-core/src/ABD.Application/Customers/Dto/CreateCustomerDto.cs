using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ABD.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Authorization.Users;
using Abp.Auditing;
using Abp.Runtime.Validation;

namespace ABD.Customers
{
    [AutoMapTo(typeof(UserProfile))]
    public class CreateCustomerDto
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
        [StringLength(MaxLength)]
        public string City { get; set; }
        [StringLength(MaxLength)]
        public string State { get; set; }
        [Required]
        [StringLength(MinLength)]
        public string Country { get; set; }
        [Required]
        [StringLength(MinLength)]
        public string Zip { get; set; }
        [Required]
        [StringLength(MinLength)]
        public string Email { get; set; }
        [Required]
        [StringLength(MinLength)]
        public string Telephone { get; set; }
        [StringLength(MinLength)]
        public string Fax { get; set; }
        public string SUSERID { get; set; }
        public DateTime? ADActiveDate { get; set; }
        public DateTime? ADExpiresDate { get; set; }
        public DateTime? BDActiveDate { get; set; }
        public string ImagePath { get; set; }
        public bool? IsSalesPerson { get; set; }
        public int? SubType { get; set; }
        [StringLength(400)]
        public string NOTES { get; set; }
        public int? AgencyRec { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? AgencyRecPrice { get; set; }
        public int? ContactRec { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? ContactRecPrice { get; set; }
    }
}
