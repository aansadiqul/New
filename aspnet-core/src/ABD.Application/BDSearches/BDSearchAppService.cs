using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using ABD.BDSearches.Dto;
using ABD.Authorization;
using ABD.Authorization.Accounts;
using ABD.Authorization.Roles;
using ABD.Authorization.Users;
using ABD.Entities;
using ABD.Roles.Dto;
using ABD.Users.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using ABD.Domain.Repositories;
using System.Text;
using ABD.ADSearches.Dto;
using ABD.Domain.Dtos;

namespace ABD.BDSearches
{
    [AbpAuthorize(PermissionNames.BD)]
    public class BDSearchAppService : ABDAppServiceBase, IBDSearchAppService
    {

        private readonly IRepository<BDSearch> _bdSearchRepository;
        private readonly IADSearchRepository _adSearchRepository;
        private readonly IAbpSession _session;

        public BDSearchAppService(IRepository<BDSearch> bdSearchRepository, IADSearchRepository adSearchRepository, IAbpSession session)
        {
            _bdSearchRepository = bdSearchRepository;
            _adSearchRepository = adSearchRepository;
            _session = session;
        }

        public async Task<PagedResultDto<BDSearchDto>> GetAll(GetBDSearchInput input)
        {
            var userQuery = CreateFilteredQuery(input);
            var userQueryCount = await userQuery.CountAsync();
            var userQueries = await userQuery.OrderByDescending(p => p.Id).PageBy(input).ToListAsync();

            return new PagedResultDto<BDSearchDto>(
                userQueryCount,
                ObjectMapper.Map<List<BDSearchDto>>(userQueries)
            );
        }

        public async Task<BDSearchDto> GetBdSearch(int queryId)
        {
            BDSearchDto bdSearch = await GetSearchDetails(queryId);
            var input = ObjectMapper.Map<BDSearchDto>(bdSearch);
            return input;
        }

        public async Task<int> Create(BDSearchInput input)
        {
            var entity = await _bdSearchRepository.FirstOrDefaultAsync(x => x.SearchName == input.SearchName);
            if (entity != null)
            {
                throw new UserFriendlyException("Query Name already Exist");
            }
            var bdSearchID = await _bdSearchRepository.InsertAndGetIdAsync(ObjectMapper.Map<BDSearch>(input));
            return bdSearchID;
        }

        public async Task Update(BDSearchDto input)
        {
            await _bdSearchRepository.UpdateAsync(ObjectMapper.Map<BDSearch>(input));
        }

        public async Task Delete(int id)
        {
            await _bdSearchRepository.DeleteAsync(id);
        }
        public async Task<List<BreakdownBDDto>> PostAnalyzeData(AnalyzeBDInput input)
        {
            var query = BDQueryBuilder.GetBDAnalyzeQuery(input);
            var result = await _adSearchRepository.ExecuteBDAnalyzeQuery(query);
            return result;
        }

        private IQueryable<BDSearch> CreateFilteredQuery(GetBDSearchInput input)
        {
            return _bdSearchRepository.GetAll().Where(x => x.CreatorUserId == _session.UserId)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                x => x.SearchName.Contains(input.Keyword) || x.Id.ToString().Contains(input.Keyword))
                .WhereIf(!input.FromDate.IsNullOrWhiteSpace(),
                    x => x.CreationTime >= DateTime.Parse(input.FromDate) && x.CreationTime <= DateTime.Parse(input.ToDate));
        }
        public async Task<BDCountsDto> PostBDSearchInput(BDSearchInput input)
        {
            return await LoadBDSearch(input);
        }
        private async Task<BDCountsDto> LoadBDSearch(BDSearchInput input)
        {
            if (string.IsNullOrEmpty(input.State) || string.IsNullOrEmpty(input.Exclude_State))
            {
                input = await GetSelectedState(input);
                input.IncludeCountyIds = await GetExcludeCountiesCode(input.State, input.Exclude_State);
            }
            var bdQueries = BDQueryBuilder.GetBDSearchQueries(input);

            var bdCounts = new BDCounts
            {
                BusinessListCount = Convert.ToInt64(await _adSearchRepository.GetQueryResult(bdQueries.BDCountQuery)),
                BDXDateListCount = Convert.ToInt64(await _adSearchRepository.GetQueryResult(bdQueries.BDXDateCountQuery)),
                BDXDateBreakDown = await _adSearchRepository.ExecuteXDateBreakdownQuery(bdQueries.BDBreakdownQuery),

            };

            return new BDCountsDto
            {
                BDCounts = bdCounts,
                BDQueries = bdQueries
            };
        }

