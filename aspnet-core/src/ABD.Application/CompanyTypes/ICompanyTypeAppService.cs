using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ABD.CompanyTypes.Dto;
using ABD.Roles.Dto;
using ABD.Users.Dto;

namespace ABD.CompanyTypes
{
    public interface ICompanyTypeAppService : IApplicationService
    {
        Task<PagedResultDto<CompanyTypeDto>> GetAllAsync(GetTypesInput input);
        Task Update(CompanyTypeDto input);
        Task UpdateAll(bool isAllActive);
        Task UpdateCompanyTypes();
    }
}
