using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("TargetSectorsDB")]
    public class TargetSector : Entity
    {
        [Required]
        [Column(TypeName = "char(12)")]
        public string AccountId { get; set; }
        [StringLength(128)]
        public string Account { get; set; }
        [StringLength(32)]
        public string SicCode { get; set; }
        [StringLength(128)]
        public string Description { get; set; }
    }
}
