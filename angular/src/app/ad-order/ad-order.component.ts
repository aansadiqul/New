import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    PagedListingComponentBase,
    PagedRequestDto
} from '@shared/paged-listing-component-base';
import {
  ADOrderServiceProxy,
  ADOrderDto,
  PagedResultDtoOfADOrderDto
} from '@shared/service-proxies/service-proxies';
import {
  CommonService
} from '@shared/service/common.service';
import { Router } from '@angular/router';

class PagedSubscriptionPlansRequestDto extends PagedRequestDto {
  keyword: string;
}

@Component({
  selector: 'app-ad-order',
  animations: [appModuleAnimation()],
  templateUrl: './ad-order.component.html',
  styleUrls: ['./ad-order.component.css'],
  styles: [
    `
      mat-form-field {
        padding: 10px;
      }
    `
]
})
export class AdOrderComponent extends PagedListingComponentBase<ADOrderDto> {
  adOrders: ADOrderDto[] = [];

  keyword = '';

  constructor(
      injector: Injector,
      private _adOrders: ADOrderServiceProxy,
      private _dialog: MatDialog,
      private commonService: CommonService,
      private router: Router
  ) {
      super(injector);
  }

  list(
      request: PagedSubscriptionPlansRequestDto,
      pageNumber: number,
      finishedCallback: Function
  ): void {

      request.keyword = this.keyword;

      this._adOrders
          .getAll(request.maxResultCount, request.skipCount, request.keyword)
          .pipe(
              finalize(() => {
                  finishedCallback();
              })
          )
          .subscribe((result: PagedResultDtoOfADOrderDto) => {
              this.adOrders = result.items;
              this.showPaging(result, pageNumber);
          });
  }


  delete(adOrder: ADOrderDto): void {

  }
  adOrderView(orderId: any): void{
    this.router.navigate(['/app/ad-reports/' + orderId]);
  }

  onClick(orderId: any): void {
        this.commonService.setId = 0;
        this.router.navigate(['/app/ad-receipt/' + orderId]);
  }
}
