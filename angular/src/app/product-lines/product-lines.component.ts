import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    PagedListingComponentBase,
    PagedRequestDto
} from '@shared/paged-listing-component-base';
import {
    ProductLineServiceProxy,
    ProductLineDto,
    PagedResultDtoOfProductLineDto
} from '@shared/service-proxies/service-proxies';

class PagedProductLinesRequestDto extends PagedRequestDto {
    keyword: string;
}

@Component({
    templateUrl: './product-lines.component.html',
    animations: [appModuleAnimation()],
    styles: [
        `
          mat-form-field {
            padding: 10px;
          }
        `
    ]
})
export class ProductLinesComponent extends PagedListingComponentBase<ProductLineDto> {
    productLines: ProductLineDto[] = [];
    productLine: ProductLineDto = new ProductLineDto();
    keyword = '';
    isActive: any | null;
    isLoading:boolean = false;

    constructor(
        injector: Injector,
        private _productLineService: ProductLineServiceProxy,
        private _dialog: MatDialog
    ) {
        super(injector);
        this.isActive = '';
    }

    list(
        request: PagedProductLinesRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {

        request.keyword = this.keyword;

        this._productLineService
            .getAllAsync(request.maxResultCount, request.skipCount, request.keyword, this.isActive)
            .pipe(
                finalize(() => {
                    finishedCallback();
                })
            )
            .subscribe((result: PagedResultDtoOfProductLineDto) => {
                this.productLines = result.items;
                this.showPaging(result, pageNumber);
            });
    }

    delete(): void {
    }

    refreshData(): void {
        this.isLoading = true;
        this._productLineService.updateTypes().subscribe(() => {
            this.isLoading = false;
            this.refresh();
            this.notify.info(this.l('Data has been refreshed successfully'));
        });
    }

    onChange(event: any, productLine: ProductLineDto) {
        if (event.checked) {
            productLine.isActive = true;
        } else {
            productLine.isActive = false;
        }

        this._productLineService
            .update(productLine)
            .subscribe(() => {
                // this.refresh();
                // this.notify.info(this.l('Data Successfully Updated'));
            });
    }
}
