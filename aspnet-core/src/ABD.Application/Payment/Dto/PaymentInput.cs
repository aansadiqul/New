using System;
using System.Collections.Generic;
using System.Text;
using Abp.AutoMapper;
using ABD.Entities;

namespace ABD.Payment.Dto
{
    [AutoMapTo(typeof(Entities.Payment))]
    public class PaymentInput
    {
        public OrderTypeEnum OrderTypeId { get; set; }
        public long OrderAmount { get; set; }
        public string CreditCardMusked { get; set; }
        public string TransactionId { get; set; }
        public string ExternalTransactionId { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public PaymentTypeEnum PaymentTypeId { get; set; }
    }
}
