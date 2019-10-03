using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ABD.ADOrders.Dto;
using ABD.Domain.Dtos;
using ABD.Hangfire.Dto;

namespace ABD.ADOrders
{
    public interface IADOrderAppService : IApplicationService
    {
        Task<PagedResultDto<ADOrderDto>> GetAll(GetOrderInput input);
        Task<AdReceiptDto> GetReceipt(int orderId, int paymentId);
        Task<JSonResultDto> GetMapData(int orderId);
        Task Create(ADOrderInput input);
        Task<JSonResultDto> GetReportData(int orderId, ReportType reportType);
        Task<AccountDto> GetAccountDetails(string accountId);
        Task SendEmail(AdOrderMailInput input);
    }
}
