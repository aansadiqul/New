import { Component, Injector, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import {
  PricingRuleServiceProxy,
  PricingRuleDto,
  CreatePricingRuleInput
} from '@shared/service-proxies/service-proxies';

@Component({
  templateUrl: 'create-pricingrule-dialog.component.html',
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
export class CreatePricingRuleDialogComponent extends AppComponentBase
  implements OnInit {
  saving = false;
  pricingRule: PricingRuleDto = new PricingRuleDto();

  constructor(
    injector: Injector,
    private _pricingRuleService: PricingRuleServiceProxy,
    private _dialogRef: MatDialogRef<CreatePricingRuleDialogComponent>
  ) {
    super(injector);
  }

  ngOnInit(): void {
  }

  save(): void {
    this.saving = true;

    const pricingRule_ = new CreatePricingRuleInput();
    pricingRule_.init(this.pricingRule);

    this._pricingRuleService
      .create(pricingRule_)
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
