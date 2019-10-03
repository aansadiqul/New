import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { BDSearchServiceProxy, BDSearchDto, PagedResultDtoOfBDSearchDto } from '@shared/service-proxies/service-proxies';
import { Moment } from 'moment';
import { Router } from '@angular/router';

class PagedBDSearchRequestDto extends PagedRequestDto {
    keyword: string;
}

@Component({
    templateUrl: './bd-search.component.html',
    animations: [appModuleAnimation()],
    styles: [
        `
          mat-form-field {
            padding: 10px;
          }
        `
      ]
})
export class BDSearchComponent extends PagedListingComponentBase<BDSearchDto> {
    bdsearches: BDSearchDto[] = [];
    keyword = '';
    isActive: boolean | null;
    fromDate = '';
    toDate = '';

    constructor(
        injector: Injector,
        private _bdsearchService: BDSearchServiceProxy,
        private router: Router,
        private _dialog: MatDialog
    ) {
        super(injector);
    }

    createBDSearch(): void {
      abp.message.info('Create AD Search');
    }

    loadSearch(bdsearch: BDSearchDto): void {
        this.router.navigate(['/app/bd-new-search',bdsearch.id]);
  }


    protected list(
        request: PagedBDSearchRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {

        request.keyword = this.keyword;
        this._bdsearchService
            .getAll(request.maxResultCount, request.skipCount, request.keyword, this.fromDate, this.toDate )
            .pipe(
                finalize(() => {
                    finishedCallback();
                })
            )
            .subscribe((result: PagedResultDtoOfBDSearchDto) => {
                this.bdsearches = result.items;
                this.showPaging(result, pageNumber);
            });
    }

    protected delete(bdsearch: BDSearchDto): void {
        abp.message.confirm(
            this.l('BDSearchDeleteWarningMessage', bdsearch.searchName),
            (result: boolean) => {
                if (result) {
                    this._bdsearchService.delete(bdsearch.id).subscribe(() => {
                        abp.notify.success(this.l('SuccessfullyDeleted'));
                        this.refresh();
                    });
                }
            }
        );
    }

    dateEvent(event: any) {
        this.fromDate = event.target.value.begin.toLocaleDateString();
        this.toDate = event.target.value.end.toLocaleDateString();
      }

    reset(){
        this.keyword = '';
        this.fromDate = '';
        this.toDate = '';
        this.getDataPage(1);
    }
}
