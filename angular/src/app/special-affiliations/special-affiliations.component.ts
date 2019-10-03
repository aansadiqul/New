import { Component, Injector } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    PagedListingComponentBase,
    PagedRequestDto
} from '@shared/paged-listing-component-base';
import {
    AffiliationServiceProxy,
    SpecialAffiliationDto,
    PagedResultDtoOfSpecialAffiliationDto
} from '@shared/service-proxies/service-proxies';

class PagedAffiliationRequestDto extends PagedRequestDto {
    keyword: string;
}

@Component({
    templateUrl: './special-affiliations.component.html',
    animations: [appModuleAnimation()],
    styles: [
        `
          mat-form-field {
            padding: 10px;
          }
        `
    ]
})
export class SpecialAffiliationsComponent extends PagedListingComponentBase<SpecialAffiliationDto> {
    affiliations: SpecialAffiliationDto[] = [];
    affiliation: SpecialAffiliationDto = new SpecialAffiliationDto();
    keyword = '';
    isActive: any | null;
    isAllActive: any | null;
    isLoading: boolean = false;

    constructor(
        injector: Injector,
        private _affiliationService: AffiliationServiceProxy
    ) {
        super(injector);
        this.isActive = '';
    }

    list(
        request: PagedAffiliationRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {

        request.keyword = this.keyword;

        this._affiliationService
            .getAllAsync(request.maxResultCount, request.skipCount, request.keyword, this.isActive)
            .pipe(
                finalize(() => {
                    finishedCallback();
                })
            )
            .subscribe((result: PagedResultDtoOfSpecialAffiliationDto) => {
                this.affiliations = result.items;
                this.showPaging(result, pageNumber);
            });
    }

    delete(): void {
    }

    refreshData(): void {
        this.isLoading = true;
        this._affiliationService.updateTypes().subscribe(() => {
            this.isLoading = false;
            this.refresh();
            this.notify.info(this.l('Data has been refreshed successfully'));
        });
    }

    onChange(event: any, affiliation: SpecialAffiliationDto) {
        if (event.checked) {
            affiliation.isActive = true;
        } else {
            affiliation.isActive = false;
        }

        this._affiliationService
            .update(affiliation)
            .subscribe(() => {
                // this.refresh();
                // this.notify.info(this.l('Data Successfully Updated'));
            });
    }

    onAllActive(event: any) {
        if (event.checked) {
            this.isAllActive = true;
        } else {
            this.isAllActive = false;
        }

        this._affiliationService
            .updateAll(this.isAllActive)
            .subscribe(() => {
                this.refresh();
                this.notify.info(this.l('Affiliations Successfully Updated'));
            });
    }

}
