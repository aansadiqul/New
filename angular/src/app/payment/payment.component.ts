import { OrderType } from '@shared/AppEnums';
import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    PagedListingComponentBase,
    PagedRequestDto
} from '@shared/paged-listing-component-base';
import {
  PaymentServiceProxy,
  PaymentDto,
  PagedResultDtoOfPaymentDto
} from '@shared/service-proxies/service-proxies';
import {
  CommonService
} from '@shared/service/common.service';
import { Router } from '@angular/router';

class PagedPaymentsRequestDto extends PagedRequestDto {
  keyword: string;
}

@Component({
  selector: 'app-payment',
  animations: [appModuleAnimation()],
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css'],
  styles: [
    `
      mat-form-field {
        padding: 10px;
      }
    `
]
})
export class PaymentComponent extends PagedListingComponentBase<PaymentDto> {
  payments: PaymentDto[] = [];

  keyword = '';

  constructor(
      injector: Injector,
      private _payments: PaymentServiceProxy,
      private _dialog: MatDialog,
      private commonService: CommonService,
      private router: Router
  ) {
      super(injector);
  }

  list(
      request: PagedPaymentsRequestDto,
      pageNumber: number,
      finishedCallback: Function
  ): void {

      request.keyword = this.keyword;

      this._payments
          .getAll(request.maxResultCount, request.skipCount, request.keyword)
          .pipe(
              finalize(() => {
                  finishedCallback();
              })
          )
          .subscribe((result: PagedResultDtoOfPaymentDto) => {
              this.payments = result.items;
              this.showPaging(result, pageNumber);
          });
  }

  delete(payment: PaymentDto): void {

  }

  onClick(paymentId: any, orderType: OrderType): void {
        this.commonService.setId = paymentId;
        if(orderType == OrderType.Ad)
        {
        this.router.navigate(['/app/ad-receipt/' + paymentId]);
        }
        else if(orderType == OrderType.Bd)
        {
          this.router.navigate(['/app/bd-receipt/' + paymentId]);
        }
  }
}
