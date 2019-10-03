using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using ABD.Domain.Dtos;
using ABD.Entities;

namespace ABD.Domain.Repositories
{
    public interface IADSearchRepository : IRepository<ADSearch>
    {
        Task<List<BreakdownDto>> ExecuteAdAnalyzeQuery(string input);
        Task<List<BreakdownBDDto>> ExecuteBDAnalyzeQuery(string input);
        Task<List<ADName>> ExecuteAdNamesQuery(string input);
        Task<List<XDateBreakdownDto>> ExecuteXDateBreakdownQuery(string input);
        Task<string> GetQueryResult(string input);

    }
}
