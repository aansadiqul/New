import { NgModule } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AbpHttpInterceptor } from '@abp/abpHttpInterceptor';

import * as ApiServiceProxies from './service-proxies';

@NgModule({
    providers: [
        ApiServiceProxies.RoleServiceProxy,
        ApiServiceProxies.SessionServiceProxy,
        ApiServiceProxies.TenantServiceProxy,
        ApiServiceProxies.UserServiceProxy,
        ApiServiceProxies.TokenAuthServiceProxy,
        ApiServiceProxies.AccountServiceProxy,
        ApiServiceProxies.ConfigurationServiceProxy,
        ApiServiceProxies.PricingRuleServiceProxy,
        ApiServiceProxies.CustomerServiceProxy,
        ApiServiceProxies.SubscriptionPlanServiceProxy,
        ApiServiceProxies.ADSearchServiceProxy,
        ApiServiceProxies.BDSearchServiceProxy,
        ApiServiceProxies.CompanyTypeServiceProxy,
        ApiServiceProxies.ProductLineServiceProxy,
        ApiServiceProxies.ContactTitleServiceProxy,
        ApiServiceProxies.CommonServiceProxy,
        ApiServiceProxies.ADOrderServiceProxy,
        ApiServiceProxies.BdOrderServiceProxy,
        ApiServiceProxies.PaymentServiceProxy,
        ApiServiceProxies.AMGListServiceProxy,
        ApiServiceProxies.AffiliationServiceProxy,
        ApiServiceProxies.CarrierServiceProxy,
        { provide: HTTP_INTERCEPTORS, useClass: AbpHttpInterceptor, multi: true }
    ]
})
export class ServiceProxyModule { }
