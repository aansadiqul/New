import { Component, Injector } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  PagedListingComponentBase,
  PagedRequestDto
} from '@shared/paged-listing-component-base';
import {
  CarrierServiceProxy,
  CarrierLineDto,
  PagedResultDtoOfCarrierLineDto
} from '@shared/service-proxies/service-proxies';

class PagedCarriersRequestDto extends PagedRequestDto {
  keyword: string;
}

@Component({
  templateUrl: './carrier-lines.component.html',
  animations: [appModuleAnimation()],
  styles: [
    `
        mat-form-field {
          padding: 10px;
        }
      `
  ]
})
export class CarrierLinesComponent extends PagedListingComponentBase<CarrierLineDto> {
  carriers: CarrierLineDto[] = [];
  contactTitle: CarrierLineDto = new CarrierLineDto();
  keyword = '';
  isActive: any | null;
  isAllActive: any | null;
  isLoading: boolean = false;

  constructor(
    injector: Injector,
    private _carrierLineService: CarrierServiceProxy
  ) {
    super(injector);
    this.isActive = '';
  }

  list(
    request: PagedCarriersRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {

    request.keyword = this.keyword;

    this._carrierLineService
      .getAllAsync(request.maxResultCount, request.skipCount, request.keyword, this.isActive)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: PagedResultDtoOfCarrierLineDto) => {
        this.carriers = result.items;
        this.showPaging(result, pageNumber);
      });
  }

  delete(): void {
  }

  refreshData(): void {
    this.isLoading = true;
    this._carrierLineService.updateTypes().subscribe(() => {
      this.isLoading = false;
      this.refresh();
      this.notify.info(this.l('Data has been refreshed successfully'));
    });
  }

  onChange(event: any, contactTitle: CarrierLineDto) {
    if (event.checked) {
      contactTitle.isActive = true;
    } else {
      contactTitle.isActive = false;
    }

    this._carrierLineService
      .update(contactTitle)
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

    this._carrierLineService
      .updateAll(this.isAllActive)
      .subscribe(() => {
        this.refresh();
        this.notify.info(this.l('Carriers Successfully Updated'));
      });
  }
}
