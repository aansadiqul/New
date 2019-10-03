using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("RS_AreaCodes")]
    public class Area : Entity
    {
        [StringLength(2)]
        public string State { get; set; }
        [StringLength(10)]
        public string AreaCode { get; set; }
    }
}
