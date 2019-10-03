import { BDSearchDto } from './../../shared/service-proxies/service-proxies';
import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { ADSearchServiceProxy, ADSearchDto, PagedResultDtoOfADSearchDto } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';

class PagedADSearchRequestDto extends PagedRequestDto {
    keyword: string;
    fromDate: string;
    toDate: string;
}

@Component({
    templateUrl: './ad-search.component.html',
    animations: [appModuleAnimation()],
    styles: [
        `
          mat-form-field {
            padding: 10px;
          }
        `
    ]
})
export class ADSearchComponent extends PagedListingComponentBase<ADSearchDto> {
    adsearches: ADSearchDto[] = [];
    keyword = '';
    isActive: boolean | null;
    fromDate = '';
    toDate = '';

    constructor(
        injector: Injector,
        private _adsearchService: ADSearchServiceProxy,
        private router: Router,
        private _dialog: MatDialog
    ) {
        super(injector);
    }

    createADSearch(): void {
        abp.message.info('Create AD Search');
    }

    loadSearch(adSearch: ADSearchDto): void {
        this.router.navigate(['/app/ad-new-search', adSearch.id]);
    }


    protected list(
        request: PagedADSearchRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {

        request.keyword = this.keyword;
        this._adsearchService
            .getAll(request.maxResultCount, request.skipCount, request.keyword, this.fromDate, this.toDate)
            .pipe(
                finalize(() => {
                    finishedCallback();
                })
            )
            .subscribe((result: PagedResultDtoOfADSearchDto) => {
                this.adsearches = result.items;
                this.showPaging(result, pageNumber);
            });
    }

    delete(adsearch: ADSearchDto): void {
        abp.message.confirm(
            this.l('ADSearchDeleteWarningMessage'),
            (result: boolean) => {
                if (result) {
                    // this._adsearchService.delete(adsearch.id).subscribe(() => {
                    //     abp.notify.success(this.l('SuccessfullyDeleted'));
                    //     this.refresh();
                    // });
                    console.log("lskagfkasfdjfwy");
                }
            }
        );
    }

    dateEvent(event: any) {
        this.fromDate = event.target.value.begin.toLocaleDateString();
        this.toDate = event.target.value.end.toLocaleDateString();
    }

    reset() {
        this.keyword = '';
        this.fromDate = '';
        this.toDate = '';
        this.getDataPage(1);
    }
}
