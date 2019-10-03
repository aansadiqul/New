import { Component, Injector, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import {
  CustomerServiceProxy,
  CustomerDto,
  CreateCustomerDto
} from '@shared/service-proxies/service-proxies';

@Component({
  templateUrl: 'create-customer-dialog.component.html',
  styles: [
    `
      mat-form-field {
        width: 100%;
      }
      mat-checkbox {
        padding-bottom: 5px;
      }
    `
  ]
})
export class CreateCustomerDialogComponent extends AppComponentBase
  implements OnInit {
  saving = false;
  customer: CustomerDto = new CustomerDto();

  constructor(
    injector: Injector,
    private _customerService: CustomerServiceProxy,
    private _dialogRef: MatDialogRef<CreateCustomerDialogComponent>
  ) {
    super(injector);
  }

  ngOnInit(): void {
  }

  save(): void {
    this.saving = true;

    const customer_ = new CreateCustomerDto();
    customer_.init(this.customer);

    this._customerService
      .create(customer_)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close(true);
      });
  }

  close(result: any): void {
    this._dialogRef.close(result);
  }
}
