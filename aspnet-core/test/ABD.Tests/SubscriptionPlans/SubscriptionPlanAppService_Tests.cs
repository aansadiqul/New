using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ABD.EntityFrameworkCore;
using ABD.SubscriptionPlans;
using ABD.SubscriptionPlans.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace ABD.Tests.SubscriptionPlans
{
    public class SubscriptionPlanAppService_Tests : ABDTestBase
    {
        private readonly ISubscriptionPlanAppService _subscriptionPlanAppService;

        public SubscriptionPlanAppService_Tests()
        {
            _subscriptionPlanAppService = Resolve<ISubscriptionPlanAppService>();
        }

        [Fact]
        public async Task Should_Get_SubscriptionPlans()
        {
            var output = await _subscriptionPlanAppService.GetAllAsync(new GetSubscriptionPlanInput());
            output.TotalCount.ShouldBe(13);
            output.Items.Count.ShouldBe(10);
        }
    }
}
