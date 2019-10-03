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
using ABD.ContactTitles.Dto;
using ABD.Domain.Repositories;
using ABD.Entities;
using Microsoft.EntityFrameworkCore;

namespace ABD.ContactTitles
{
    [AbpAuthorize(PermissionNames.Pages_Admin)]
    public class ContactTitleAppService : ABDAppServiceBase, IContactTitleAppService
    {
        private readonly IRepository<ContactTitle> _contactTitleRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ICacheManager _cacheManager;

        public ContactTitleAppService(IRepository<ContactTitle> contactTitleRepository,
                                    ICommonRepository commonRepository,
                                    ICacheManager cacheManager)
        {
            _contactTitleRepository = contactTitleRepository;
            _commonRepository = commonRepository;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<ContactTitleDto>> GetAllAsync(GetTypesInput input)
        {
            var query = CreateFilteredQuery(input);
            var count = await query.CountAsync();
            var types = await query.OrderBy(p => p.Name).PageBy(input).ToListAsync();

            return new PagedResultDto<ContactTitleDto>(
                count,
                ObjectMapper.Map<List<ContactTitleDto>>(types)
            );
        }

        public async Task UpdateTypes()
        {
            await _cacheManager.GetCache("ContactTitles").ClearAsync();
            var contactTitles = await _commonRepository.GetProcedureData("GetContactTitles");
            foreach (var contactTitle in contactTitles)
            {
                if (!await IsThisTypeExist(contactTitle.Name))
                {
                    await InsertContactTitle(new CreateContactTitleInput
                    {
                        Name = contactTitle.Name,
                        IsActive = false
                    });
                }
            }
        }

        private async Task InsertContactTitle(CreateContactTitleInput input)
        {
            await _contactTitleRepository.InsertAsync(ObjectMapper.Map<ContactTitle>(input));
        }

        public async Task Update(ContactTitleDto input)
        {
            await _cacheManager.GetCache("ContactTitles").ClearAsync();
            await _contactTitleRepository.UpdateAsync(ObjectMapper.Map<ContactTitle>(input));
        }

        private async Task<bool> IsThisTypeExist(string name)
        {
            var query = CheckDuplicate(name);
            var result = await query.CountAsync() > 0 ? true : false;
            return result;
        }

        protected IQueryable<ContactTitle> CreateFilteredQuery(GetTypesInput input)
        {
            return _contactTitleRepository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected IQueryable<ContactTitle> CheckDuplicate(string type)
        {
            return _contactTitleRepository.GetAll()
                .WhereIf(!type.IsNullOrWhiteSpace(), x => x.Name == type);
        }
    }
}

