import { IsTenantAvailableOutputState, ADOrderInputOrderType, ReportType, JSonResultDtoStatus, PurchaseType } from '@shared/service-proxies/service-proxies';


export class AppTenantAvailabilityState {
    static Available: number = IsTenantAvailableOutputState._1;
    static InActive: number = IsTenantAvailableOutputState._2;
    static NotFound: number = IsTenantAvailableOutputState._3;
}

export class PurchaseTypes {
    static Records: number = PurchaseType._1;
    static XDates: number = PurchaseType._2;
    static All: number = PurchaseType._3;
}

export class OrderType {
    static Ad: number = 1;
    static Bd: number = 2;
    static AdSubscription: number = 3;
    static BdSubscription: number = 4;
}

export class PaymentType {
    static Card: number = 1;
    static Check: number = 2;
}

export class ReportTypes {
    static AgencyReport: number = ReportType._1;
    static ContactsReport: number = ReportType._2;
    static CarriersReport: number = ReportType._3;
    static SicCodesReport: number = ReportType._4;
    static AffliationsReport: number = ReportType._5;
}

export class OrderStatus {
    static Valid: number = JSonResultDtoStatus._1;
    static Expired: number = JSonResultDtoStatus._2;
    static PurchaseRequired: number = JSonResultDtoStatus._3;
    static SelectOrder: number = JSonResultDtoStatus._4;
}

export class AdOrderTypes {
    static UserSubscription: number = ADOrderInputOrderType._1;
    static RecordsPurchase: number = ADOrderInputOrderType._2;
    static UserRenewal: number = ADOrderInputOrderType._3;
    static WholeDatabase: number = ADOrderInputOrderType._4;
    static QuarterlyUpdates: number = ADOrderInputOrderType._5;
    static AnnualUpdates: number = ADOrderInputOrderType._6;
    static WholeDatabaseQuarterlyUpdates: number = ADOrderInputOrderType._7;
    static WholeDatabaseAnnualUpdates: number = ADOrderInputOrderType._8;
    static SingleLocationDatabase: number = ADOrderInputOrderType._9;
    static SingleLocationQuarterlyUpdates: number = ADOrderInputOrderType._10;
    static SingleLocationAnnualUpdates: number = ADOrderInputOrderType._11;
    static SingleLocationAndQuarterlyUpdates: number = ADOrderInputOrderType._12;
    static SingleLocationAndAnnualUpdates: number = ADOrderInputOrderType._13;
}

export class SubscriptionType {
    static AnnualSubscription: number = 1;
    static MultipleLocationEnterprise: number = 2;
    static MLQuarterly: number = 3;
    static MLAnnual: number = 4;
    static SingleLocationEnterprise: number = 5;
    static SLQuarterly: number = 6;
    static SLAnnual: number = 7;
}

export class SubscriptionPlans {
    static AllAccessUnlimited: number = 1;
    static AllAccess: number = 2;
    static Retail: number = 3;
    static Wholesale: number = 4;
}
