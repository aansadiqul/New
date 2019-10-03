using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using ABD.Authorization;
using ABD.Authorization.Accounts;
using ABD.Authorization.Roles;
using ABD.Authorization.Users;
using ABD.Entities;
using ABD.PricingRules;
using ABD.PricingRules.Dto;
using ABD.Roles.Dto;
using ABD.Users.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ABD.PricingRules
{
    [AbpAuthorize]
    public class PricingRuleAppService : AsyncCrudAppService<PricingRule, PricingRuleDto, int, GetPricingRulesInput, CreatePricingRuleInput, PricingRuleDto>, IPricingRuleAppService
    {

        public PricingRuleAppService(
            IRepository<PricingRule> pricingRuleRepository) : base(pricingRuleRepository)
        {
        }

        public override async Task<PagedResultDto<PricingRuleDto>> GetAll(GetPricingRulesInput input)
        {
            var query = CreateFilteredQuery(input);
            var queryCount = await query.CountAsync();
            var userQueries = await query.OrderBy(p => p.Id).PageBy(input).ToListAsync();

            return new PagedResultDto<PricingRuleDto>(
                queryCount,
                ObjectMapper.Map<List<PricingRuleDto>>(userQueries)
            );
        }

        protected override IQueryable<PricingRule> CreateFilteredQuery(GetPricingRulesInput input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Title.Contains(input.Keyword));
        }
    }
}

