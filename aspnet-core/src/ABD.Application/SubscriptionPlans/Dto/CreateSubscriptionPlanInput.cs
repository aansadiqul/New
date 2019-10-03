using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.AutoMapper;
using ABD.Entities;

namespace ABD.SubscriptionPlans.Dto
{
    [AutoMapTo(typeof(SubscriptionPlan))]
    public class CreateSubscriptionPlanInput
    {
        public string TypeName { get; set; }
        public int? Amount { get; set; }
        public string Options { get; set; }
        public string AccessLevel { get; set; }
    }
}
