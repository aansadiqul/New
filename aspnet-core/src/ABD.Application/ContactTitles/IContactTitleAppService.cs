using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ABD.CompanyTypes.Dto;
using ABD.ContactTitles.Dto;
using ABD.Roles.Dto;
using ABD.Users.Dto;

namespace ABD.ContactTitles
{
    public interface IContactTitleAppService : IApplicationService
    {
        Task<PagedResultDto<ContactTitleDto>> GetAllAsync(GetTypesInput input);
        Task Update(ContactTitleDto input);
        Task UpdateTypes();
    }
}
