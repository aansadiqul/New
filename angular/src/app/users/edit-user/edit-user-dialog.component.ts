import { Component, Injector, Optional, Inject, OnInit } from '@angular/core';
import { CommonComponentsHelper } from '@shared/helpers/commonComponents';
import {
  MAT_DIALOG_DATA,
  MatDialogRef,
  MatCheckboxChange
} from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import {
  UserServiceProxy,
  UserDto,
  CustomerServiceProxy,
  CustomerDto,
  RoleDto,
  FileParameter
} from '@shared/service-proxies/service-proxies';
import { DataService } from '@shared/service/data.service';
@Component({
  templateUrl: './edit-user-dialog.component.html',
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
export class EditUserDialogComponent extends AppComponentBase
  implements OnInit {
  saving = false;

  user: UserDto = new UserDto();
  customer: CustomerDto = new CustomerDto();
  isSalesperson: boolean = true;

  roles: RoleDto[] = [];
  checkedRolesMap: { [key: string]: boolean } = {};
  isAdmin: boolean = true;
  parameters: any;
  ///For Country & States retrieval
  commonVar=new CommonComponentsHelper();
  countryList: any = this.commonVar.countryList;
  states: any = this.commonVar.defaultStates;


  //////// Image Upload///
  url : any ='';
  image : any = null;
  uploadImg : FileParameter = null;

  constructor(
    injector: Injector,
    private _dataService: DataService,
    public _userService: UserServiceProxy,
    public _customerService: CustomerServiceProxy,
    private _dialogRef: MatDialogRef<EditUserDialogComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) private _id: number
  ) {
    super(injector);
  }
  countryLevelChangeAction(country) {
    this.states =this.commonVar.countryLevelChangeAction(country);
  }
  ngOnInit(): void {
    this.parameters = this._dataService.getData();
    this.isAdmin = this.parameters.isAdmin;
    this._userService.get(this._id).subscribe(result => {
      this.user = result;

      this._userService.getRoles().subscribe(result2 => {
        this.roles = result2.items;
        this.setInitialRolesStatus();
      });
      ///Get Customer
      this._customerService.getCustomerByUserid(this._id).subscribe(result3 => {
        this.customer = result3;
      });

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
    return _.includes(this.user.roleNames, normalizedName);
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
    _.forEach(this.checkedRolesMap, function(value, key) {
      if (value) {
        roles.push(key);
      }
    });
    return roles;
  }

  save(): void {
    this.saving = true;

    this.user.roleNames = this.getCheckedRoles();

    this._userService
      .update(this.user)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(result => {
        this.customer.fName=result.fullName;
        this.customer.lName=result.surname;
        this.customer.email=result.emailAddress;

        console.log(this.customer);
        this._customerService
        .update(this.customer)
        .pipe(
          finalize(() => {
            this.saving = false;
          })
        ).subscribe(result2 => {});

            if(this.image)
            {
               this._customerService
              .postImage(this.customer.userId, this.uploadImg)
              .subscribe(result => {  });
            }
            this.notify.info(this.l('SavedSuccessfully'));
            this.close(true);
       });
     /* .subscribe(() => {

        this.notify.info(this.l('SavedSuccessfully'));
        this.close(true);
      });*/
  }

  close(result: any): void {
    this._dialogRef.close(result);
  }


  onSelectFile(event) {
    if (event.target.files && event.target.files[0]) {
      var reader = new FileReader();


      this.image = event.target.files[0];
      this.uploadImg = { data:  event.target.files[0], fileName:  event.target.files[0].name }  as FileParameter;

      reader.readAsDataURL(event.target.files[0]); // read file as data url
      reader.onload = (event : Event) => { // called once readAsDataURL is completed
        console.log(event);
        this.url = reader.result;
      }
    }
  }
}
