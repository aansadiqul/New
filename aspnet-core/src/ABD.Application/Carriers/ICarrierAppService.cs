using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ABD.Affiliations.Dto;
using ABD.Carriers.Dto;
using ABD.CompanyTypes.Dto;
using ABD.ContactTitles.Dto;
using ABD.Roles.Dto;
using ABD.Users.Dto;

namespace ABD.Carriers
{
    public interface ICarrierAppService : IApplicationService
    {
        Task<PagedResultDto<CarrierLineDto>> GetAllAsync(GetTypesInput input);
        Task Update(CarrierLineDto input);
        Task UpdateTypes();
        Task UpdateAll(bool isAllActive);
    }
}
