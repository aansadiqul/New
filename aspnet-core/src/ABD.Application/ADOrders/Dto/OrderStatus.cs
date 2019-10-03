using System;
using System.Collections.Generic;
using System.Text;

namespace ABD.ADOrders.Dto
{
    public enum OrderStatus
    {
        Valid = 1,
        Expired = 2,
        PurchaseRequired = 3,
        SelectOrder = 4,
    }
}
