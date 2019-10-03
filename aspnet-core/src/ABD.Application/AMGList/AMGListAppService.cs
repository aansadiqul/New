using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using ABD.AMGList.Dto;
using ABD.Authorization;
using ABD.CompanyTypes.Dto;
using ABD.Domain.Repositories;
using Microsoft.EntityFrameworkCore;


namespace ABD.AMGList
{
    [AbpAuthorize(PermissionNames.Pages_Admin)]
    public class AMGListAppService : ABDAppServiceBase, IAMGListAppService
    {
        private readonly IRepository<Entities.AMGList> _amgListRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ICacheManager _cacheManager;

        public AMGListAppService(IRepository<Entities.AMGList> amgListRepository,
                                    ICommonRepository commonRepository,
                                    ICacheManager cacheManager)
        {
            _amgListRepository = amgListRepository;
            _commonRepository = commonRepository;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<AMGListDto>> GetAllAsync(GetTypesInput input)
        {
            var query = CreateFilteredQuery(input);
            var count = await query.CountAsync();
            var types = await query.OrderBy(p => p.AList).PageBy(input).ToListAsync();

            return new PagedResultDto<AMGListDto>(
                count,
                ObjectMapper.Map<List<AMGListDto>>(types)
            );
        }

        public async Task UpdateTypes()
        {
            await _cacheManager.GetCache("AMGList").ClearAsync();
            var AMGList = await _commonRepository.GetProcedureData("GetAMGList");
            foreach (var amg in AMGList)
            {
                if (!await IsThisTypeExist(amg.Name))
                {
                    await InsertAmgList(new CreateAMGListInput
                    {
                        AList = amg.Name,
                        IsActive = false
                    });
                }
            }
        }

        private async Task InsertAmgList(CreateAMGListInput input)
        {
            await _amgListRepository.InsertAsync(ObjectMapper.Map<Entities.AMGList>(input));
        }

        public async Task Update(AMGListDto input)
        {
            await _cacheManager.GetCache("AMGList").ClearAsync();
            await _amgListRepository.UpdateAsync(ObjectMapper.Map<Entities.AMGList>(input));
        }

        private async Task<bool> IsThisTypeExist(string name)
        {
            var query = CheckDuplicate(name);
            var result = await query.CountAsync() > 0 ? true : false;
            return result;
        }

        protected IQueryable<Entities.AMGList> CreateFilteredQuery(GetTypesInput input)
        {
            return _amgListRepository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.AList.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected IQueryable<Entities.AMGList> CheckDuplicate(string type)
        {
            return _amgListRepository.GetAll()
                .WhereIf(!type.IsNullOrWhiteSpace(), x => x.AList == type);
        }

        public async Task UpdateAll(bool isAllActive)
        {
            await _cacheManager.GetCache("AMGList").ClearAsync();
            await _commonRepository.ExecuteAdminStoredProcedure("UpdateAMGList", isAllActive);

        }
    }
}
