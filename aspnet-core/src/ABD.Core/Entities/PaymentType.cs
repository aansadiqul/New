using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;

namespace ABD.Entities
{
    public class PaymentType : Entity
    {
        public string Name { get; set; }
    }
}
