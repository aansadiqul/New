import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    PagedListingComponentBase,
    PagedRequestDto
} from '@shared/paged-listing-component-base';
import {
  BdOrderServiceProxy,
  BdOrderDto,
  PagedResultDtoOfBdOrderDto
} from '@shared/service-proxies/service-proxies';
import {
  CommonService
} from '@shared/service/common.service';
import { Router } from '@angular/router';

class PagedSubscriptionPlansRequestDto extends PagedRequestDto {
  keyword: string;
}

@Component({
  selector: 'app-bd-order',
  animations: [appModuleAnimation()],
  templateUrl: './bd-order.component.html',
  styleUrls: ['./bd-order.component.css'],
  styles: [
    `
      mat-form-field {
        padding: 10px;
      }
    `
]
})
export class BdOrderComponent extends PagedListingComponentBase<BdOrderDto> {
  bdOrders: BdOrderDto[] = [];

  keyword = '';

  constructor(
      injector: Injector,
      private _bdOrders: BdOrderServiceProxy,
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

      this._bdOrders
          .getAll(request.maxResultCount, request.skipCount, request.keyword)
          .pipe(
              finalize(() => {
                  finishedCallback();
              })
          )
          .subscribe((result: PagedResultDtoOfBdOrderDto) => {
              this.bdOrders = result.items;
              this.showPaging(result, pageNumber);
          });
  }


  delete(bdOrder: BdOrderDto): void {

  }

  onClick(orderId: any): void {
    this.commonService.setId = 0;
    this.router.navigate(['/app/bd-receipt/' + orderId]);
  }

  bdOrderView(orderId: any): void{
    this.router.navigate(['/app/bd-reports/' + orderId]);
  }
}
