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
using ABD.CompanyTypes.Dto;
using ABD.ProductLines.Dto;
using ABD.Domain.Repositories;
using ABD.Entities;
using Microsoft.EntityFrameworkCore;
namespace ABD.ProductLines
{
    [AbpAuthorize(PermissionNames.Pages_Admin)]
    public class ProductLineAppService : ABDAppServiceBase, IProductLineAppService
    {
        private readonly IRepository<ProductLine> _productLineRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ICacheManager _cacheManager;

        public ProductLineAppService(IRepository<ProductLine> productLineRepository,
                                    ICommonRepository commonRepository,
                                    ICacheManager cacheManager)
        {
            _productLineRepository = productLineRepository;
            _commonRepository = commonRepository;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<ProductLineDto>> GetAllAsync(GetTypesInput input)
        {
            var query = CreateFilteredQuery(input);
            var count = await query.CountAsync();
            var types = await query.OrderBy(p => p.Name).PageBy(input).ToListAsync();

            return new PagedResultDto<ProductLineDto>(
                count,
                ObjectMapper.Map<List<ProductLineDto>>(types)
            );
        }

        public async Task UpdateTypes()
        {
            await _cacheManager.GetCache("ProductLines").ClearAsync();
            var productLines = await _commonRepository.GetProcedureData("GetProductLines");
            foreach (var productLine in productLines)
            {
                if (!await IsThisTypeExist(productLine.Name))
                {
                    await InsertProductLine(new CreateProductLineInput
                    {
                        Name = productLine.Name,
                        IsActive = false
                    });
                }
            }
        }

        private async Task InsertProductLine(CreateProductLineInput input)
        {
            await _productLineRepository.InsertAsync(ObjectMapper.Map<ProductLine>(input));
        }

        public async Task Update(ProductLineDto input)
        {
            await _cacheManager.GetCache("ProductLines").ClearAsync();
            await _productLineRepository.UpdateAsync(ObjectMapper.Map<ProductLine>(input));
        }

        private async Task<bool> IsThisTypeExist(string name)
        {
            var query = CheckDuplicate(name);
            var result = await query.CountAsync() > 0 ? true : false;
            return result;
        }

        protected IQueryable<ProductLine> CreateFilteredQuery(GetTypesInput input)
        {
            return _productLineRepository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected IQueryable<ProductLine> CheckDuplicate(string type)
        {
            return _productLineRepository.GetAll()
                .WhereIf(!type.IsNullOrWhiteSpace(), x => x.Name == type);
        }
    }
}

