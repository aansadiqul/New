import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    PagedListingComponentBase,
    PagedRequestDto
} from '@shared/paged-listing-component-base';
import {
    CustomerServiceProxy,
    CustomerDto,
    PagedResultDtoOfCustomerDto
} from '@shared/service-proxies/service-proxies';
import { CreateCustomerDialogComponent } from './create-customer/create-customer-dialog.component';
import { EditCustomerDialogComponent } from './edit-customer/edit-customer-dialog.component';

class PagedCustomersRequestDto extends PagedRequestDto {
    keyword: string;
}

@Component({
    templateUrl: './customers.component.html',
    animations: [appModuleAnimation()],
    styles: [
        `
          mat-form-field {
            padding: 10px;
          }
        `
    ]
})
export class CustomersComponent extends PagedListingComponentBase<CustomerDto> {
    customers: CustomerDto[] = [];

    keyword = '';

    constructor(
        injector: Injector,
        private _customersService: CustomerServiceProxy,
        private _dialog: MatDialog
    ) {
        super(injector);
    }

    list(
        request: PagedCustomersRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {

        request.keyword = this.keyword;

        this._customersService
            .getAll(request.maxResultCount, request.skipCount)
            .pipe(
                finalize(() => {
                    finishedCallback();
                })
            )
            .subscribe((result: PagedResultDtoOfCustomerDto) => {
                this.customers = result.items;
                this.showPaging(result, pageNumber);
            });
    }

    delete(customer: CustomerDto): void {
        abp.message.confirm(
            this.l('CustomerDeleteWarningMessage', customer.fName),
            (result: boolean) => {
                if (result) {
                    this._customersService
                        .delete(customer.id)
                        .pipe(
                            finalize(() => {
                                abp.notify.success(this.l('SuccessfullyDeleted'));
                                this.refresh();
                            })
                        )
                        .subscribe(() => { });
                }
            }
        );
    }

    createCustomer(): void {
        this.showCreateOrEditCustomerDialog();
    }

    editCustomer(customer: CustomerDto): void {
        this.showCreateOrEditCustomerDialog(customer.id);
    }

    showCreateOrEditCustomerDialog(id?: number): void {
        let createOrEditCustomerDialog;
        if (id === undefined || id <= 0) {
            createOrEditCustomerDialog = this._dialog.open(CreateCustomerDialogComponent);
        } else {
            createOrEditCustomerDialog = this._dialog.open(EditCustomerDialogComponent, {
                data: id
            });
        }

        createOrEditCustomerDialog.afterClosed().subscribe(result => {
            if (result) {
                this.refresh();
            }
        });
    }
}
