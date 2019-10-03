import { Component, Injector } from '@angular/core';
import { MatDialog, MatSlideToggleChange } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    PagedListingComponentBase,
    PagedRequestDto
} from '@shared/paged-listing-component-base';
import {
    AMGListServiceProxy,
    AMGListDto,
    PagedResultDtoOfAMGListDto
} from '@shared/service-proxies/service-proxies';

class PagedCompanyTypesRequestDto extends PagedRequestDto {
  keyword: string;
}

@Component({
  templateUrl: './agency-management-system.component.html',
  animations: [appModuleAnimation()],
  styles: [
      `
        mat-form-field {
          padding: 10px;
        }
      `
  ]
})

  export class AgencyManagementSystemComponent extends PagedListingComponentBase<AMGListDto> {
    amgLists: AMGListDto[] = [];
    amgList: AMGListDto = new AMGListDto();
    keyword = '';
    isActive: any | null;
    isAllActive: any | null;
    isLoading:boolean = false;

    constructor(
        injector: Injector,
        private _amgListService: AMGListServiceProxy,
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

        this._amgListService
            .getAllAsync(request.maxResultCount, request.skipCount, request.keyword, this.isActive)
            .pipe(
                finalize(() => {
                    finishedCallback();
                })
            )
            .subscribe((result: PagedResultDtoOfAMGListDto) => {
                this.amgLists = result.items;
                this.showPaging(result, pageNumber);
            });
    }

    delete(): void {
    }

    refreshData(): void {
        this.isLoading = true;
        this._amgListService.updateTypes().subscribe(() => {
            this.isLoading = false;
            this.refresh();
            this.notify.info(this.l('Data has been refreshed successfully'));
        });
    }

    onChange(event: any, amgList: AMGListDto) {
        if (event.checked) {
            amgList.isActive = true;
        } else {
            amgList.isActive = false;
        }

        this._amgListService
            .update(amgList)
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

        this._amgListService
          .updateAll(this.isAllActive)
          .subscribe(() => {
            this.refresh();
            this.notify.info(this.l('Agency Management System Successfully Updated'));
          });
      }
}
