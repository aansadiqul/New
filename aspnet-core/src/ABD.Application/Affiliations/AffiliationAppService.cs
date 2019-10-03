using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using ABD.Affiliations.Dto;
using ABD.Authorization;
using ABD.CompanyTypes.Dto;
using ABD.Domain.Repositories;
using ABD.Entities;
using Microsoft.EntityFrameworkCore;

namespace ABD.Affiliations
{
    [AbpAuthorize(PermissionNames.Pages_Admin)]
    public class AffiliationAppService : ABDAppServiceBase, IAffiliationAppService
    {
        private readonly IRepository<SpecialAffiliation> _affiliationRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ICacheManager _cacheManager;

        public AffiliationAppService(IRepository<SpecialAffiliation> affiliationRepository,
                                    ICommonRepository commonRepository,
                                    ICacheManager cacheManager)
        {
            _affiliationRepository = affiliationRepository;
            _commonRepository = commonRepository;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<SpecialAffiliationDto>> GetAllAsync(GetTypesInput input)
        {
            var query = CreateFilteredQuery(input);
            var count = await query.CountAsync();
            var types = await query.OrderBy(p => p.Name).PageBy(input).ToListAsync();

            return new PagedResultDto<SpecialAffiliationDto>(
                count,
                ObjectMapper.Map<List<SpecialAffiliationDto>>(types)
            );
        }

        public async Task UpdateTypes()
        {
            await _cacheManager.GetCache("Affiliations").ClearAsync();
            var contactTitles = await _commonRepository.GetProcedureData("GetAffiliations");
            foreach (var contactTitle in contactTitles)
            {
                if (!await IsThisTypeExist(contactTitle.Name))
                {
                    await InsertSpecialAffiliation(new CreateAffiliationInput
                    {
                        Name = contactTitle.Name,
                        IsActive = false
                    });
                }
            }
        }

        private async Task InsertSpecialAffiliation(CreateAffiliationInput input)
        {
            await _affiliationRepository.InsertAsync(ObjectMapper.Map<SpecialAffiliation>(input));
        }

        public async Task Update(SpecialAffiliationDto input)
        {
            await _cacheManager.GetCache("Affiliations").ClearAsync();
            await _affiliationRepository.UpdateAsync(ObjectMapper.Map<SpecialAffiliation>(input));
        }

        private async Task<bool> IsThisTypeExist(string name)
        {
            var query = CheckDuplicate(name);
            var result = await query.CountAsync() > 0 ? true : false;
            return result;
        }

        protected IQueryable<SpecialAffiliation> CreateFilteredQuery(GetTypesInput input)
        {
            return _affiliationRepository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected IQueryable<SpecialAffiliation> CheckDuplicate(string type)
        {
            return _affiliationRepository.GetAll()
                .WhereIf(!type.IsNullOrWhiteSpace(), x => x.Name == type);
        }

        public async Task UpdateAll(bool isAllActive)
        {
            await _cacheManager.GetCache("Affiliations").ClearAsync();
            await _commonRepository.ExecuteAdminStoredProcedure("UpdateAffiliations", isAllActive);

        }
    }
}

