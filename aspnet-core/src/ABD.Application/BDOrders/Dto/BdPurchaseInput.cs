using System;
using System.Collections.Generic;
using System.Text;
using Abp.AutoMapper;
using ABD.Entities;

namespace ABD.BDOrders.Dto
{
    [AutoMapTo(typeof(BdPurchasedData))]
    public class BdPurchaseInput
    {
        public int OrderID { get; set; }
        public string DUNSNumber { get; set; }
        public string RecordType { get; set; }
    }
}
