using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Services;
using ABD.Authorization.Users;
using ABD.Entities;
using ABD.PricingRules.Dto;
using ABD.SubscriptionPlans.Dto;

namespace ABD.SubscriptionPlans
{
    public interface ISubscriptionPlanAppService : IApplicationService
    {
        Task<PagedResultDto<SubscriptionPlanDto>> GetAllAsync(GetSubscriptionPlanInput input);
        Task<SubscriptionPlanDto> GetAsync(int id);
        Task CreateAsync(CreateSubscriptionPlanInput @subscriptionPlan);
        Task Update(SubscriptionPlanDto @subscriptionPlan);
        Task Delete(int id);
    }
}