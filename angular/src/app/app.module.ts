import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { JsonpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';
import { ModalModule } from 'ngx-bootstrap';
import { NgxPaginationModule } from 'ngx-pagination';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { AbpModule } from '@abp/abp.module';

import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { SharedModule } from '@shared/shared.module';

import { HomeComponent } from '@app/home/home.component';
import { AboutComponent } from '@app/about/about.component';
import { TopBarComponent } from '@app/layout/topbar.component';
import { TopBarLanguageSwitchComponent } from '@app/layout/topbar-languageswitch.component';
import { SideBarUserAreaComponent } from '@app/layout/sidebar-user-area.component';
import { SideBarNavComponent } from '@app/layout/sidebar-nav.component';
import { SideBarFooterComponent } from '@app/layout/sidebar-footer.component';
import { RightSideBarComponent } from '@app/layout/right-sidebar.component';
// tenants
import { TenantsComponent } from '@app/tenants/tenants.component';
import { CreateTenantDialogComponent } from './tenants/create-tenant/create-tenant-dialog.component';
import { EditTenantDialogComponent } from './tenants/edit-tenant/edit-tenant-dialog.component';
// roles
import { RolesComponent } from '@app/roles/roles.component';
import { CreateRoleDialogComponent } from './roles/create-role/create-role-dialog.component';
import { EditRoleDialogComponent } from './roles/edit-role/edit-role-dialog.component';
// users
import { UsersComponent } from '@app/users/users.component';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { EditUserDialogComponent } from '@app/users/edit-user/edit-user-dialog.component';
import { ChangePasswordComponent } from './users/change-password/change-password.component';
import { ResetPasswordDialogComponent } from './users/reset-password/reset-password.component';
// customers
import { CustomersComponent } from '@app/customers/customers.component';
import { CreateCustomerDialogComponent } from './customers/create-customer/create-customer-dialog.component';
import { EditCustomerDialogComponent } from './customers/edit-customer/edit-customer-dialog.component';
// pricingrules
import { PricingRulesComponent } from '@app/pricing-rules/pricingrules.component';
import { CreatePricingRuleDialogComponent } from './pricing-rules/create-pricing-rule/create-pricingrule-dialog.component';
import { EditPricingRuleDialogComponent } from './pricing-rules/edit-pricing-rule/edit-pricingrule-dialog.component';
// subscription-plans
import { SubscriptionPlansComponent } from '@app/subscription-plans/subscription-plans.component';
// tslint:disable-next-line: max-line-length
import { CreateSubscriptionPlanDialogComponent } from './subscription-plans/create-subscription-plan/create-subscription-plan-dialog.component';
// tslint:disable-next-line: max-line-length
import { EditSubscriptionPlanDialogComponent } from './subscription-plans/edit-subscription-plan/edit-subscription-plan-dialog.component';
import { ADSearchComponent } from './ad-search/ad-search.component';
import { BDSearchComponent } from './bd-search/bd-search.component';
import { CompanyTypesComponent } from './company-types/company-types.component';
import { ContactTitlesComponent } from './contact-titles/contact-titles.component';
import { ProductLinesComponent } from './product-lines/product-lines.component';

import { LoginService } from '../account/login/login.service';
import { AdNewSearchComponent } from './ad-search/ad-new-search/ad-new-search.component';
import { CompanyComponent } from './ad-search/ad-new-search/company/company.component';
import { CarriersComponent } from './ad-search/ad-new-search/carriers/carriers.component';
import { AffiliationsComponent } from './ad-search/ad-new-search/affiliations/affiliations.component';
import { ContactsComponent } from './ad-search/ad-new-search/contacts/contacts.component';
import { GeographicComponent } from './ad-search/ad-new-search/geographic/geographic.component';
import { AngularMultiSelectModule } from 'angular2-multiselect-dropdown';
import { IndustryComponent } from './ad-search/ad-new-search/industry/industry.component';
import { BdNewSearchComponent } from './bd-search/bd-new-search/bd-new-search.component';
import { CompanyBdComponent } from './bd-search/bd-new-search/company/company.component';
import { AdOrderComponent } from './ad-order/ad-order.component';
import { AdReceiptComponent } from './ad-order/ad-receipt/ad-receipt.component';
import { BdOrderComponent } from './bd-order/bd-order.component';
import { AdReviewNamesComponent } from './ad-order/ad-review-names/ad-review-names.component';
import { DataService } from '@shared/service/data.service';
import { NumberDirective } from '@shared/helpers/numbers-only.directive';
import { BdReceiptComponent } from './bd-order/bd-receipt/bd-receipt.component';
import { AdBuyNamesComponent } from './ad-order/ad-buy-names/ad-buy-names.component';
import { OrderComponent } from './customers/order/order.component';
import { OrderProductComponent } from './users/order-product/order-product.component';
import { CheckoutComponent } from './checkout/checkout.component';
import { AdReportsComponent } from './ad-reports/ad-reports.component';
import { AdReportsViewComponent } from './ad-reports/ad-reports-view/ad-reports-view.component';
import { AdReportsViewContactsComponent } from './ad-reports/ad-reports-view-contacts/ad-reports-view-contacts.component';
import { AdReportsViewCarriersComponent } from './ad-reports/ad-reports-view-carriers/ad-reports-view-carriers.component';
import { AdReportsViewSiccodesComponent } from './ad-reports/ad-reports-view-siccodes/ad-reports-view-siccodes.component';
import { AdReportsViewAffiliationsComponent } from './ad-reports/ad-reports-view-affiliations/ad-reports-view-affiliations.component';
import { BdReportViewComponent } from './bd-reports/bd-report-view/bd-report-view.component';
import { BdReportsComponent } from './bd-reports/bd-reports.component';
import { AdReportsMapComponent } from './ad-reports/ad-reports-map/ad-reports-map.component';
import { MapModule, MapAPILoader, WindowRef, DocumentRef, BingMapAPILoaderConfig, BingMapAPILoader } from 'angular-maps';
import { AdReportsMapDetailComponent } from './ad-reports/ad-reports-map/ad-reports-map-detail/ad-reports-map-detail.component';
import { PaymentComponent } from './payment/payment.component';
import { AgencyManagementSystemComponent } from './agency-management-system/agency-management-system.component';
import { CarrierLinesComponent } from './carrier-lines/carrier-lines.component';
import { SpecialAffiliationsComponent } from './special-affiliations/special-affiliations.component';
import { SatDatepickerModule, SatNativeDateModule } from 'saturn-datepicker';
import { SubscriptionComponent } from './subscription/subscription.component';

export function MapServiceProviderFactory() {
  let bc: BingMapAPILoaderConfig = new BingMapAPILoaderConfig();
  bc.apiKey = "AjIwjztEZUGVsVHJ1FXu_Jxg3wsXmErEwQEikWO4-ekKvjr0lCEsWtO-pN7dE3b9"; // your bing map key
  bc.branch = "experimental";
  // to use the experimental bing brach. There are some bug fixes for
  // clustering in that branch you will need if you want to use
  // clustering.
  return new BingMapAPILoader(bc, new WindowRef(), new DocumentRef());
}
@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AboutComponent,
    TopBarComponent,
    TopBarLanguageSwitchComponent,
    SideBarUserAreaComponent,
    SideBarNavComponent,
    SideBarFooterComponent,
    RightSideBarComponent,
    // tenants
    TenantsComponent,
    CreateTenantDialogComponent,
    EditTenantDialogComponent,
    // roles
    RolesComponent,
    CreateRoleDialogComponent,
    EditRoleDialogComponent,
    // users
    UsersComponent,
    CreateUserDialogComponent,
    EditUserDialogComponent,
    ChangePasswordComponent,
    ResetPasswordDialogComponent,
    // Customer
    CustomersComponent,
    CreateCustomerDialogComponent,
    EditCustomerDialogComponent,
    // pricingrules
    PricingRulesComponent,
    CreatePricingRuleDialogComponent,
    EditPricingRuleDialogComponent,
    // subscription-plans
    SubscriptionPlansComponent,
    CreateSubscriptionPlanDialogComponent,
    EditSubscriptionPlanDialogComponent,
    ADSearchComponent,
    BDSearchComponent,
    // company-type
    CompanyTypesComponent,
    // contact-title
    ContactTitlesComponent,
    // product-line
    ProductLinesComponent,
    AdNewSearchComponent,
    CompanyComponent,
    CarriersComponent,
    AffiliationsComponent,
    ContactsComponent,
    GeographicComponent,
    IndustryComponent,
    BdNewSearchComponent,
    CompanyBdComponent,
    AdOrderComponent,
    AdReceiptComponent,
    BdOrderComponent,
    AdReviewNamesComponent,
    NumberDirective,
    BdReceiptComponent,
    AdBuyNamesComponent,
    OrderComponent,
    OrderProductComponent,
    CheckoutComponent,
    AdReportsComponent,
    AdReportsViewComponent,
    AdReportsViewContactsComponent,
    AdReportsViewCarriersComponent,
    AdReportsViewSiccodesComponent,
    AdReportsViewAffiliationsComponent,
    BdReportsComponent,
    BdReportViewComponent,
    AdReportsMapComponent,
    AdReportsMapDetailComponent,
    PaymentComponent,
    AgencyManagementSystemComponent,
    CarrierLinesComponent,
    SpecialAffiliationsComponent,
    SubscriptionComponent
  ],
  imports: [
    CommonModule,
    AngularMultiSelectModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    JsonpModule,
    ModalModule.forRoot(),
    MapModule.forRoot(),
    AbpModule,
    AppRoutingModule,
    ServiceProxyModule,
    SharedModule,
    NgxPaginationModule,
    SatDatepickerModule,
    SatNativeDateModule
  ],
  providers: [LoginService, DataService, {
    provide: MapAPILoader, deps: [], useFactory: MapServiceProviderFactory
  }],
  entryComponents: [
    // tenants
    CreateTenantDialogComponent,
    EditTenantDialogComponent,
    // roles
    CreateRoleDialogComponent,
    EditRoleDialogComponent,
    // users
    CreateUserDialogComponent,
    EditUserDialogComponent,
    ResetPasswordDialogComponent,
    // pricingrule
    CreatePricingRuleDialogComponent,
    EditPricingRuleDialogComponent,
    // subscription-plans
    CreateSubscriptionPlanDialogComponent,
    EditSubscriptionPlanDialogComponent,
  ]
})
export class AppModule { }
