import { Component, Injector, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import {
  SubscriptionPlanServiceProxy,
  SubscriptionPlanDto,
  CreateSubscriptionPlanInput
} from '@shared/service-proxies/service-proxies';

@Component({
  templateUrl: 'create-subscription-plan-dialog.component.html',
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
export class CreateSubscriptionPlanDialogComponent extends AppComponentBase
  implements OnInit {
  saving = false;
  subscriptionPlan: SubscriptionPlanDto = new SubscriptionPlanDto();

  constructor(
    injector: Injector,
    private _subscriptionPlanService: SubscriptionPlanServiceProxy,
    private _dialogRef: MatDialogRef<CreateSubscriptionPlanDialogComponent>
  ) {
    super(injector);
  }

  ngOnInit(): void {
  }

  save(): void {
    this.saving = true;

    const subscriptionPlan_ = new CreateSubscriptionPlanInput();
    subscriptionPlan_.init(this.subscriptionPlan);

    this._subscriptionPlanService
      .createAsync(subscriptionPlan_)
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
