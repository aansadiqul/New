import { Component, Injector, Inject, OnInit, Optional } from '@angular/core';
import {
  MatDialogRef,
  MAT_DIALOG_DATA
} from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import {
  SubscriptionPlanServiceProxy,
  SubscriptionPlanDto,
  PermissionDto
} from '@shared/service-proxies/service-proxies';

@Component({
  templateUrl: 'edit-subscription-plan-dialog.component.html',
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
export class EditSubscriptionPlanDialogComponent extends AppComponentBase
  implements OnInit {
  saving = false;
  subscriptionPlan: SubscriptionPlanDto = new SubscriptionPlanDto();

  constructor(
    injector: Injector,
    private _subscriptionPlanService: SubscriptionPlanServiceProxy,
    private _dialogRef: MatDialogRef<EditSubscriptionPlanDialogComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) private _id: number
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._subscriptionPlanService
      .getAsync(this._id)
      .subscribe((result: SubscriptionPlanDto) => {
        this.subscriptionPlan.init(result);
      });
  }

    save(): void {
    this.saving = true;

    // this.subscriptionPlan.permissions = this.getCheckedPermissions();

    this._subscriptionPlanService
      .update(this.subscriptionPlan)
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
