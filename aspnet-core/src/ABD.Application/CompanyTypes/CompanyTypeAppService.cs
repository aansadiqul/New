using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using ABD.Authorization;
using ABD.Entities;
using ABD.CompanyTypes.Dto;
using ABD.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ABD.CompanyTypes
{
    [AbpAuthorize(PermissionNames.Pages_Admin)]
    public class CompanyTypeAppService : ABDAppServiceBase, ICompanyTypeAppService
    {
        private readonly IRepository<CompanyType> _companyTypeRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ICacheManager _cacheManager;

        public CompanyTypeAppService(IRepository<CompanyType> companyTypeRepository,
                                    ICommonRepository commonRepository,
                                    ICacheManager cacheManager)
        {
            _companyTypeRepository = companyTypeRepository;
            _commonRepository = commonRepository;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<CompanyTypeDto>> GetAllAsync(GetTypesInput input)
        {
            var query = CreateFilteredQuery(input);
            var count = await query.CountAsync();
            var companyTypes = await query.OrderBy(p => p.Name).PageBy(input).ToListAsync();

            return new PagedResultDto<CompanyTypeDto>(
                count,
                ObjectMapper.Map<List<CompanyTypeDto>>(companyTypes)
            );
        }

        public async Task UpdateCompanyTypes()
        {
            await _cacheManager.GetCache("CompanyTypes").ClearAsync();
            var companyTypes = await _commonRepository.GetProcedureData("GetCompanyTypes");
            foreach (var companyType in companyTypes)
            {
                if (!await IsThisTypeExist(companyType.Name))
                {
                    await InsertCompanyType(new CreateCompanyTypeInput
                    {
                        Name = companyType.Name,
                        IsActive = false,
                        IsRetail = false,
                        IsWholesale = false
                    });
                }
            }
        }

        private async Task InsertCompanyType(CreateCompanyTypeInput companyType)
        {
            await _companyTypeRepository.InsertAsync(ObjectMapper.Map<CompanyType>(companyType));
        }

        public async Task Update(CompanyTypeDto input)
        {
            await _cacheManager.GetCache("CompanyTypes").ClearAsync();
            await _companyTypeRepository.UpdateAsync(ObjectMapper.Map<CompanyType>(input));
        }

        public async Task UpdateAll(bool isAllActive)
        {
            await _cacheManager.GetCache("CompanyTypes").ClearAsync();
            await _commonRepository.ExecuteAdminStoredProcedure("UpdateCompanyTypes", isAllActive);

        }

        private async Task<bool> IsThisTypeExist(string companyTypeName)
        {
            var query = CheckDuplicate(companyTypeName);
            var result = await query.CountAsync() > 0 ? true : false;
            return result;
        }

        protected IQueryable<CompanyType> CreateFilteredQuery(GetTypesInput input)
        {
            return _companyTypeRepository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected IQueryable<CompanyType> CheckDuplicate(string companyType)
        {
            return _companyTypeRepository.GetAll()
                .WhereIf(!companyType.IsNullOrWhiteSpace(), x => x.Name == companyType);
        }
    }
}

