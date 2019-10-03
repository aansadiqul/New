using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("RS_Counties")]
    public class County : Entity
    {
        [StringLength(2)]
        public string State { get; set; }
        [StringLength(10)]
        public string CountyCode { get; set; }
        [StringLength(30)]
        public string Description { get; set; }
        [StringLength(10)]
        public string CountyCodeActual { get; set; }
    }
}
