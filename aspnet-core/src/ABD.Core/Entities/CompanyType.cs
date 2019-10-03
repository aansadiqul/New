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
    public class CompanyType : AuditedEntity
    {
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsRetail { get; set; }
        public bool? IsWholesale { get; set; }
    }
}
