using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using ABD.Common.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.Extensions;
using Abp.UI;
using ABD.Entities;
using Abp.Extensions;
using Abp.Runtime.Caching;
using ABD.ADOrders.Dto;
using ABD.Domain.Dtos;
using ABD.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using ABD.EntityFrameworkCore;
using Abp.Runtime.Session;
using ABD.SubscriptionPlans.Dto;

namespace ABD.Common
{
    [AbpAuthorize]
    public class CommonAppService : ABDAppServiceBase, ICommonAppService
    {
        private readonly IDbContextProvider<ABDDbContext> _dbContextProvider;

        private readonly ICacheManager _cacheManager;

        private ABDDbContext Context => _dbContextProvider.GetDbContext();

        private readonly ICommonRepository _commonRepository;
        private readonly IRepository<UserProfile> _userProfile;
        private readonly IAbpSession _session;


        public CommonAppService(ICommonRepository commonRepository,
                                IDbContextProvider<ABDDbContext> dbContextProvider,
                                 IAbpSession session,
                                 IRepository<UserProfile> userProfile,
                                ICacheManager cacheManager)
        {
            _commonRepository = commonRepository;
            _dbContextProvider = dbContextProvider;
            _userProfile = userProfile;
            _session = session;
            _cacheManager = cacheManager;
        }

        public async Task<List<CommonDto>> GetCompanyTypes()
        {
            var subtype = (from c in _userProfile.GetAll()
                           where c.UserId == _session.UserId
                           select c.SubType).SingleOrDefault();

            bool isRetail = false;
            bool isWholesale = false;
            if (subtype == (int)AccessLevelEnum.Retail)
            {
                isRetail = true;
            }
            else if (subtype == (int)AccessLevelEnum.Wholesale)
            {
                isWholesale = true;
            }
            var query = Context.CompanyTypes.Where(x => x.IsActive == true);
            query = isRetail ? query.Where(x => x.IsRetail == isRetail) : isWholesale ? query.Where(x => x.IsWholesale == isWholesale) : query;
            var companyTypes = await query.OrderBy(x => x.Name).Select(y =>
            new CommonDto
            {
                Id = y.Id.ToString(),
                IsChecked = false,
                Name = y.Name
            }).ToListAsync();
            return companyTypes;
        }

       

        public async Task<List<CommonDto>> GetAMGList()
        {
            var result = await _cacheManager
                .GetCache("AMGList")
                .GetAsync("AMGList", AMGList);
            return result;
        }

        private async Task<List<CommonDto>> AMGList()
        {
            var amgList = await Context.AMGLists.Where(x => x.IsActive == true).OrderBy(x => x.AList).Select(y =>
                new CommonDto
                {
                    Id = y.Id.ToString(),
                    IsChecked = false,
                    Name = y.AList
                }).ToListAsync();
            return amgList;
        }

        public async Task<List<CommonDto>> GetStates()
        {
            var states = await Context.RsStates.OrderBy(x => x.Description).Select(y =>
                    new CommonDto
                    {
                        Id = y.State,
                        IsChecked = false,
                        Name = y.Description
                    }).ToListAsync();
            return states;
        }

        public async Task<List<CommonDto>> GetCarriers()
        {
            var result = await _cacheManager
                .GetCache("Carriers")
                .GetAsync("Carriers", Carriers);
            return result;
        }

        private async Task<List<CommonDto>> Carriers()
        {
            var carriers = await Context.CarrierLines.Where(x => x.IsActive == true).OrderBy(x => x.Name).Select(y =>
                new CommonDto
                {
                    Id = y.Id.ToString(),
                    IsChecked = false,
                    Name = y.Name
                }).ToListAsync();
            return carriers;
        }

        public async Task<List<CommonDto>> GetAffiliations()
        {
            var result = await _cacheManager
                .GetCache("Affiliations")
                .GetAsync("Affiliations", Affiliations);
            return result;
        }

        private async Task<List<CommonDto>> Affiliations()
        {
            var affiliations = await Context.SpecialAffiliations.Where(x => x.IsActive == true).OrderBy(x => x.Name).Select(y =>
                new CommonDto
                {
                    Id = y.Id.ToString(),
                    IsChecked = false,
                    Name = y.Name
                }).ToListAsync();
            return affiliations;

        }

        public async Task<List<CommonDto>> GetContactTitles()
        {
            var result = await _cacheManager
                .GetCache("ContactTitles")
                .GetAsync("ContactTitles", ContactTitles);
            return result;
        }

        private async Task<List<CommonDto>> ContactTitles()
        {
            var contactTitles = await Context.ContactTitles.Where(x => x.IsActive == true).OrderBy(x => x.Name).Select(y =>
                new CommonDto
                {
                    Id = y.Id.ToString(),
                    IsChecked = false,
                    Name = y.Name
                }).ToListAsync();
            return contactTitles;
        }

        public async Task<List<CommonDto>> GetProductLines()
        {
            var result = await _cacheManager
                .GetCache("ProductLines")
                .GetAsync("ProductLines", ProductLines);
            return result;
        }

        private async Task<List<CommonDto>> ProductLines()
        {
            var productLines = await Context.ProductLines.Where(x => x.IsActive == true).OrderBy(x => x.Name).Select(y =>
                new CommonDto
                {
                    Id = y.Id.ToString(),
                    IsChecked = false,
                    Name = y.Name
                }).ToListAsync();
            return productLines;
        }

