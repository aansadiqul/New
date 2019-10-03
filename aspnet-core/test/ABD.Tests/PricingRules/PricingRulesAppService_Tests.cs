using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ABD.EntityFrameworkCore;
using ABD.PricingRules;
using ABD.PricingRules.Dto;
using ABD.SubscriptionPlans;
using ABD.SubscriptionPlans.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace ABD.Tests.PricingRules
{
    public class PricingRulesAppService_Tests : ABDTestBase
    {
        private readonly IPricingRuleAppService _pricingRuleAppService;

        public PricingRulesAppService_Tests()
        {
            _pricingRuleAppService = Resolve<IPricingRuleAppService>();
        }

        [Fact]
        public async Task Should_Get_PricingRules()
        {
            var output = await _pricingRuleAppService.GetAll(new GetPricingRulesInput());
            output.TotalCount.ShouldBe(2);
            output.Items.Count.ShouldBe(2);
        }
    }
}
