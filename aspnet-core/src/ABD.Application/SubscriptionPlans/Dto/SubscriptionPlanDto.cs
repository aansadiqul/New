using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ABD.Entities;

namespace ABD.SubscriptionPlans.Dto
{
    [AutoMapTo(typeof(SubscriptionPlan))]
    public class SubscriptionPlanDto : AuditedEntityDto
    {
        public string TypeName { get; set; }
        public int? Amount { get; set; }
        public string Options { get; set; }
        public string AccessLevel { get; set; }
    }
}
