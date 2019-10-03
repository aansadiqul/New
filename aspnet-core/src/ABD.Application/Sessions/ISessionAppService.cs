using System.Threading.Tasks;
using Abp.Application.Services;
using ABD.Sessions.Dto;

namespace ABD.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
