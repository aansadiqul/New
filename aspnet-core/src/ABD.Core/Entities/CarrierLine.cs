﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    public class CarrierLine : AuditedEntity
    {
        [StringLength(100)]
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
