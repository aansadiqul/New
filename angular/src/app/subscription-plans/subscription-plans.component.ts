import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    PagedListingComponentBase,
    PagedRequestDto
} from '@shared/paged-listing-component-base';
import {
    SubscriptionPlanServiceProxy,
    SubscriptionPlanDto,
    PagedResultDtoOfSubscriptionPlanDto
} from '@shared/service-proxies/service-proxies';
import { CreateSubscriptionPlanDialogComponent } from './create-subscription-plan/create-subscription-plan-dialog.component';
import { EditSubscriptionPlanDialogComponent } from './edit-subscription-plan/edit-subscription-plan-dialog.component';

class PagedSubscriptionPlansRequestDto extends PagedRequestDto {
    keyword: string;
}

@Component({
    templateUrl: './subscription-plans.component.html',
    animations: [appModuleAnimation()],
    styles: [
        `
          mat-form-field {
            padding: 10px;
          }
        `
    ]
})
export class SubscriptionPlansComponent extends PagedListingComponentBase<SubscriptionPlanDto> {
    subscriptionPlans: SubscriptionPlanDto[] = [];

    keyword = '';

    constructor(
        injector: Injector,
        private _subscriptionPlansService: SubscriptionPlanServiceProxy,
        private _dialog: MatDialog
    ) {
        super(injector);
    }

    list(
        request: PagedSubscriptionPlansRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {

        request.keyword = this.keyword;

        this._subscriptionPlansService
            .getAllAsync(request.maxResultCount, request.skipCount, request.keyword)
            .pipe(
                finalize(() => {
                    finishedCallback();
                })
            )
            .subscribe((result: PagedResultDtoOfSubscriptionPlanDto) => {
                this.subscriptionPlans = result.items;
                this.showPaging(result, pageNumber);
            });
    }

    delete(subscriptionPlan: SubscriptionPlanDto): void {
        abp.message.confirm(
            this.l('SubscriptionPlanDeleteWarningMessage', subscriptionPlan.typeName),
            (result: boolean) => {
                if (result) {
                    this._subscriptionPlansService
                        .delete(subscriptionPlan.id)
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

    createSubscriptionPlan(): void {
        this.showCreateOrEditSubscriptionPlanDialog();
    }

    editSubscriptionPlan(subscriptionPlan: SubscriptionPlanDto): void {
        this.showCreateOrEditSubscriptionPlanDialog(subscriptionPlan.id);
    }

    showCreateOrEditSubscriptionPlanDialog(id?: number): void {
        let createOrEditSubscriptionPlanDialog;
        if (id === undefined || id <= 0) {
            createOrEditSubscriptionPlanDialog = this._dialog.open(CreateSubscriptionPlanDialogComponent);
        } else {
            createOrEditSubscriptionPlanDialog = this._dialog.open(EditSubscriptionPlanDialogComponent, {
                data: id
            });
        }

        createOrEditSubscriptionPlanDialog.afterClosed().subscribe(result => {
            if (result) {
                this.refresh();
            }
        });
    }
}
