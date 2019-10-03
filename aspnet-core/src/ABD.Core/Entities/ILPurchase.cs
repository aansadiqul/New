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
    [Table("IL_Purchased")]
    public class BdPurchasedData : CreationAuditedEntity
    {
        [Required]
        public int OrderID { get; set; }
        [StringLength(20)]
        public string DUNSNumber { get; set; }
        [StringLength(50)]
        public string RecordType { get; set; }
    }
}
