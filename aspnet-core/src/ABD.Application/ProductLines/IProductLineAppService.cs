using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ABD.CompanyTypes.Dto;
using ABD.ProductLines.Dto;
using ABD.Roles.Dto;
using ABD.Users.Dto;

namespace ABD.ProductLines
{
    public interface IProductLineAppService : IApplicationService
    {
        Task<PagedResultDto<ProductLineDto>> GetAllAsync(GetTypesInput input);
        Task Update(ProductLineDto input);
        Task UpdateTypes();
    }
}
