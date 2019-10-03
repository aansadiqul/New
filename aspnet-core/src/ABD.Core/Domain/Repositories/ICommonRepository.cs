using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using ABD.Domain.Dtos;
using ABD.Entities;

namespace ABD.Domain.Repositories
{
    public interface ICommonRepository
    {
        Task<List<CommonDto>> GetProcedureData(string procedureName);
        Task ExecuteAdminStoredProcedure(string procedureName, bool isActive);
        Task<string> GetData(string query);
        Task<string> GetBdReportData(int orderId, long? userId, int purchaseType);
    }
}
