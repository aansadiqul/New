using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("RS_DNB_FIPSCOUNTYCODES")]
    public class DNBFIPSCountyCode : Entity
    {
        [StringLength(2)]
        public string State { get; set; }
        [StringLength(2)]
        public string StateCode { get; set; }
        [StringLength(2)]
        public string FIPSStateCode { get; set; }
        [StringLength(20)]
        public string County { get; set; }
        [StringLength(3)]
        public string CountyCode { get; set; }
        [StringLength(3)]
        public string FIPSCountyCode { get; set; }
        [StringLength(10)]
        public string FIPSStateCountyCode { get; set; }
    }
}
