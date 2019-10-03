import { Component, OnInit,Injector } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AdOrderTypes,SubscriptionType,OrderType } from '@shared/AppEnums';
import { ADroles } from '@shared/helpers/commonComponents'
import {
  SubscriptionPlanServiceProxy,
  ADOrderInput,
  CustomerDto,
  CustomerServiceProxy,
  PricingRuleServiceProxy,
  UserServiceProxy,
  PricingRuleDto,
  UserDto
} from '@shared/service-proxies/service-proxies';
import { DataService } from '@shared/service/data.service';
import { AbpSessionService } from '@abp/session/abp-session.service';
import * as moment from 'moment';
import { Router } from '@angular/router';
@Component({
  selector: 'app-order-product',
  templateUrl: './order-product.component.html',
  styleUrls: ['./order-product.component.css']
})

export class OrderProductComponent implements OnInit {
  quarter: string;
  mquarter: string;
  isLiscenceSingle:  boolean = false;
  isLiscenceMultiple:  boolean = false;
  subTypeID : number = 0;

  MyOrderType : string = "AD";
  adOrderTypes: any = [];
  adOrderTypesTotal: any = [];
  adOrderTypesSingle: any = [];
  adOrderTypesMultiple: any = [];
  parameters: any;
  orderForm: FormGroup;
  isImpersonated: boolean = false;

  customerDto : CustomerDto =  new CustomerDto();
  customerDtoTemp : CustomerDto =  new CustomerDto();

  aDOrderInput: ADOrderInput = new ADOrderInput();

  subscriptionType : SubscriptionType = new SubscriptionType();
  pricingRuleDto : PricingRuleDto[];

  userDto: UserDto = new UserDto();


