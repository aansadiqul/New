using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    [Table("ContactsListEmailCount")]
    public class ContactsListEmailCount : Entity
    {
        [StringLength(64)]
        public string Type { get; set; }
        public int? Locations { get; set; }
        public int? Contacts { get; set; }
        public int? ContactsWithEmail { get; set; }
    }
}
