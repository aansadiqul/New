import { Component, Injector, Inject, OnInit, Optional } from '@angular/core';
import {
  MatDialogRef,
  MAT_DIALOG_DATA
} from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import {
  CustomerServiceProxy,
  CustomerDto,
  PermissionDto
} from '@shared/service-proxies/service-proxies';

@Component({
  templateUrl: 'edit-customer-dialog.component.html',
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
export class EditCustomerDialogComponent extends AppComponentBase
  implements OnInit {
  saving = false;
  customer: CustomerDto = new CustomerDto();

  constructor(
    injector: Injector,
    private _customerService: CustomerServiceProxy,
    private _dialogRef: MatDialogRef<EditCustomerDialogComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) private _id: number
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._customerService
      .get(this._id)
      .subscribe((result: CustomerDto) => {
        this.customer.init(result);
      });
  }

    save(): void {
    this.saving = true;

    // this.customer.permissions = this.getCheckedPermissions();

    this._customerService
      .update(this.customer)
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
