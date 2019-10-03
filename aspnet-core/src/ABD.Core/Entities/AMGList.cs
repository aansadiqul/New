using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("AMGList")]
    public class AMGList : AuditedEntity
    {
        [StringLength(50)]
        public string AList { get; set; }
        public bool? IsActive { get; set; }
    }
}
