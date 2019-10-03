using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using ABD.ADSearches.Dto;
using ABD.Authorization;
using ABD.Customers;
using ABD.Entities;
using ABD.Domain.Dtos;
using ABD.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using ABD.SubscriptionPlans.Dto;
using Abp.Domain.Repositories;

namespace ABD.ADSearches
{
    [AbpAuthorize(PermissionNames.AD)]
    public class ADSearchAppService : ABDAppServiceBase, IADSearchAppService
    {
        private readonly IADSearchRepository _adSearchRepository;
        private readonly ICustomerAppService _customerAppService;
        private readonly IAbpSession _session;
        private readonly IRepository<UserProfile> _userProfile;


        public ADSearchAppService(IADSearchRepository adSearchRepository,
                                  IAbpSession session,
                                  IRepository<UserProfile> userProfile,
                                  ICustomerAppService customerAppService)
        {
            _adSearchRepository = adSearchRepository;
            _session = session;
            _userProfile = userProfile;
            _customerAppService = customerAppService;
        }

        public async Task<PagedResultDto<ADSearchDto>> GetAll(GetADSearchInput input)
        {
            var userQuery = CreateFilteredQuery(input);
            var userQueryCount = await userQuery.CountAsync();
            var userQueries = await userQuery.OrderByDescending(p => p.Id).PageBy(input).ToListAsync();

            return new PagedResultDto<ADSearchDto>(
                userQueryCount,
                ObjectMapper.Map<List<ADSearchDto>>(userQueries)
            );
        }

        public async Task<int> Create(ADSearchInput input)
        {
            return await SaveQuery(input);
        }

        public async Task Update(ADSearchDto input)
        {
            //var entity = await _adSearchRepository.FirstOrDefaultAsync(x => x.QueryName == input.QueryName);
            //if (entity != null)
            //{
            //    throw new UserFriendlyException("Query Name already Exist");
            //}
            await _adSearchRepository.UpdateAsync(ObjectMapper.Map<ADSearch>(input));
        }

        public async Task Delete(int id)
        {
            await _adSearchRepository.DeleteAsync(id);
        }

        private IQueryable<ADSearch> CreateFilteredQuery(GetADSearchInput input)
        {
            return _adSearchRepository.GetAll().Where(x => x.CreatorUserId == _session.UserId)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                x => x.QueryName.Contains(input.Keyword) || x.Id.ToString().Contains(input.Keyword))
                .WhereIf(!input.FromDate.IsNullOrWhiteSpace(),
                    x => x.CreationTime >= DateTime.Parse(input.FromDate) && x.CreationTime <= DateTime.Parse(input.ToDate));
        }

        public async Task<ADCountsDto> PostADSearchInput(ADSearchInput input)
        {
            return await LoadAdSearch(input);
        }

        private async Task<ADCountsDto> LoadAdSearch(ADSearchInput input)
        {
            if (!string.IsNullOrEmpty(input.ExcludeCountyIds))
            {
                input.IncludeCountyIds = await GetExcludeCountiesCode(input.ExcludeCountyIds);
            }
            var subtype = (from c in _userProfile.GetAll()
                           where c.UserId == _session.UserId
                           select c.SubType).SingleOrDefault();


            if (subtype == (int)AccessLevelEnum.Retail)
            {
                input.isRetail = true;
            }
            else if (subtype == (int)AccessLevelEnum.Wholesale)
            {
                input.isWholesale = true;
            }
            var adQueries = ADQueryBuilder.GetADSearchQueries(input);
            var adCounts = new ADCounts
            {
                AgencyListCount = await _adSearchRepository.GetQueryResult(adQueries.ADCountQuery),
                ADContactsCount = await _adSearchRepository.GetQueryResult(adQueries.ADContactsCountQuery),
                ADEmailCount = await _adSearchRepository.GetQueryResult(adQueries.ADEmailCountQuery)
            };

            return new ADCountsDto
            {
                ADCounts = adCounts,
                ADQueries = adQueries
            };
        }

        public async Task<ADSearchDto> GetAdSearch(int queryId)
        {
            ADSearchDto adSearch = await GetSearchDetails(queryId);
            var input = ObjectMapper.Map<ADSearchDto>(adSearch);
            return input;
        }

        public async Task<AdBuyNames> GetAdNames(int queryId)
        {
            var customer = await _customerAppService.GetCustomerByUserid((long) _session.UserId);
            var recordPrice = new RecordPriceDto
            {
                AgencyRecPrice = customer.AgencyRecPrice,
                ContactRecPrice = customer.ContactRecPrice
            };

            var adSearch = await _adSearchRepository.GetAsync(queryId);
            var query = ADQueryBuilder.GetAdNamesQuery(adSearch.QueryCriteria);
            var adNames = await _adSearchRepository.ExecuteAdNamesQuery(query);
            return new AdBuyNames
            {
                RecordPrice = recordPrice,
                ADNames = adNames
            };
        }

        public async Task<ADNamesDto> UpdatePurchase(ADPurchaseUpdate input)
        {
            var adSearch = await GetAdSearch(input.QueryId);
            adSearch.QueryCriteria = input.Query;
            await Update(adSearch);
            var query = ADQueryBuilder.GetAdNamesQuery(input.Query);
            var adNames = await _adSearchRepository.ExecuteAdNamesQuery(query);
            return new ADNamesDto
            {
                ADNames = adNames,
                QueryId = input.QueryId
            };
        }

        public async Task<List<BreakdownDto>> PostAnalyzeData(AnalyzeInput input)
        {
            var query = ADQueryBuilder.GetAdAnalyzeQuery(input);
            var result = await _adSearchRepository.ExecuteAdAnalyzeQuery(query);
            return result;
        }

        private async Task<string> GetExcludeCountiesCode(string input)
        {
            var sqlQuery = ADQueryBuilder.GetAllExcludecounties(input);
            return await _adSearchRepository.GetQueryResult(sqlQuery);
        }

        private async Task<int> SaveQuery(ADSearchInput input)
        {
            var entity = await _adSearchRepository.FirstOrDefaultAsync(x => x.QueryName == input.QueryName);
            if (entity != null)
            {
                throw new UserFriendlyException("Query Name already Exist");
            }
            var adSearch = await _adSearchRepository.InsertAndGetIdAsync(ObjectMapper.Map<ADSearch>(input));
            return adSearch;
        }

        private async Task<ADSearchDto> GetSearchDetails(int queryId)
        {
            var adSearch = await _adSearchRepository.GetAsync(queryId);
            if (adSearch.CreatorUserId != _session.UserId)
            {
                throw new UserFriendlyException("Invalid search id!");
            }
            var bdSearchDetails = ObjectMapper.Map<ADSearchDto>(adSearch);
            return bdSearchDetails;
        }
    }
}