  roleNames:any = [];
  roleId: number = ADroles.AD_User;
  currentDateTime:any;
  isAdORBd:boolean = true;
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
  )
  {
    this.orderForm = this.fb.group({
      amount: ['', Validators.required],
      userRole: "AD User",
      subamount: '',
      overrideamount: '',
      overridemonth: ''
    });
  }

  ngOnInit() {
    this.parameters = localStorage.getItem("isImpersonated");
    this._customerServiceProxy.getCustomerByUserid(this._sessionService.userId).subscribe(result => {
          this.customerDto = result;
          this.customerDtoTemp = result;
          this._pricingRuleServiceProxy.getAll(2,0,"").subscribe(result => {
            console.log(result);
            this.pricingRuleDto = result.items;

          });
    });

    this._userServiceProxy.get(this._sessionService.userId).subscribe(result => {
      this.userDto =  result;
    });
    this._userServiceProxy.getRoles().subscribe(result => {
      this.roleNames.push(result.items[1]);
      this.roleNames.push(result.items[2]);
    });

    //console.log(this._sessionService.userId);
    this._subscriptionPlansService
        .getAllAsync(12, 0, "")
        .subscribe(result => {
        for (let i = 0; i < result.items.length; i++) {
           let item = result.items[i];
           if(item.id ==  SubscriptionType.AnnualSubscription || item.id == SubscriptionType.MultipleLocationEnterprise ||  item.id == SubscriptionType.SingleLocationEnterprise || item.id == SubscriptionType.SLQuarterly)
           {
              this.adOrderTypes.push(item);
           }
           if(item.id == SubscriptionType.SLQuarterly || item.id == SubscriptionType.SLAnnual)
           {
             this.adOrderTypesSingle.push(item);
           }
           if(item.id == SubscriptionType.MLQuarterly || item.id == SubscriptionType.MLAnnual)
           {
             this.adOrderTypesMultiple.push(item);
           }
           if(item.id == SubscriptionType.MLQuarterly || item.id == SubscriptionType.MLAnnual)
           {
             this.adOrderTypesMultiple.push(item);
           }
        }
        this.adOrderTypesTotal = this.adOrderTypes;

    });


  }
  getSubscriptionType(id:number){
    this.roleId = id;
    if(this.roleId == ADroles.AD_User)
    {
      this.isAdORBd = true;
    }
    else
    {
      this.isAdORBd = false;
    }
  }
  getChekedOption(x: any) {
    if (x == SubscriptionType.SingleLocationEnterprise) {
        this.isLiscenceSingle = true;
        this.isLiscenceMultiple = false;
        this.isImpersonated = false;
        this.orderForm.controls['subamount'].setValue("");
    }
    else if (x == SubscriptionType.MultipleLocationEnterprise) {
      this.isLiscenceSingle = false;
      this.isLiscenceMultiple = true;
      this.isImpersonated = false;
      this.orderForm.controls['subamount'].setValue("");
    }
    else {
      this.isLiscenceSingle = false;
      this.isLiscenceMultiple = false;
      this.orderForm.controls['subamount'].setValue("");
      if(localStorage.getItem("isImpersonated"))
      {
        if(localStorage.getItem("isImpersonated") == "1")
        {
          this.isImpersonated = true;
        }
      }

    }
 }
 getChekedSubOption(y: any) {
   this.subTypeID = y;
 }
 save(){
   this.currentDateTime = moment();
   if(this.roleId == ADroles.AD_User)
   {
        this.AdOrder();
        localStorage.setItem("customer",JSON.stringify(this.customerDto));
        localStorage.setItem("user",JSON.stringify(this.userDto));
        localStorage.setItem("adOrderInput",JSON.stringify(this.aDOrderInput));
        this._dataService.setData("orderAmount",  this.aDOrderInput.orderValue);
        this._dataService.setData("orderNotes",  this.aDOrderInput.orderNotes + ' ( ' + this.aDOrderInput.description + ' )');


        this.router.navigate(['/app/checkout']);
   }
   else
   {
        this.BdOrder();
   }



   //this.orderForm.reset()
 }

 AdOrder(){
  this.aDOrderInput.orderValue = 0;
  console.log(this.orderForm.value);
  var formvalue = this.orderForm.value;
  this.userDto.roleNames = [formvalue.userRole];
  var intMonth = 12;
  this.customerDto.agencyRec = this.pricingRuleDto[0].noRecords;
  this.customerDto.agencyRecPrice = this.pricingRuleDto[0].perCreditRate;
  this.customerDto.contactRec = this.pricingRuleDto[1].noRecords;
  this.customerDto.contactRecPrice =  this.pricingRuleDto[1].perCreditRate;
  this.customerDto.adActiveDate =  this.currentDateTime;
  this.customerDto.active = true;
  if(formvalue.overridemonth !="")
  {
    intMonth = formvalue.overridemonth;
  }
  this.customerDto.adExpiresDate = moment().add(intMonth, 'month');
  //this.aDOrderInput.
  this._dataService.setData("orderType", OrderType.AdSubscription);


  if(this.isLiscenceSingle)
  {
       this.aDOrderInput.orderValue = formvalue.amount + (formvalue.subamount!="" ? formvalue.subamount : 0);
       this.aDOrderInput.orderType = this.adOrderTypes.SingleLocationDatabase;
       this.customerDto.subType = this.adOrderTypes.SingleLocationDatabase;
       this.customerDto.byPassCard = 0;

       this.aDOrderInput.description = "Retail Access - 5 additional users - Retail only";
       if (this.subTypeID == SubscriptionType.SLQuarterly)
       {
           // write code for QuarterlyUpdates
           this.aDOrderInput.orderType = this.adOrderTypes.SingleLocationAndQuarterlyUpdates;
           this.customerDto.byPassCard = 0;
           this.customerDto.subType = this.adOrderTypes.SingleLocationAndQuarterlyUpdates;
           this.aDOrderInput.description = "Single Location + Quarterly Updates";
       }
       if (this.subTypeID == SubscriptionType.SLAnnual)
       {
          this.aDOrderInput.orderType = this.adOrderTypes.SingleLocationAndAnnualUpdates;
          this.customerDto.subType = this.adOrderTypes.SingleLocationAndAnnualUpdates;
          this.aDOrderInput.description = "Single Location + Annual Updates";
       }


  }
  else if(this.isLiscenceMultiple)
  {
       this.aDOrderInput.orderValue = formvalue.amount + (formvalue.subamount!="" ? formvalue.subamount : 0);
       this.aDOrderInput.orderType = AdOrderTypes.WholeDatabase;
       this.customerDto.subType = AdOrderTypes.WholeDatabase;
       this.customerDto.byPassCard = 0;
       this.aDOrderInput.description = "All Access - 5 additional users - Retail & MGA/Wholesale";
       if (this.subTypeID == SubscriptionType.MLQuarterly)
       {
           // write code for QuarterlyUpdates
           this.aDOrderInput.orderType = AdOrderTypes.WholeDatabaseQuarterlyUpdates;
           this.customerDto.subType = AdOrderTypes.WholeDatabaseQuarterlyUpdates;
           this.customerDto.byPassCard = 1;
           this.aDOrderInput.description = "Whole Database + Quarterly Updates";
       }
       if (this.subTypeID == SubscriptionType.MLAnnual)
       {
         this.aDOrderInput.orderType = AdOrderTypes.WholeDatabaseAnnualUpdates;
         this.customerDto.subType = AdOrderTypes.WholeDatabaseAnnualUpdates;
         this.customerDto.byPassCard = 0;
         this.aDOrderInput.description = "Whole Database + Annual Updates";
       }
  }
  else
  {
   this.aDOrderInput.orderValue = formvalue.amount;
   this.aDOrderInput.orderType = AdOrderTypes.UserSubscription;
   this.customerDto.subType = AdOrderTypes.UserSubscription;
   this.customerDto.byPassCard = 0;
   this.aDOrderInput.description = "All Access Unlimited - Unlimited Users - Retail & MGA/Wholesale";
  }
  if (this.MyOrderType == "Renewal")
  {
     this.aDOrderInput.orderType = AdOrderTypes.UserRenewal;
     this.customerDto.subType = AdOrderTypes.UserRenewal;
     this.customerDto.byPassCard = 0;
     this.aDOrderInput.description = "Agency Directory Renewal";
  }
  // Override Amount
  if (formvalue.amount.overrideamount)
  this.aDOrderInput.orderValue = formvalue.amount.overrideamount;

  this.aDOrderInput.isCtPurchased = false;
  this.aDOrderInput.ctCount = 0
  this.aDOrderInput.recordCount = 0;
  this.aDOrderInput.orderNotes = 'AD Subscription Order';
 }
 BdOrder(){
  var formvalue = this.orderForm.value;
  this.customerDto.bdActiveDate =  this.currentDateTime;
  this.customerDto.active = true;
  this.userDto.roleNames = [formvalue.userRole];
  this.customerDto.bdCredits = 0;
  this.customerDto.active = true;
  this._customerServiceProxy.update(this.customerDto).subscribe();
  this._userServiceProxy.update(this.userDto).subscribe(()=>
  {
    abp.message.info('Your Business Directory Subscription order has been successfully submitted');
    location.reload();
  });
 }
}
