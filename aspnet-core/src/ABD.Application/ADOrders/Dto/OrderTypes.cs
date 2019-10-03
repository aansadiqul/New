using System;
using System.Collections.Generic;
using System.Text;

namespace ABD.ADOrders.Dto
{
    public enum OrderTypes
    {
        UserSubscription = 1,
        RecordsPurchase = 2,
        UserRenewal = 3,
        WholeDatabase = 4,
        QuarterlyUpdates = 5,
        AnnualUpdates = 6,
        WholeDatabaseQuarterlyUpdates = 7,
        WholeDatabaseAnnualUpdates = 8,
        SingleLocationDatabase = 9,
        SingleLocationQuarterlyUpdates = 10,
        SingleLocationAnnualUpdates = 11,
        SingleLocationAndQuarterlyUpdates = 12,
        SingleLocationAndAnnualUpdates = 13
    }
}
