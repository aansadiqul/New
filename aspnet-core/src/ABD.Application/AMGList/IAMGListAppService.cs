using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ABD.AMGList.Dto;
using ABD.CompanyTypes.Dto;

namespace ABD.AMGList
{
    public interface IAMGListAppService : IApplicationService
    {
        Task<PagedResultDto<AMGListDto>> GetAllAsync(GetTypesInput input);
        Task Update(AMGListDto input);
        Task UpdateTypes();
        Task UpdateAll(bool isAllActive);
    }
}
