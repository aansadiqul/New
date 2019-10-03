import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import {
    UserServiceProxy, CustomerServiceProxy, UserProfileDto, PagedResultDtoOfUserProfileDto,
    UserDto, CustomerDto
} from '@shared/service-proxies/service-proxies';
import { CreateUserDialogComponent } from './create-user/create-user-dialog.component';
import { EditUserDialogComponent } from './edit-user/edit-user-dialog.component';
import { Moment } from 'moment';
import { ResetPasswordDialogComponent } from './reset-password/reset-password.component';
import { ADroles } from '@shared/helpers/commonComponents';
import { FormGroup, FormControl, FormBuilder } from '@angular/forms';
import { DataService } from '@shared/service/data.service';
import {Router} from "@angular/router"
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { AppConsts } from '@shared/AppConsts';
import { AuthenticateModel, AuthenticateResultModel, TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { LoginService } from '../../account/login/login.service';
import { ActivatedRoute } from '@angular/router';
import { SubscriptionPlans } from '@shared/AppEnums';
class PagedUsersRequestDto extends PagedRequestDto {
    keyword: string;
    isActive: boolean | null;
    roleId: number | null;
}

@Component({
    templateUrl: './users.component.html',
    animations: [appModuleAnimation()],
    styles: [
        `
          mat-form-field {
            padding: 10px;
          }
        `
    ]
})
export class UsersComponent extends PagedListingComponentBase<UserDto> {
    //usersAD: UserDto[] = [];
    customers: UserProfileDto[] = [];
    customer: CustomerDto = new CustomerDto();
    recordForm: FormGroup;
    ////Record User
    recordPriceId: number = null;
    subTypeId: number = null;
    currentRoleId: number = ADroles.AD_User;
    keyword = '';
    isActive: boolean | null;
    authenticateResult: AuthenticateResultModel;
    loginservice: LoginService;
    serverBaseurl: string = AppConsts.remoteServiceBaseUrl;
    isAdmin: boolean = true;
    hasCreatePermission: boolean = true;
    isAdUser: boolean = true;
    isSuperAdmin : boolean = false;
    isAdminTab : boolean = false;
    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
        private _customerService: CustomerServiceProxy,
        private _loginService: LoginService,
        private _tokenAuthService: TokenAuthServiceProxy,
        private recordFormBuilder: FormBuilder,
        private _dialog: MatDialog,
        private route: ActivatedRoute,
        private _dataService: DataService
    ) {
        super(injector);
        this.isActive = true;
        this.isAdmin = this.route.snapshot.data['permission'] == "Pages.Admin";
        if(this.appSession.userId == 2)
        {
            this.isSuperAdmin = true;
        }
        this.recordForm = this.recordFormBuilder.group({
            agencyNoRecord: new FormControl(),
            agencyRecordPrice: new FormControl(),
            contactNoRecord: new FormControl(),
            contactRecordPrice: new FormControl(),
        })
    }

    createUser(): void {
        this.showCreateOrEditUserDialog();
    }

    editUser(user: UserProfileDto): void {
        this.showCreateOrEditUserDialog(user.userId);
    }
    editPrice(user: UserProfileDto): void {
        this._customerService
            .getCustomerByUserid(user.userId)
            .subscribe(result => {
                this.customer = result;
                this.recordForm.controls.agencyNoRecord.setValue(result.agencyRec);
                this.recordForm.controls.agencyRecordPrice.setValue(result.agencyRecPrice);
                this.recordForm.controls.contactNoRecord.setValue(result.contactRec);
                this.recordForm.controls.contactRecordPrice.setValue(result.contactRecPrice);
            });
    }
    recordSave(): void {
        this.customer.agencyRec = this.recordForm.value.agencyNoRecord;
        this.customer.agencyRecPrice = this.recordForm.value.agencyRecordPrice;
        this.customer.contactRec = this.recordForm.value.contactNoRecord;
        this.customer.contactRecPrice = this.recordForm.value.contactRecordPrice;

        this._customerService
            .update(this.customer)
            .pipe(
                finalize(() => {

                })
            ).subscribe(result2 => {
                this.notify.info(this.l('SavedSuccessfully'));
                //this.refresh();
                $('#PriceEditModal').modal('hide');
            });
    }

    public resetPassword(user: UserProfileDto): void {
        this.showResetPasswordUserDialog(user.userId);
    }
    public changeUserList(Type): void {
        this.isAdminTab = false;
        if(Type.index == 0)
        {
            this.currentRoleId =  ADroles.AD_User;
            this.isAdUser = true;
        }
        else if(Type.index == 1)
        {
            this.currentRoleId =  ADroles.AD_Admin;
            this.isAdUser = true;
        }
        else if(Type.index == 2)
        {
            this.currentRoleId =  ADroles.BD_User;
            this.isAdUser = false;
        }
        else if(Type.index == 3)
        {
            this.currentRoleId =  ADroles.Admin;
            this.isAdUser = false;
            this.isAdminTab = true;
        }
        this.refresh();
    }
    public loginAs(id?: number): void {
        this._tokenAuthService.impersonate(id)
            .subscribe((result: AuthenticateResultModel) => {
                if(this.isAdmin)
                {
                localStorage.setItem("isImpersonated", "1");
                }
                this._loginService.processAuthenticateResult(result);
            });

    }
    protected list(
        request: PagedUsersRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {

        request.keyword = this.keyword;
        request.isActive = this.isActive;
        request.roleId = this.currentRoleId;
        if(this.isAdmin)
        {
            this._customerService
            .getAllCustomers(request.keyword, request.isActive, request.roleId, request.skipCount, request.maxResultCount)
            .pipe(
                finalize(() => {
                    finishedCallback();
                })
            )
            .subscribe((result: PagedResultDtoOfUserProfileDto) => {
                this.customers = result.items;
                this.showPaging(result, pageNumber);
            });
        }
        else{
            this._customerService
            .getUsers(request.keyword, request.isActive, request.roleId, request.skipCount, request.maxResultCount)
            .pipe(
                finalize(() => {
                    finishedCallback();
                })
            )
            .subscribe((result: PagedResultDtoOfUserProfileDto) => {
                this.customers = result.items;
                this._customerService.getCustomerByUserid(this.appSession.userId).subscribe(data => {
                   this.subTypeId = data.subType;
                   if (this.subTypeId != SubscriptionPlans.AllAccessUnlimited) {
                    this.hasCreatePermission = this.customers.length >= 5 ? false : true;
                   }
                   this.showPaging(result, pageNumber);
                });
            });
        }
    }

    protected delete(user: UserDto): void {
        abp.message.confirm(
            this.l('UserDeleteWarningMessage', user.fullName),
            (result: boolean) => {
                if (result) {
                    this._userService.delete(user.id).subscribe(() => {
                        abp.notify.success(this.l('SuccessfullyDeleted'));
                        this.refresh();
                    });
                }
            }
        );
    }

    private showResetPasswordUserDialog(userId?: number): void {
        this._dialog.open(ResetPasswordDialogComponent, {
            data: userId
        });
    }

    private showCreateOrEditUserDialog(id?: number): void {
        this._dataService.setData("isAdmin", this.isAdmin);
        this._dataService.setData("subType", this.subTypeId);
        let createOrEditUserDialog;
        if (id === undefined || id <= 0) {
            createOrEditUserDialog = this._dialog.open(CreateUserDialogComponent, {
                data: this.isAdmin
            });
        } else {
            createOrEditUserDialog = this._dialog.open(EditUserDialogComponent, {
                data: id,
            });
        }

        createOrEditUserDialog.afterClosed().subscribe(result => {
            if (result) {
                this.refresh();
            }
        });
    }
}
