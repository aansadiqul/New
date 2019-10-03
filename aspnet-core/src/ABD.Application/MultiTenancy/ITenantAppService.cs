using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ABD.MultiTenancy.Dto;

namespace ABD.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