        public async Task<List<CommonDto>> GetCounties(GetStatesInput input, bool isAdSearch)
        {
            var counties = new List<CommonDto>();
            List<string> includedStates = input.IncludedStates.IsNullOrEmpty() ? new List<string>() : input.IncludedStates.Split(',').ToList();
            List<string> excludedStates = input.ExcludedStates.IsNullOrEmpty() ? new List<string>() : input.ExcludedStates.Split(',').ToList();
            if (isAdSearch)
            {
                

                counties = await Context.Counties.Where(x => includedStates.Any(y => x.State.Contains(y)))
                    .Where(x => !excludedStates.Any(y => x.State.Contains(y)))
                    .OrderBy(x => x.Description).Select(y => new CommonDto
                    {
                        Name = y.Description + ":" + y.State,
                        IsChecked = false,
                        Id = y.CountyCode
                    }).ToListAsync();
            }
            else
            {
                counties = await Context.DNBFIPSCountyCodes.Where(x => includedStates.Any(y => x.State.Contains(y)))
                    .Where(x => !excludedStates.Any(y => x.State.Contains(y)))
                    .OrderBy(x => x.County).Select(y => new CommonDto
                    {
                        Name = y.County + ":" + y.State,
                        IsChecked = false,
                        Id = y.FIPSStateCountyCode
                    }).ToListAsync();
            }

            return counties;
        }

        public async Task<ZipCodesDto> GetZipCodes(GetStatesInput input)
        {
            List<string> includedStates = input.IncludedStates.IsNullOrEmpty() ? new List<string>() : input.IncludedStates.Split(',').ToList();
            List<string> excludedStates = input.ExcludedStates.IsNullOrEmpty() ? new List<string>() : input.ExcludedStates.Split(',').ToList();

            var zipCodes = await Context.ZipCodes.Where(x => includedStates.Any( y => x.State.Contains(y)))
                                .Where(x => !excludedStates.Any(y => x.State.Contains(y)))
                                .OrderBy(x => x.Zip).ToListAsync();

            var zip3Digits = zipCodes.Select(y =>
                        new CommonDto
                        {
                            Id = y.Zip.Substring(0,3),
                            Name = y.Zip.Substring(0, 3) + " - " + y.State,
                            IsChecked = false
                        }).OrderBy(x => x.Name)
                    .ToList();

            var zip5Digits = zipCodes.OrderBy(x => x.Zip).Select(y =>
                    new CommonDto
                    {
                        Id = y.Zip,
                        IsChecked = false,
                        Name = y.Zip + " - " + y.State
                    }).ToList();

            var result = new ZipCodesDto
                        {
                            Zip3Digits = zip3Digits.OrderBy(x => x.Name).GroupBy(x => x.Id)
                                .Select(y =>
                                    new CommonDto
                                    {
                                        Id = y.First().Id.ToString(),
                                        IsChecked = false,
                                        Name = y.First().Name
                                    }).ToList(),
                            Zip5Digits = zip5Digits
                        };

            return result;
        }

        public async Task<List<CommonDto>> GetMetroAreas(GetStatesInput input)
        {
            List<string> includedStates = input.IncludedStates.IsNullOrEmpty() ? new List<string>() : input.IncludedStates.Split(',').ToList();
            List<string> excludedStates = input.ExcludedStates.IsNullOrEmpty() ? new List<string>() : input.ExcludedStates.Split(',').ToList();

            var counties = await Context.MetroAreas.Where(x => includedStates.Any(y => x.State.Contains(y)))
                .Where(x => !excludedStates.Any(y => x.State.Contains(y)))
                .OrderBy(x => x.Name).Select(y => new CommonDto
                {
                    Name = y.Name + " - " + y.State,
                    IsChecked = false,
                    Id = y.Code
                }).ToListAsync();

            return counties;
        }

        public async Task<List<CommonDto>> GetAreas(GetStatesInput input)
        {
            List<string> includedStates = input.IncludedStates.IsNullOrEmpty() ? new List<string>() : input.IncludedStates.Split(',').ToList();
            List<string> excludedStates = input.ExcludedStates.IsNullOrEmpty() ? new List<string>() : input.ExcludedStates.Split(',').ToList();

            var counties = await Context.Areas.Where(x => includedStates.Any(y => x.State.Contains(y)))
                .Where(x => !excludedStates.Any(y => x.State.Contains(y)))
                .OrderBy(x => x.AreaCode).Select(y => new CommonDto
                {
                    Name = y.AreaCode + " - " + y.State,
                    IsChecked = false,
                    Id = y.AreaCode
                }).ToListAsync();

            return counties;
        }

        public async Task<List<IndustryDto>> GetIndustries()
        {
            var result = await _cacheManager
                .GetCache("Industries")
                .GetAsync("Industries", Industries);
            return result;
        }

        private async Task<List<IndustryDto>> Industries()
        {
            var lookup = Context.Industries.ToLookup(x => x.ParentId);
            var trees =  await Task.Run(() => GetTrees(lookup));
            return trees;
        }

        private static Task<List<IndustryDto>> GetTrees(ILookup<string, Industry> lookup)
        {
            Func<string, List<IndustryDto>> build = null;
            build = pid =>
                lookup[pid]
                    .Select(x => new IndustryDto()
                    {
                        Text = x.SIC + " " + x.Description,
                        Code = x.SIC,
                        Level = x.PositionCount,
                        Expandable = false,
                        IsChecked = false,
                        SIC = x.SIC,
                        SICID = x.SICId,                      
                        children = build(x.SIC),
                    }).OrderBy(x => x.Code).ToList();
            List<IndustryDto> trees = build(null);
            return Task.FromResult<List<IndustryDto>>(trees);
        }
    }
}
