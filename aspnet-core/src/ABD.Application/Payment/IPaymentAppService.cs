using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ABD.ADOrders.Dto;
using ABD.BDOrders.Dto;
using ABD.Payment.Dto;
namespace ABD.Payment
{
    public interface IPaymentAppService : IApplicationService
    {
        Task<PagedResultDto<PaymentDto>> GetAll(GetPaymentInput input);
        KeyedSaleResponse CheckOut(PaymentRequestDto PaymentRequest);
        Task<int> Create(PaymentInput paymentInput);
    }
}
