using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("RS_MSAs")]
    public class MetroArea : Entity
    {
        [StringLength(5)]
        public string Code { get; set; }
        [StringLength(2)]
        public string State { get; set; }
        [StringLength(128)]
        public string Name { get; set; }
    }
}
