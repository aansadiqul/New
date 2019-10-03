using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    public class SubscriptionPlan : AuditedEntity
    {
        public string TypeName { get; set; }
        public int? Amount { get; set; }
        public string Options { get; set; }
        public string AccessLevel { get; set; }
    }
}
