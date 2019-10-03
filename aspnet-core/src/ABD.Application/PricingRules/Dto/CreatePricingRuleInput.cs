using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using ABD.Authorization.Users;
using ABD.Entities;

namespace ABD.PricingRules.Dto
{
    [AutoMapTo(typeof(PricingRule))]
    public class CreatePricingRuleInput
    {
        [Required]
        public string Title { get; set; }
        public decimal PerCreditRate { get; set; }
    }
}
