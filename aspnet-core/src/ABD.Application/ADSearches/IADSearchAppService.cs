using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ABD.ADSearches.Dto;
using ABD.Domain.Dtos;
using ABD.Entities;
using ABD.Roles.Dto;
using ABD.Users.Dto;

namespace ABD.ADSearches
{
    public interface IADSearchAppService : IApplicationService
    {
        Task<PagedResultDto<ADSearchDto>> GetAll(GetADSearchInput input);
        Task Delete(int id);
        Task<ADCountsDto> PostADSearchInput(ADSearchInput input);
        Task<ADSearchDto> GetAdSearch(int id);
        Task<List<BreakdownDto>> PostAnalyzeData(AnalyzeInput input);
        Task<int> Create(ADSearchInput input);
        Task Update(ADSearchDto input);
        Task<AdBuyNames> GetAdNames(int queryId);
        Task<ADNamesDto> UpdatePurchase(ADPurchaseUpdate input);
    }
}
