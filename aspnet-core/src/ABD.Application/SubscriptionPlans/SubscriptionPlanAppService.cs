using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using ABD.Authorization;
using ABD.Authorization.Users;
using ABD.Entities;
using ABD.SubscriptionPlans.Dto;
using Microsoft.EntityFrameworkCore;

namespace ABD.SubscriptionPlans
{
    [AbpAuthorize]
    public class SubscriptionPlanAppService : ABDAppServiceBase, ISubscriptionPlanAppService
    {
        private readonly IRepository<SubscriptionPlan> _subscriptionPlanRepository;

        public SubscriptionPlanAppService(IRepository<SubscriptionPlan> subscriptionPlanRepository)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
        }

        
        public async Task<PagedResultDto<SubscriptionPlanDto>> GetAllAsync(GetSubscriptionPlanInput input)
        {
            var query = CreateFilteredQuery(input);
            var subscriptionPlanCount = await query.CountAsync();
            var subscriptionPlans = await query.OrderBy(p => p.Id).PageBy(input).ToListAsync();

            return new PagedResultDto<SubscriptionPlanDto>(
                subscriptionPlanCount,
                ObjectMapper.Map<List<SubscriptionPlanDto>>(subscriptionPlans)
            );
        }

        public async Task<SubscriptionPlanDto> GetAsync(int id)
        {
            var subscriptionPlan = await _subscriptionPlanRepository.GetAsync(id);
            if (subscriptionPlan == null)
            {
                throw new UserFriendlyException("Could not find the subscription plan, maybe it's deleted!");
            }

            return subscriptionPlan.MapTo<SubscriptionPlanDto>();
        }

        [AbpAuthorize(PermissionNames.Pages_Admin)]
        public async Task CreateAsync(CreateSubscriptionPlanInput input)
        {
            var entity = await _subscriptionPlanRepository.FirstOrDefaultAsync(x => x.TypeName == input.TypeName);
            if (entity != null)
            {
                throw new UserFriendlyException("Type name already Exist");
            }
            else
            {
                await _subscriptionPlanRepository.InsertAsync(ObjectMapper.Map<SubscriptionPlan>(input));
            }
        }

        [AbpAuthorize(PermissionNames.Pages_Admin)]
        public async Task Update(SubscriptionPlanDto subscriptionPlan)
        {
            await _subscriptionPlanRepository.UpdateAsync(ObjectMapper.Map<SubscriptionPlan>(subscriptionPlan));
        }

        [AbpAuthorize(PermissionNames.Pages_Admin)]
        public async Task Delete(int id)
        {
            await _subscriptionPlanRepository.DeleteAsync(id);
        }


        private IQueryable<SubscriptionPlan> CreateFilteredQuery(GetSubscriptionPlanInput input)
        {
            return _subscriptionPlanRepository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.TypeName.Contains(input.Keyword));
        }
    }
}