using System.Threading.Tasks;
using Abp.Application.Services;
using ABD.Authorization.Accounts.Dto;

namespace ABD.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
