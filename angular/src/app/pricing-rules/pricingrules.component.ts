import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    PagedListingComponentBase,
    PagedRequestDto
} from '@shared/paged-listing-component-base';
import {
    PricingRuleServiceProxy,
    PricingRuleDto,
    PagedResultDtoOfPricingRuleDto
} from '@shared/service-proxies/service-proxies';
import { CreatePricingRuleDialogComponent } from './create-pricing-rule/create-pricingrule-dialog.component';
import { EditPricingRuleDialogComponent } from './edit-pricing-rule/edit-pricingrule-dialog.component';

class PagedPricingRulesRequestDto extends PagedRequestDto {
    keyword: string;
}

@Component({
    templateUrl: './pricingrules.component.html',
    animations: [appModuleAnimation()],
    styles: [
        `
          mat-form-field {
            padding: 10px;
          }
        `
    ]
})
export class PricingRulesComponent extends PagedListingComponentBase<PricingRuleDto> {
    pricingRules: PricingRuleDto[] = [];

    keyword = '';

    constructor(
        injector: Injector,
        private _pricingRulesService: PricingRuleServiceProxy,
        private _dialog: MatDialog
    ) {
        super(injector);
    }

    list(
        request: PagedPricingRulesRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {

        request.keyword = this.keyword;

        this._pricingRulesService
            .getAll(request.maxResultCount, request.skipCount, request.keyword)
            .pipe(
                finalize(() => {
                    finishedCallback();
                })
            )
            .subscribe((result: PagedResultDtoOfPricingRuleDto) => {
                this.pricingRules = result.items;
                this.showPaging(result, pageNumber);
            });
    }

    delete(pricingRule: PricingRuleDto): void {
        abp.message.confirm(
            this.l('PricingRuleDeleteWarningMessage', pricingRule.title),
            (result: boolean) => {
                if (result) {
                    this._pricingRulesService
                        .delete(pricingRule.id)
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

    createPricingRule(): void {
        this.showCreateOrEditPricingRuleDialog();
    }

    editPricingRule(pricingRule: PricingRuleDto): void {
        this.showCreateOrEditPricingRuleDialog(pricingRule.id);
    }

    showCreateOrEditPricingRuleDialog(id?: number): void {
        let createOrEditPricingRuleDialog;
        if (id === undefined || id <= 0) {
            createOrEditPricingRuleDialog = this._dialog.open(CreatePricingRuleDialogComponent);
        } else {
            createOrEditPricingRuleDialog = this._dialog.open(EditPricingRuleDialogComponent, {
                data: id
            });
        }

        createOrEditPricingRuleDialog.afterClosed().subscribe(result => {
            if (result) {
                this.refresh();
            }
        });
    }
}
