using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("ClinesDB")]
    public class Carrier : Entity
    {
        [Column(TypeName = "char(12)")]
        public string AccountId { get; set; }
        [StringLength(100)]
        public string Account { get; set; }
        [StringLength(100)]
        public string CompanyLine { get; set; }
    }
}
