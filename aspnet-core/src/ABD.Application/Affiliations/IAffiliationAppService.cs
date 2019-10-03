using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ABD.Affiliations.Dto;
using ABD.CompanyTypes.Dto;
using ABD.ContactTitles.Dto;
using ABD.Roles.Dto;
using ABD.Users.Dto;

namespace ABD.Affiliations
{
    public interface IAffiliationAppService : IApplicationService
    {
        Task<PagedResultDto<SpecialAffiliationDto>> GetAllAsync(GetTypesInput input);
        Task Update(SpecialAffiliationDto input);
        Task UpdateTypes();
        Task UpdateAll(bool isAllActive);
    }
}
