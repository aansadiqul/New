import { Component, Injector, Inject, OnInit, Optional } from '@angular/core';
import {
  MatDialogRef,
  MAT_DIALOG_DATA
} from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import {
  PricingRuleServiceProxy,
  PricingRuleDto,
  PermissionDto
} from '@shared/service-proxies/service-proxies';

@Component({
  templateUrl: 'edit-pricingrule-dialog.component.html',
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
export class EditPricingRuleDialogComponent extends AppComponentBase
  implements OnInit {
  saving = false;
  pricingRule: PricingRuleDto = new PricingRuleDto();

  constructor(
    injector: Injector,
    private _pricingRuleService: PricingRuleServiceProxy,
    private _dialogRef: MatDialogRef<EditPricingRuleDialogComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) private _id: number
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._pricingRuleService
      .get(this._id)
      .subscribe((result: PricingRuleDto) => {
        this.pricingRule.init(result);
      });
  }

    save(): void {
    this.saving = true;

    // this.pricingRule.permissions = this.getCheckedPermissions();

    this._pricingRuleService
      .update(this.pricingRule)
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