        private async Task<string> GetExcludeCountiesCode(string states, string exStates)
        {
            string countyQuery = BDQueryBuilder.GetCountyQuery(states, exStates);
            return await _adSearchRepository.GetQueryResult(countyQuery);

        }
        private static string GetZipQuery(string zip)
        {
            if (!string.IsNullOrEmpty(zip.Trim()))
            {
                string strList = "";
                strList = zip.Trim();
                string[] strArray = strList.Split(",");
                string sValue;
                StringBuilder STB = new StringBuilder();
                for (int I = 0; I <= strArray.Length - 1; I++)
                {
                    sValue = "";
                    sValue = strArray[I];
                    if (STB.ToString().Trim() == "")
                    {
                        if (sValue.Trim().Length > 3)
                            STB.Append("(Zip like '" + sValue + "%')");
                        else if (sValue.Trim().Length == 3)
                            STB.Append("(Zip like '" + sValue + "%')");
                    }
                    else if (sValue.Trim().Length > 3)
                        STB.Append(" OR (Zip like '" + sValue + "%')");
                    else if (sValue.Trim().Length == 3)
                        STB.Append(" OR (Zip like '" + sValue + "%')");
                }
                if (STB.ToString().Trim() != "")
                    return "(" + STB.ToString() + ")";
                else
                    return "";
            }
            else
                return "";
        }
        private static string GetExcludeZipQuery(string exzip)
        {
            if (!string.IsNullOrEmpty(exzip.Trim()))
            {
                string strList = "";
                strList = exzip.Trim();
                string[] strArray = strList.Split(",");
                string sValue;
                StringBuilder STB = new StringBuilder();
                for (int I = 0; I <= strArray.Length - 1; I++)
                {
                    sValue = "";
                    sValue = strArray[I];
                    if (STB.ToString().Trim() == "")
                    {
                        if (sValue.Trim().Length > 3)
                            STB.Append("(Zip not like '" + sValue + "%')");
                        else if (sValue.Trim().Length == 3)
                            STB.Append("(Zip not like '" + sValue + "%')");
                    }
                    else if (sValue.Trim().Length > 3)
                        STB.Append(" AND (Zip not like '" + sValue + "%')");
                    else if (sValue.Trim().Length == 3)
                        STB.Append(" AND (Zip not like '" + sValue + "%')");
                }
                if (STB.ToString().Trim() != "")
                    return "(" + STB.ToString() + ")";
                else
                    return "";
            }
            else
                return "";
        }
        private async Task<BDSearchInput> GetSelectedState(BDSearchInput input)
        {
            // This function will get the states which dont have to sub parts like counties and its call on restore the Query
            string strState;
            string strExState;
            strState = input.County;
            strExState = input.Exclude_State;
            string StrSql;

            if (!string.IsNullOrEmpty(input.County))
            {
                string selectedcounties;
                selectedcounties = strState.Replace(",", "','");
                selectedcounties = "'" + selectedcounties + "'";

                StrSql = "select DISTINCT STATE  from RS_DNB_FIPSCOUNTYCODES WHERE FIPSStateCountyCode IN (" + selectedcounties + ")";
                string strquerycounty = "SELECT STRING_AGG(State, \',\') FROM (" + StrSql + ") as TB";
                strState = await _adSearchRepository.GetQueryResult(strquerycounty);
                foreach (string str in strState.Split(","))
                {
                    input.State = input.State.Replace(str + ",", "");
                }
            }
            if (!string.IsNullOrEmpty(input.Exclude_State))
            {
                string selectedExcounties;
                selectedExcounties = strExState.Replace(",", "','");
                selectedExcounties = "'" + selectedExcounties + "'";

                StrSql = "select DISTINCT STATE  from RS_DNB_FIPSCOUNTYCODES WHERE FIPSStateCountyCode IN (" + selectedExcounties + ")";
                string strquerycounty = "SELECT STRING_AGG(State, \',\') FROM (" + StrSql + ") as TB";
                strExState = await _adSearchRepository.GetQueryResult(strquerycounty);
                foreach (string str in strExState.Split(","))
                {
                    input.Exclude_State = input.Exclude_State.Replace(str + ",", "");
                }
            }
            if (!string.IsNullOrEmpty(input.Zip))
            {
                string strzipQ = GetZipQuery(input.Zip);
                strzipQ = strzipQ.Replace("Zip", "zipcode");
                StrSql = "Select  distinct state  from RS_DNB_ZipCodes where (" + strzipQ + ")";
                string strquerycounty = "SELECT STRING_AGG(State, \',\') FROM (" + StrSql + ") as TB";
                strState = await _adSearchRepository.GetQueryResult(strquerycounty);
                foreach (string str in strState.Split(","))
                {
                    input.State = input.State.Replace(str + ",", "");
                }
            }
            if (!string.IsNullOrEmpty(input.Exclude_Zip))
            {
                string strzipQ = GetExcludeZipQuery(input.Exclude_Zip);
                strzipQ = strzipQ.Replace("Zip", "zipcode");
                StrSql = "Select distinct state  from RS_DNB_ZipCodes where (" + strzipQ + ")";
                string strquerycounty = "SELECT STRING_AGG(State, \',\') FROM (" + StrSql + ") as TB";
                strExState = await _adSearchRepository.GetQueryResult(strquerycounty);
                foreach (string str in strExState.Split(","))
                {
                    input.Exclude_State = input.Exclude_State.Replace(str + ",", "");
                }
            }
            if (!string.IsNullOrEmpty(input.AreaCode))
            {
                string selectedArea;
                selectedArea = input.AreaCode.Replace(",", "','");
                selectedArea = "'" + selectedArea + "'";

                StrSql = "Select distinct State   from RS_AreaCodes where AreaCode in (" + selectedArea + ")";
                string strquerycounty = "SELECT STRING_AGG(State, \',\') FROM (" + StrSql + ") as TB";
                strState = await _adSearchRepository.GetQueryResult(strquerycounty);
                foreach (string str in strState.Split(","))
                {
                    input.State = input.State.Replace(str + ",", "");
                }
            }
            if (!string.IsNullOrEmpty(input.Exclude_AreaCode))
            {
                string selectedExArea;
                selectedExArea = input.Exclude_AreaCode.Replace(",", "','");
                selectedExArea = "'" + selectedExArea + "'";

                StrSql = "Select distinct State   from RS_AreaCodes where AreaCode in (" + selectedExArea + ")";
                string strquerycounty = "SELECT STRING_AGG(State, \',\') FROM (" + StrSql + ") as TB";
                strExState = await _adSearchRepository.GetQueryResult(strquerycounty);
                foreach (string str in strExState.Split(","))
                {
                    input.Exclude_State = input.Exclude_State.Replace(str + ",", "");
                }
            }

            if (!string.IsNullOrEmpty(input.MSA))
            {
                string selectedMsa;
                selectedMsa = input.MSA.Replace(",", "','");
                selectedMsa = "'" + selectedMsa + "'";

                StrSql = "Select DISTINCT State  from RS_DNB_SMSAs where SMSACode IN (" + selectedMsa + ")";
                string strquerycounty = "SELECT STRING_AGG(State, \',\') FROM (" + StrSql + ") as TB";
                strState = await _adSearchRepository.GetQueryResult(strquerycounty);
                foreach (string str in strState.Split(","))
                {
                    input.State = input.State.Replace(str + ",", "");
                }
            }
            if (!string.IsNullOrEmpty(input.Exclude_MSA))
            {
                string selectedExMsa;
                selectedExMsa = input.Exclude_MSA.Replace(",", "','");
                selectedExMsa = "'" + selectedExMsa + "'";

                StrSql = "Select DISTINCT State  from RS_DNB_SMSAs where SMSACode IN (" + selectedExMsa + ")";
                string strquerycounty = "SELECT STRING_AGG(State, \',\') FROM (" + StrSql + ") as TB";
                strExState = await _adSearchRepository.GetQueryResult(strquerycounty);
                foreach (string str in strExState.Split(","))
                {
                    input.Exclude_State = input.Exclude_State.Replace(str + ",", "");
                }
            }

            input.IL_SlectedStates = strState;
            input.IL_EXSlectedStates = strExState;
            return input;
        }

        private async Task<BDSearchDto> GetSearchDetails(int queryId)
        {
            var bdSearch = await _bdSearchRepository.GetAsync(queryId);
            if (bdSearch.CreatorUserId != _session.UserId)
            {
                throw new UserFriendlyException("Invalid search id!");
            }
            var bdSearchDetails = ObjectMapper.Map<BDSearchDto>(bdSearch);
            return bdSearchDetails;
        }
    }
}

