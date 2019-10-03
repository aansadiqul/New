import { SalespersonDto } from './../../../shared/service-proxies/service-proxies';
import { Component, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MatCheckboxChange } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { CommonComponentsHelper } from '@shared/helpers/commonComponents';
import {
  UserServiceProxy,
  CreateUserDto,
  CustomerServiceProxy,
  CreateCustomerDto,
  RoleDto,
  CustomerDto
} from '@shared/service-proxies/service-proxies';
import { DataService } from '@shared/service/data.service';
import * as moment from 'moment';
import { ActivatedRoute } from '@angular/router';
import { SubscriptionPlans } from '@shared/AppEnums';
@Component({
  templateUrl: './create-user-dialog.component.html',
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
export class CreateUserDialogComponent extends AppComponentBase
  implements OnInit {
  saving = false;
  user: CreateUserDto = new CreateUserDto();
  customer: CreateCustomerDto = new CreateCustomerDto();
  adAdminProfile: CustomerDto = new CustomerDto();
  
  isSalesperson: boolean = true;
  currentDateTime = moment();
  AdExpireDate = moment().add(1, 'year');
  //console.log();

  roles: RoleDto[] = [];
  checkedRolesMap: { [key: string]: boolean } = {};
  defaultRoleCheckedStatus = false;

  ///For Country & States retrieval
  states: any = [{ value: "", text: "Select State" }];
  commonVar = new CommonComponentsHelper();
  countryList: any = this.commonVar.countryList;
  salespersons: SalespersonDto[] = [];
  isAdmin: boolean = true;
  subType: number;
  parameters: any;
  constructor(
    injector: Injector,
    public _userService: UserServiceProxy,
    public _customerService: CustomerServiceProxy,
    private _dialogRef: MatDialogRef<CreateUserDialogComponent>,
    private _dataService: DataService
  ) {
    super(injector);

  }
  countryLevelChangeAction(country) {
    this.states = this.commonVar.countryLevelChangeAction(country);
  }
  ngOnInit(): void {
    this.user.isActive = true;
    ///Set Default country & State value
    this.customer.country = "United States"
    this.states =this.commonVar.countryLevelChangeAction(this.customer.country);

    this.parameters = this._dataService.getData();
    this.isAdmin = this.parameters.isAdmin;
    this._userService.getRoles().subscribe(result => {
      this.roles = result.items;
      this.setInitialRolesStatus();
    });

    if (!this.isAdmin) {
      this._customerService.getCustomerByUserid(this.appSession.userId).subscribe(data => {
        this.adAdminProfile = data;
      });
    }
    this._customerService.getSalesperson().subscribe(result => {
      this.salespersons = result;
    });
  }

  setInitialRolesStatus(): void {
    _.map(this.roles, item => {
      this.checkedRolesMap[item.normalizedName] = this.isRoleChecked(
        item.normalizedName
      );
    });
  }

  isRoleChecked(normalizedName: string): boolean {
    // just return default role checked status
    // it's better to use a setting    
    return this.defaultRoleCheckedStatus;
  }

  onRoleChange(role: RoleDto, $event: MatCheckboxChange) {
    if($event.checked){
      this.isSalesperson = role.name == 'Admin' ? false : true;      
    }
    else
    {
      this.isSalesperson = true;
    }
    this.checkedRolesMap[role.normalizedName] = $event.checked;
  }

  getCheckedRoles(): string[] {
    const roles: string[] = [];
    _.forEach(this.checkedRolesMap, function (value, key) {
      if (value) {
        roles.push(key);
      }
    });
    return roles;
  }

  save(): void {
    this.saving = true;
    if (this.isAdmin) {
      this.user.roleNames = this.getCheckedRoles();
    }
    else {
      this.user.roleNames = ['AD USER'];
      this.customer.subType = this.adAdminProfile.subType;
      this.customer.agencyRec = this.adAdminProfile.agencyRec;
      this.customer.agencyRecPrice = this.adAdminProfile.agencyRecPrice;
      this.customer.contactRec = this.adAdminProfile.contactRec;
      this.customer.contactRecPrice = this.adAdminProfile.contactRecPrice;
    }


    if (this.user.roleNames.indexOf('AD USER') != -1) {
      this.customer.adActiveDate = this.currentDateTime;
      this.customer.adExpiresDate = this.AdExpireDate;
    }

    if (this.user.roleNames.indexOf('BD USER') != -1) {
      this.customer.bdActiveDate = this.currentDateTime;
    }

    if (this.user.roleNames.indexOf('ADMIN') != -1) {
      this.customer.subType = SubscriptionPlans.AllAccessUnlimited;
    }

    this._userService
      .create(this.user)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(result => {
        console.log(result.id);
        this.customer.userId = result.id;
        this.customer.fName = result.name;
        this.customer.lName = result.surname;
        this.customer.email = result.emailAddress;
        this._customerService
          .create(this.customer)
          .pipe(
            finalize(() => {
              this.saving = false;
            })
          ).subscribe(result2 => { });
        this.notify.info(this.l('Saved Successfully'));
        this.close(true);
      });
  }

  close(result: any): void {
    this._dialogRef.close(result);
  }
}
