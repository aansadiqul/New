using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ABD.BDSearches.Dto;
using ABD.Entities;
using ABD.Roles.Dto;
using ABD.Users.Dto;
using ABD.Domain.Dtos;
using System.Collections.Generic;

namespace ABD.BDSearches
{
    public interface IBDSearchAppService : IApplicationService
    {
        Task<PagedResultDto<BDSearchDto>> GetAll(GetBDSearchInput input);
        Task<BDSearchDto> GetBdSearch(int id);
        Task<BDCountsDto> PostBDSearchInput(BDSearchInput input);
        Task<List<BreakdownBDDto>> PostAnalyzeData(AnalyzeBDInput input);
        Task<int> Create(BDSearchInput input);
        Task Update(BDSearchDto input);
        Task Delete(int id);
    }
}
