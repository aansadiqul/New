using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ABD.Entities;

namespace ABD.Payment.Dto
{
    [AutoMapFrom(typeof(Entities.Payment))]
    public class PaymentDto : CreationAuditedEntityDto
    {
        public OrderType OrderType { get; set; }
        public long OrderAmount { get; set; }
        public string CreditCardMusked { get; set; }
        public string TransactionId { get; set; }
        public string ExternalTransactionId { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}
