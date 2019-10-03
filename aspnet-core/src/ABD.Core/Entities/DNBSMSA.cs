using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("RS_DNB_SMSAs")]
    public class DNBSMSA : Entity
    {
        [Column("SMSAID")]
        [Key]
        public override int Id { get; set; }
        [StringLength(10)]
        public string SMSACode { get; set; }
        [StringLength(64)]
        public string SMSA { get; set; }
        [StringLength(64)]
        public string State { get; set; }
        [StringLength(64)]
        public string SMSAStateCode{ get; set; }
    }
}
