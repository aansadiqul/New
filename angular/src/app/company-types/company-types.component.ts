import { Component, Injector } from '@angular/core';
import { MatDialog, MatSlideToggleChange } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    PagedListingComponentBase,
    PagedRequestDto
} from '@shared/paged-listing-component-base';
import {
    CompanyTypeServiceProxy,
    CompanyTypeDto,
    PagedResultDtoOfCompanyTypeDto
} from '@shared/service-proxies/service-proxies';

class PagedCompanyTypesRequestDto extends PagedRequestDto {
    keyword: string;
}

@Component({
    templateUrl: './company-types.component.html',
    animations: [appModuleAnimation()],
    styles: [
        `
          mat-form-field {
            padding: 10px;
          }
        `
    ]
})
export class CompanyTypesComponent extends PagedListingComponentBase<CompanyTypeDto> {
    companyTypes: CompanyTypeDto[] = [];
    companyType: CompanyTypeDto = new CompanyTypeDto();
    keyword = '';
    isActive: any | null;
    isAllActive: any | null;
    isLoading:boolean = false;

    constructor(
        injector: Injector,
        private _companyTypesService: CompanyTypeServiceProxy,
        private _dialog: MatDialog
    ) {
        super(injector);
        this.isActive = '';
    }

    list(
        request: PagedCompanyTypesRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {

        request.keyword = this.keyword;

        this._companyTypesService
            .getAllAsync(request.maxResultCount, request.skipCount, request.keyword, this.isActive)
            .pipe(
                finalize(() => {
                    finishedCallback();
                })
            )
            .subscribe((result: PagedResultDtoOfCompanyTypeDto) => {
                this.companyTypes = result.items;
                this.showPaging(result, pageNumber);
            });
    }

    delete(): void {
    }

    refreshData(): void {
        this.isLoading = true;
        this._companyTypesService.updateCompanyTypes().subscribe(() => {
            this.isLoading = false;
            this.refresh();
            this.notify.info(this.l('Data has been refreshed successfully'));
        });
    }

    onChange(event: any, companyType: CompanyTypeDto) {
        if (event.checked) {
            companyType.isActive = true;
        } else {
            companyType.isActive = false;
        }
        this.updateCompanyTypes(companyType);
    }

    onChangeRetail(event: any, companyType: CompanyTypeDto) {
        if (event.checked) {
            companyType.isRetail = true;
        } else {
            companyType.isRetail = false;
        }
        this.updateCompanyTypes(companyType);
    }

    onChangeWholesale(event: any, companyType: CompanyTypeDto) {
        if (event.checked) {
            companyType.isWholesale = true;
        } else {
            companyType.isWholesale = false;
        }
        this.updateCompanyTypes(companyType);
    }

    updateCompanyTypes(companyType: CompanyTypeDto){
        this._companyTypesService
            .update(companyType)
            .subscribe(() => {
                // this.refresh();
                // this.notify.info(this.l('Company types Successfully Updated'));
            });
    }

    onAllActive(event: any) {
        if (event.checked) {
            this.isAllActive = true;
        } else {
            this.isAllActive = false;
        }

        this._companyTypesService
            .updateAll(this.isAllActive)
            .subscribe(() => {
                this.refresh();
                this.notify.info(this.l('Company types Successfully Updated'));
            });
    }
}
