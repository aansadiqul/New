using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;

namespace ABD.ADOrders.Dto
{
    public class AffiliationDto : EntityDto
    {
        public string AccountId { get; set; }
        public string Account { get; set; }
        public string SpecialAffiliation { get; set; }
    }
}
