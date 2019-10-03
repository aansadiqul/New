using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("PricingRules")]
    public class PricingRule : AuditedEntity
    {
        public string Title { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PerCreditRate { get; set; }
    }
}
