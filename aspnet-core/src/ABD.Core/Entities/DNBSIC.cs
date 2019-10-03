using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("RS_DNB_SICs")]
    public class DNBSIC : Entity
    {
        [StringLength(10)]
        public string SIC { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
    }
}
