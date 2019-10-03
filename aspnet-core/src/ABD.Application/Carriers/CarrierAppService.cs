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
using ABD.Carriers.Dto;
using ABD.CompanyTypes.Dto;
using ABD.Domain.Repositories;
using ABD.Entities;
using Microsoft.EntityFrameworkCore;

namespace ABD.Carriers
{
    [AbpAuthorize(PermissionNames.Pages_Admin)]
    public class CarrierAppService : ABDAppServiceBase, ICarrierAppService
    {
        private readonly IRepository<CarrierLine> _carrierLineRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ICacheManager _cacheManager;

        public CarrierAppService(IRepository<CarrierLine> carrierLineRepository,
                                    ICommonRepository commonRepository,
                                    ICacheManager cacheManager)
        {
            _carrierLineRepository = carrierLineRepository;
            _commonRepository = commonRepository;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<CarrierLineDto>> GetAllAsync(GetTypesInput input)
        {
            var query = CreateFilteredQuery(input);
            var count = await query.CountAsync();
            var types = await query.OrderBy(p => p.Name).PageBy(input).ToListAsync();

            return new PagedResultDto<CarrierLineDto>(
                count,
                ObjectMapper.Map<List<CarrierLineDto>>(types)
            );
        }

        public async Task UpdateTypes()
        {
            await _cacheManager.GetCache("Carriers").ClearAsync();
            var contactTitles = await _commonRepository.GetProcedureData("GetCarriers");
            foreach (var contactTitle in contactTitles)
            {
                if (!await IsThisTypeExist(contactTitle.Name))
                {
                    await InsertCarrierLine(new CreateCarrierLineInput
                    {
                        Name = contactTitle.Name,
                        IsActive = false
                    });
                }
            }
        }

        public async Task UpdateAll(bool isAllActive)
        {
            await _cacheManager.GetCache("Carriers").ClearAsync();
            await _commonRepository.ExecuteAdminStoredProcedure("UpdateCarriers", isAllActive);

        }

        private async Task InsertCarrierLine(CreateCarrierLineInput input)
        {
            await _carrierLineRepository.InsertAsync(ObjectMapper.Map<CarrierLine>(input));
        }

        public async Task Update(CarrierLineDto input)
        {
            await _cacheManager.GetCache("Carriers").ClearAsync();
            await _carrierLineRepository.UpdateAsync(ObjectMapper.Map<CarrierLine>(input));
        }

        private async Task<bool> IsThisTypeExist(string name)
        {
            var query = CheckDuplicate(name);
            var result = await query.CountAsync() > 0 ? true : false;
            return result;
        }

        protected IQueryable<CarrierLine> CreateFilteredQuery(GetTypesInput input)
        {
            return _carrierLineRepository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected IQueryable<CarrierLine> CheckDuplicate(string type)
        {
            return _carrierLineRepository.GetAll()
                .WhereIf(!type.IsNullOrWhiteSpace(), x => x.Name == type);
        }
    }
}

