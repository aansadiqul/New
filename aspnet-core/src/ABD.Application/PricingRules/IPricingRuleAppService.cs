using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ABD.PricingRules.Dto;
using ABD.Roles.Dto;
using ABD.Users.Dto;

namespace ABD.PricingRules
{
    public interface IPricingRuleAppService : IAsyncCrudAppService<PricingRuleDto, int, GetPricingRulesInput, CreatePricingRuleInput, PricingRuleDto>
    {
      
    }
}
