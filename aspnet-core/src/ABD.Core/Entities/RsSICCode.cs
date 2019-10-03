using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("RS_SICCodes")]
    public class RsSICCode : Entity
    {
        [StringLength(20)]
        public string SICCodeId { get; set; }
        public int? PositionCount { get; set; }
        [StringLength(50)]
        public string SIC { get; set; }
        [StringLength(50)]
        public string ParentId { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
    }
}
