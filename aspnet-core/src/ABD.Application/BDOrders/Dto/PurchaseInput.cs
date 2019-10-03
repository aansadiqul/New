using System;
using System.Collections.Generic;
using System.Text;

namespace ABD.BDOrders.Dto
{
    public class PurchaseInput
    {
        public int OrderId { get; set; }
        public PurchaseType PurchaseType { get; set; }
        public string RecordQuery { get; set; }
        public int RecordCount { get; set; }
        public string XDateQuery { get; set; }
        public int XDateCount{ get; set; }
        public string XDateMonths{ get; set; }
    }
}
