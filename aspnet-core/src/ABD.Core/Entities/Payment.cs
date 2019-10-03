using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities.Auditing;

namespace ABD.Entities
{
    public class Payment : CreationAuditedEntity
    {
        [ForeignKey("OrderType")]
        public int OrderTypeId { get; set; }
        [ForeignKey("PaymentType")]
        public int PaymentTypeId { get; set; }
        public long OrderAmount { get; set; }
        public string CreditCardMusked { get; set; }
        public string TransactionId { get; set; }
        public string ExternalTransactionId { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public virtual OrderType OrderType { get; set; }
        public virtual PaymentType PaymentType { get; set; }
    }
}
