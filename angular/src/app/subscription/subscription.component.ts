import { Component, OnInit, Injector } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AdOrderTypes, SubscriptionType, OrderType, SubscriptionPlans } from '@shared/AppEnums';
import { ADroles } from '@shared/helpers/commonComponents'
import {
  SubscriptionPlanServiceProxy,
  ADOrderInput,
  CustomerDto,
  CustomerServiceProxy,
  PricingRuleServiceProxy,
  UserServiceProxy,
  PricingRuleDto,
  UserDto,
  SubscriptionPlanDto
} from '@shared/service-proxies/service-proxies';
import { DataService } from '@shared/service/data.service';
import { AbpSessionService } from '@abp/session/abp-session.service';
import * as moment from 'moment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-subscription',
  templateUrl: './subscription.component.html',
  styleUrls: ['./subscription.component.css']
})
export class SubscriptionComponent implements OnInit {
  subTypeID: number = 0;
  isSelected: boolean = true;
  show:boolean = true;
  MyOrderType: string = "AD";
  adOrderTypes: any = [];
  adOrderTypesTotal: any = [];
  adOrderTypesSingle: any = [];
  adOrderTypesMultiple: any = [];
  parameters: any;
  orderForm: FormGroup;
  isImpersonated: boolean = false;
  customerDto: CustomerDto = new CustomerDto();
  aDOrderInput: ADOrderInput = new ADOrderInput();

  subscriptionType: SubscriptionType = new SubscriptionType();
  pricingRuleDto: PricingRuleDto[];
  subsriptionPlan: SubscriptionPlanDto = new SubscriptionPlanDto();
  subsriptionPlans: SubscriptionPlanDto[];
  userDto: UserDto = new UserDto();
  currentDateTime: any;
  isAdORBd: boolean = true;
  isSubscribed: boolean = false;
  expireDate: any;

  //adCustomerDto:any = [];
  constructor(private fb: FormBuilder,
    injector: Injector,
    private _subscriptionPlansService: SubscriptionPlanServiceProxy,
    private _customerServiceProxy: CustomerServiceProxy,
    private _sessionService: AbpSessionService,
    private _pricingRuleServiceProxy: PricingRuleServiceProxy,
    private _userServiceProxy: UserServiceProxy,
    private _dataService: DataService,
    private router: Router
  ) {
    this.orderForm = this.fb.group({
      amount: ['', Validators.required],
      userRole: 'AD Admin',
      subamount: '',
      overrideamount: '',
      overridemonth: ''
    });
  }

  ngOnInit() {

    this._subscriptionPlansService
    .getAllAsync(4, 0, "")
    .subscribe(result => {
      this.subsriptionPlans = result.items;
      this._customerServiceProxy.getCustomerByUserid(this._sessionService.userId).subscribe(items => {
        this.customerDto = items;
        if(this.customerDto.subType)
        {
        this.expireDate = this.customerDto.adExpiresDate;
        this.isSubscribed = true;
        var data = this.subsriptionPlans.filter(x => x.id == this.customerDto.subType);
        this.subsriptionPlan = Object.assign({}, ...data);
        }
        this.show = false;
      });
    });

    this._pricingRuleServiceProxy.getAll(2, 0, "").subscribe(result => {
      this.pricingRuleDto = result.items;
    });

    this._userServiceProxy.get(this._sessionService.userId).subscribe(result => {
      this.userDto = result;
    });
  }

  getChekedOption(subId: any) {
    this.subTypeID = subId;
    this.aDOrderInput.orderValue = this.orderForm.value.amount;
    this.customerDto.byPassCard = 0;
    this.isSelected = false;
  }

  save() {
    this.currentDateTime = moment();
    this.AdOrder();
    localStorage.setItem("customer", JSON.stringify(this.customerDto));
    localStorage.setItem("user", JSON.stringify(this.userDto));
    localStorage.setItem("adOrderInput", JSON.stringify(this.aDOrderInput));
    this._dataService.setData("orderAmount", this.aDOrderInput.orderValue);
    this._dataService.setData("orderNotes", this.aDOrderInput.orderNotes + ' ( ' + this.aDOrderInput.description + ' )');
    this.router.navigate(['/app/checkout']);
  }

  AdOrder() {
    this.userDto.roleNames = ['AD Admin'];
    var intMonth = 12;
    this.customerDto.agencyRec = this.pricingRuleDto[0].noRecords;
    this.customerDto.agencyRecPrice = this.pricingRuleDto[0].perCreditRate;
    this.customerDto.contactRec = this.pricingRuleDto[1].noRecords;
    this.customerDto.contactRecPrice = this.pricingRuleDto[1].perCreditRate;
    this.customerDto.adActiveDate = this.currentDateTime;
    this.customerDto.active = true;
    this.customerDto.adExpiresDate = moment().add(intMonth, 'month');
    //this.aDOrderInput.
    this._dataService.setData("orderType", OrderType.AdSubscription);


    if (this.subTypeID == SubscriptionPlans.AllAccessUnlimited) {
      this.aDOrderInput.orderType = AdOrderTypes.UserSubscription;
      this.customerDto.subType = SubscriptionPlans.AllAccessUnlimited;
      this.aDOrderInput.description = "All Access Unlimited";
    }
    else if (this.subTypeID == SubscriptionPlans.AllAccess) {
      this.aDOrderInput.orderType = AdOrderTypes.UserSubscription;
      this.customerDto.subType = SubscriptionPlans.AllAccess;
      this.aDOrderInput.description = "All Access";
    }
    else if (this.subTypeID == SubscriptionPlans.Retail) {
      this.aDOrderInput.orderType = AdOrderTypes.UserSubscription;
      this.customerDto.subType = SubscriptionPlans.Retail;
      this.aDOrderInput.description = "Retail Access";
    }
    else if (this.subTypeID == SubscriptionPlans.Wholesale) {
      this.aDOrderInput.orderType = AdOrderTypes.UserSubscription;
      this.customerDto.subType = SubscriptionPlans.Wholesale;
      this.aDOrderInput.description = "Wholesale Access";
    }
    this.aDOrderInput.isCtPurchased = false;
    this.aDOrderInput.ctCount = 0
    this.aDOrderInput.recordCount = 0;
    this.aDOrderInput.orderNotes = 'AD Subscription Order';
  }

}
