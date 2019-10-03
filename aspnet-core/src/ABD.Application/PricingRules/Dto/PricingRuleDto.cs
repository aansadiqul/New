using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using ABD.Authorization.Users;
using ABD.Entities;

namespace ABD.PricingRules.Dto
{
    [AutoMapFrom(typeof(PricingRule))]
    public class PricingRuleDto : AuditedEntityDto
    {
        public string Title { get; set; }
        public decimal PerCreditRate { get; set; }

        public decimal noRecords { get; set; }
    }
}
