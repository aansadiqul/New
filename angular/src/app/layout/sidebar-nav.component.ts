import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MenuItem } from '@shared/layout/menu-item';

@Component({
    templateUrl: './sidebar-nav.component.html',
    selector: 'sidebar-nav',
    encapsulation: ViewEncapsulation.None
})
export class SideBarNavComponent extends AppComponentBase {

    menuItems: MenuItem[] = [
        new MenuItem('Dashboard', '', 'home', '/app/users'),
        new MenuItem('Tenants', 'Pages.Tenants', 'business', '/app/tenants'),
        new MenuItem('Users', 'Pages.UsersList', 'people', '/app/users'),
        new MenuItem('Customers', 'Pages.Admin', 'people', '/app/customers'),
        new MenuItem('Admin Control', 'Pages.Admin', 'settings_applications', '', [
            new MenuItem('Roles', 'Pages.Roles', '', '/app/roles'),
            new MenuItem('Pricing Rules', 'Pages.Admin', 'fiber_manual_record', '/app/pricingrules'),
            new MenuItem('Subscription Plans', 'Pages.Admin', 'fiber_manual_record', '/app/subscription-plans'),
            new MenuItem('Carriers', 'Pages.Admin', 'fiber_manual_record', '/app/carriers'),
            new MenuItem('Affiliations', 'Pages.Admin', '', '/app/affiliations'),
            new MenuItem('Product Lines', 'Pages.Admin', '', '/app/productLines'),
            new MenuItem('Contact Titles', 'Pages.Admin', '', '/app/contactTitles'),
            new MenuItem('Company Types', 'Pages.Admin', '', '/app/companyTypes'),
            new MenuItem('Agency Management', 'Pages.Admin', '', '/app/agencyManagement')
        ]),
        new MenuItem('AD Saved Search', 'Pages.AD', 'save_alt', '/app/ad-search'),
        new MenuItem('AD New Search', 'Pages.AD', 'search', '/app/ad-new-search'),
        new MenuItem('My AD Orders', 'Pages.AD', 'format_list_bulleted', '/app/ad-orders'),
        new MenuItem('BD New Search', 'Pages.BD', 'search', '/app/bd-new-search'),
        new MenuItem('BD Saved Search', 'Pages.BD', 'save_alt', '/app/bd-search'),
        new MenuItem('My BD Orders', 'Pages.BD', 'format_list_bulleted', '/app/bd-orders'),
        new MenuItem('All Payments', 'Pages.Admin', 'payment', '/app/payments'),
        new MenuItem('Subscription', 'Pages.Users', 'payment', '/app/subscription')
        //new MenuItem('About', '', 'info', '/app/about')
    ];

    constructor(
        injector: Injector
    ) {
        super(injector);
    }

    showMenuItem(menuItem): boolean {
        if (menuItem.permissionName) {
            return this.permission.isGranted(menuItem.permissionName);
        }

        return true;
    }
}
