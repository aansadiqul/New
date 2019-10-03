using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("RS_States")]
    public class RsState : Entity
    {
        [StringLength(10)]
        public string State { get; set; }
        [StringLength(50)]
        public string Description { get; set; }
    }
}
