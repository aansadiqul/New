import { SubscriptionComponent } from './subscription/subscription.component';
import { CarrierLinesComponent } from './carrier-lines/carrier-lines.component';
import { AgencyManagementSystemComponent } from './agency-management-system/agency-management-system.component';
import { BdReportViewComponent } from './bd-reports/bd-report-view/bd-report-view.component';
import { BdReportsComponent } from './bd-reports/bd-reports.component';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about.component';
import { UsersComponent } from './users/users.component';
import { TenantsComponent } from './tenants/tenants.component';
import { RolesComponent } from 'app/roles/roles.component';
import { PricingRulesComponent } from '@app/pricing-rules/pricingrules.component';
import { SubscriptionPlansComponent } from '@app/subscription-plans/subscription-plans.component';
import { ChangePasswordComponent } from './users/change-password/change-password.component';
import { ADSearchComponent } from './ad-search/ad-search.component';
import { BDSearchComponent } from './bd-search/bd-search.component';
import { AdNewSearchComponent } from '@app/ad-search/ad-new-search/ad-new-search.component';
import { BdNewSearchComponent } from './bd-search/bd-new-search/bd-new-search.component';
import { AdOrderComponent } from './ad-order/ad-order.component';
import { AdReceiptComponent } from './ad-order/ad-receipt/ad-receipt.component';
import { BdOrderComponent } from './bd-order/bd-order.component';
import { BdReceiptComponent } from './bd-order/bd-receipt/bd-receipt.component';
import { AdReviewNamesComponent } from './ad-order/ad-review-names/ad-review-names.component';
import { AdBuyNamesComponent } from './ad-order/ad-buy-names/ad-buy-names.component';
import { OrderProductComponent } from './users/order-product/order-product.component';
import { CheckoutComponent } from './checkout/checkout.component';
import { AdReportsComponent } from './ad-reports/ad-reports.component';
import { AdReportsViewComponent } from './ad-reports/ad-reports-view/ad-reports-view.component';
import { AdReportsMapComponent } from './ad-reports/ad-reports-map/ad-reports-map.component';
import { AdReportsMapDetailComponent } from './ad-reports/ad-reports-map/ad-reports-map-detail/ad-reports-map-detail.component';
import { PaymentComponent } from './payment/payment.component';
import { ContactTitlesComponent } from './contact-titles/contact-titles.component';
import { ProductLinesComponent } from './product-lines/product-lines.component';
import { CompanyTypesComponent } from './company-types/company-types.component';
import { SpecialAffiliationsComponent } from './special-affiliations/special-affiliations.component';
@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'home', component: UsersComponent, data: { permission: 'Pages.Admin' }, canActivate: [AppRouteGuard] },
                    { path: 'customers', component: UsersComponent, data: { permission: 'Pages.Admin' }, canActivate: [AppRouteGuard] },
                    { path: 'users', component: UsersComponent, data: { permission: 'Pages.UsersList' }, canActivate: [AppRouteGuard] },
                    ///Page After Signup///
                    { path: 'subscription', component: SubscriptionComponent, data: { permission: 'Pages.BD' }, canActivate: [AppRouteGuard] },

                    { path: 'roles', component: RolesComponent, data: { permission: 'Pages.Roles' }, canActivate: [AppRouteGuard] },
                    { path: 'tenants', component: TenantsComponent, data: { permission: 'Pages.Roles' }, canActivate: [AppRouteGuard] },
                    { path: 'pricingrules', component: PricingRulesComponent, data: { permission: 'Pages.Admin' }, canActivate: [AppRouteGuard] },
                    { path: 'subscription-plans', component: SubscriptionPlansComponent, data: { permission: 'Pages.Admin' }, canActivate: [AppRouteGuard] },
                    { path: 'ad-search', component: ADSearchComponent, data: { permission: 'Pages.AD' }, canActivate: [AppRouteGuard] },
                    { path: 'ad-new-search', component: AdNewSearchComponent, data: { permission: 'Pages.AD' }, canActivate: [AppRouteGuard] },
                    { path: 'ad-new-search/:id', component: AdNewSearchComponent, data: { permission: 'Pages.AD' }, canActivate: [AppRouteGuard] },
                    { path: 'ad-orders', component: AdOrderComponent, data: { permission: 'Pages.AD' }, canActivate: [AppRouteGuard] },
                    { path: 'ad-receipt/:id', component: AdReceiptComponent, data: { permission: 'Pages.AD' }, canActivate: [AppRouteGuard] },
                    { path: 'ad-names', component: AdReviewNamesComponent, data: { permission: 'Pages.AD' }, canActivate: [AppRouteGuard] },
                    { path: 'ad-buy-names', component: AdBuyNamesComponent, data: { permission: 'Pages.AD' }, canActivate: [AppRouteGuard] },
                    { path: 'ad-reports/:id', component: AdReportsComponent, data: { permission: 'Pages.AD' }, canActivate: [AppRouteGuard] },
                    { path: 'ad-reports-view/:id', component: AdReportsViewComponent, data: { permission: 'Pages.AD' }, canActivate: [AppRouteGuard] },
                    { path: 'ad-reports-map/:id', component: AdReportsMapComponent, data: { permission: 'Pages.AD' }, canActivate: [AppRouteGuard] },
                    { path: 'ad-reports-accountDetail/:id', component: AdReportsMapDetailComponent, data: { permission: 'Pages.AD' }, canActivate: [AppRouteGuard] },

                    { path: 'bd-search', component: BDSearchComponent, data: { permission: 'Pages.BD' }, canActivate: [AppRouteGuard] },
                    { path: 'bd-new-search', component: BdNewSearchComponent, data: { permission: 'Pages.BD' }, canActivate: [AppRouteGuard] },
                    { path: 'bd-new-search/:id', component: BdNewSearchComponent, data: { permission: 'Pages.BD' }, canActivate: [AppRouteGuard] },
                    { path: 'bd-orders', component: BdOrderComponent, data: { permission: 'Pages.BD' }, canActivate: [AppRouteGuard] },
                    { path: 'bd-receipt/:id', component: BdReceiptComponent, data: { permission: 'Pages.BD' }, canActivate: [AppRouteGuard] },
                    { path: 'bd-reports/:id', component: BdReportsComponent, data: { permission: 'Pages.BD' }, canActivate: [AppRouteGuard] },
                    { path: 'bd-report-view/:id', component: BdReportViewComponent, data: { permission: 'Pages.BD' }, canActivate: [AppRouteGuard] },
                    { path: 'checkout', component: CheckoutComponent, data: { permission: 'Pages.Users' }, canActivate: [AppRouteGuard]},
                    { path: 'about', component: AboutComponent },
                    { path: 'update-password', component: ChangePasswordComponent , data: { permission: 'Pages.Users' }, canActivate: [AppRouteGuard]},
                    { path: 'payments', component: PaymentComponent , data: { permission: 'Pages.Admin' }, canActivate: [AppRouteGuard]},
                    { path: 'companyTypes', component: CompanyTypesComponent, data: { permission: 'Pages.Admin' }, canActivate: [AppRouteGuard] },
                    { path: 'contactTitles', component: ContactTitlesComponent, data: { permission: 'Pages.Admin' }, canActivate: [AppRouteGuard] },
                    { path: 'subscription-plans', component: SubscriptionPlansComponent, data: { permission: 'Pages.Admin' }, canActivate: [AppRouteGuard] },
                    { path: 'productLines', component: ProductLinesComponent, data: { permission: 'Pages.Admin' }, canActivate: [AppRouteGuard] },
                    { path: 'agencyManagement', component: AgencyManagementSystemComponent, data: { permission: 'Pages.Admin' }, canActivate: [AppRouteGuard] },
                    { path: 'carriers', component: CarrierLinesComponent, data: { permission: 'Pages.Admin' }, canActivate: [AppRouteGuard] },
                    { path: 'affiliations', component: SpecialAffiliationsComponent, data: { permission: 'Pages.Admin' }, canActivate: [AppRouteGuard] }
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
