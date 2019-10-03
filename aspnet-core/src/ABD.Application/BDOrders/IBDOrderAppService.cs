using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ABD.ADOrders.Dto;
using ABD.BDOrders.Dto;

namespace ABD.BDOrders
{
    public interface IBdOrderAppService : IApplicationService
    {
        Task<PagedResultDto<BdOrderDto>> GetAll(GetOrderInput input);
        Task<BdReceiptDto> GetReceipt(int orderId, int paymentId);
        Task<string> GetReportData(int orderId, PurchaseType purchaseType);
        Task<int> Create(BdOrderInput input);
        Task CreatePurchase(PurchaseInput input);
    }
}
