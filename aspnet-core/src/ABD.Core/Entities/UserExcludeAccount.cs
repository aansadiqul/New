using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("UserExcludeAccounts")]
    public class UserExcludeAccount : Entity
    {
        public int? UserId { get; set; }
        [Column(TypeName = "char(12)")]
        public string AccountId { get; set; }
       
    }
}
