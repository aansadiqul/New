import { Component, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import {
  MatCheckboxChange
} from '@angular/material';
import { DataService } from '@shared/service/data.service';
import { OrderType, PaymentType, PurchaseTypes } from '@shared/AppEnums';
import {
  BdOrderServiceProxy,
  BdOrderInput,
  ADOrderServiceProxy,
  ADOrderInput,
  PurchaseInput,
  PaymentServiceProxy,
  PaymentRequestDto,
  BillingAddressDto,
  CreditCardDto,
  KeyedSaleResponse,
  PaymentInput,
  CustomerServiceProxy,
  CustomerDto,
  UserServiceProxy,
  UserDto
} from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements OnInit {
  saving = false;
  orderTypeName: string = "Business Directory";
  IsCheque: boolean = false;
  user: UserDto = new UserDto();;
  bdOrderInput: BdOrderInput = new BdOrderInput();
  customer: CustomerDto = new CustomerDto();
  adOrderInput: ADOrderInput = new ADOrderInput();
  purchaseInput: PurchaseInput = new PurchaseInput();
  orderType: number;
  purchaseType: number;
  paymentType: number;
  orderAmount: number = 0;
  orderNotes: string = "";
  name: string;
  cardNumber: any;
  cvv: number;
  address: any;
  zip: any;
  checkNumber: any;
  parameters: any;
  queryName: string;
  queryId: string;
  agencyQuery: string;
  sqlQuery: string;
  isCtPurchased: boolean;
  agencyCount: number = 0;
  contactCount: number = 0;
  agencyRecPrice: number = 0;
  contactRecPrice: number = 0;
  totalAgencyPrice: number = 0;
  totalAdditionalPrice: number = 0;
  totalOrder: number;
  yearsRange: any = [];
  isAdmin: boolean = false;

  ////For Checkout Form///
  paymentRequestDto: PaymentRequestDto = new PaymentRequestDto();
  creditCardDto: CreditCardDto = new CreditCardDto();
  billingAddressDto: BillingAddressDto = new BillingAddressDto();
  keyedSaleResponse: KeyedSaleResponse = new KeyedSaleResponse();
  paymentInput: PaymentInput = new PaymentInput();
  paymentId: number;

  constructor(private _dataService: DataService,
    private _bdOrderService: BdOrderServiceProxy,
    private _adOrderService: ADOrderServiceProxy,
    private _paymentService: PaymentServiceProxy,
    private _customerService: CustomerServiceProxy,
    private _userService: UserServiceProxy,
    private router: Router) {
    var year = new Date().getFullYear();
    for (var i = 0; i < 10; i++) {
      this.yearsRange.push({
        label: year + i,
        value: parseInt(String(year + i).slice(2, 4))
      });
    }

  }

  ngOnInit() {
this.isAdmin = localStorage.getItem("isImpersonated") ? true : false;
    this.creditCardDto.expiration_month = "";
    this.creditCardDto.expiration_year = "";
    this.parameters = this._dataService.getData();
    this.orderAmount = this.parameters.orderAmount ? +this.parameters.orderAmount.toFixed(2) : 0;
    this.orderNotes = this.parameters.orderNotes ? this.parameters.orderNotes : "";
    if (this.parameters.orderType == OrderType.Ad) {
      this.setAdOrderData();
    }
    else if (this.parameters.orderType == OrderType.Bd) {
      this.setBdOrderData();
    }
    else if (this.parameters.orderType == OrderType.AdSubscription) {
      this.setAdOrderSubscriptionData();
    }
  }

  setAdOrderData() {
    this.orderType = OrderType.Ad;
    this.orderTypeName = "Agency Directory";
    this.adOrderInput.description = this.parameters.queryName;
    this.adOrderInput.orderType = this.parameters.subType;
    this.adOrderInput.queryID = this.parameters.queryId;
    this.agencyQuery = this.parameters.agencyQuery;
    this.sqlQuery = this.parameters.sqlQuery;
    this.adOrderInput.isCtPurchased = this.parameters.isCtPurchased;
    this.adOrderInput.recordCount = this.parameters.agencyCount ? this.parameters.agencyCount : 0;
    if (this.adOrderInput.isCtPurchased) {
      this.adOrderInput.ctCount = this.parameters.contactCount ? this.parameters.contactCount : 0;
      this.adOrderInput.queryCriteria = this.parameters.sqlQuery;
    }
    else {
      this.adOrderInput.ctCount = 0;
      this.adOrderInput.queryCriteria = this.parameters.agencyQuery;
    }

    this.adOrderInput.orderValue = this.orderAmount;
    this.adOrderInput.orderNotes = this.orderNotes;
  }

  setAdOrderSubscriptionData() {
    this.orderType = OrderType.AdSubscription;
    this.orderTypeName = "Agency Directory Subscription";
    this.adOrderInput.description = this.parameters.description;
    this.adOrderInput.isCtPurchased = false;
    this.adOrderInput.ctCount = 0
    this.adOrderInput.recordCount = 0;
    this.adOrderInput.orderType = this.parameters.subType;
    this.adOrderInput.orderValue = this.orderAmount;
    this.adOrderInput.orderNotes = this.orderNotes;
    this.customer = JSON.parse(localStorage.getItem("customer"));
    this.user = JSON.parse(localStorage.getItem("user"));
    this.adOrderInput = JSON.parse(localStorage.getItem("adOrderInput"));
  }

  setBdOrderData() {
    this.orderType = OrderType.Bd;
    this.orderTypeName = "Business Directory";
    this.bdOrderInput.description = this.parameters.queryName;
    this.bdOrderInput.searchID = this.parameters.queryId;
    this.bdOrderInput.creditsUsed = 0;
    this.sqlQuery = this.parameters.sqlQuery;
    this.isCtPurchased = this.parameters.isCtPurchased;
    this.bdOrderInput.recordCount = this.parameters.agencyCount ? this.parameters.agencyCount : 0;
    this.bdOrderInput.recordPrice = this.parameters.agencyRecPrice ? this.parameters.agencyRecPrice : 0;
    this.bdOrderInput.xDateOrdValue = this.parameters.contactRecPrice ? this.parameters.contactRecPrice : 0;
    if (this.isCtPurchased) {
      this.bdOrderInput.xDatesPurchased = this.isCtPurchased;
      this.bdOrderInput.xDatesPurchasedCnt = this.parameters.contactCount ? this.parameters.contactCount : 0;
      this.bdOrderInput.xDatesOrderAmt = this.bdOrderInput.xDatesPurchasedCnt * this.bdOrderInput.xDateOrdValue;
    }
    else {
      this.bdOrderInput.xDatesPurchased = this.isCtPurchased;
      this.bdOrderInput.xDatesPurchasedCnt = 0;
      this.bdOrderInput.xDatesOrderAmt = 0;
    }

    this.bdOrderInput.orderValue = this.orderAmount;
    this.bdOrderInput.orderNotes = this.orderNotes;
    this.purchaseInput.purchaseType = this.parameters.purchaseType;
    this.purchaseInput.recordQuery = this.parameters.businessQuery;
    this.purchaseInput.recordCount = this.bdOrderInput.recordCount;
    this.purchaseInput.xDateQuery = this.parameters.bdxDateQuery;
    this.purchaseInput.xDateCount = this.bdOrderInput.xDatesPurchasedCnt;
    this.purchaseInput.xDateMonths = this.parameters.xDateMonths;
  }

  byCheque(event: MatCheckboxChange) {
    if (event.checked) {
      this.IsCheque = true;
    }
    else {
      this.IsCheque = false;
    }
  }

  length: number;
  timestamp: any;
  generate: any;

  _getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
  }

  TXNIDGenerator(): any {
    this.length = 8;
    this.timestamp = +new Date;
    var ts = this.timestamp.toString();
    var parts = ts.split("").reverse();
    var id = "";

    for (var i = 0; i < this.length; ++i) {
      var index = this._getRandomInt(0, parts.length - 1);
      id += parts[index];
    }
    return id.toString();

  }
  onSubmit() {
    this.saving = true;
    this.paymentType = PaymentType.Card;

    if (this.orderAmount == 0) {
      abp.message.info('Invalid Order Amount');
    }
    else {
      this.paymentRequestDto.amount = this.orderAmount;
      this.paymentRequestDto.billing_address = this.billingAddressDto;
      this.paymentRequestDto.credit_card = this.creditCardDto;
      this.paymentRequestDto.external_transaction_id = this.TXNIDGenerator();
      this._paymentService.checkOut(this.paymentRequestDto).subscribe(data => {
        this.keyedSaleResponse = data;
        if (this.keyedSaleResponse.response_code == 101 && this.keyedSaleResponse.external_transaction_id == this.paymentRequestDto.external_transaction_id) {
          this.paymentInput.externalTransactionId = this.keyedSaleResponse.external_transaction_id;
          this.paymentInput.transactionId = this.keyedSaleResponse.transaction_id.toString();
          this.savePayment(this.keyedSaleResponse.masked_card_number);
        }
        else {
          var errorStr = "";
          for (let key in this.keyedSaleResponse.errors) {
            errorStr = errorStr + this.keyedSaleResponse.errors[key][0] + '-';
          }
          abp.message.error(errorStr);
          this.saving = false;
        }

      });

    }
  }

  savePayment(cardorCheck: string = "") {
    this.paymentInput.orderAmount = this.orderAmount;
    this.paymentInput.orderTypeId = this.orderType;
    this.paymentInput.paymentTypeId = this.paymentType;
    this.paymentInput.creditCardMusked = cardorCheck;
    this._paymentService.create(this.paymentInput).subscribe(paymentID => {
      this.paymentId = paymentID;
      this.saveOrder();
    });

  }
  onCheckSubmit() {
    this.saving = true;
    this.paymentType = PaymentType.Check;
    this.savePayment(this.checkNumber);
  }

  saveOrder() {
    if (this.orderType == OrderType.Ad) {
      this.saveAdOrder();
    }
    else if (this.orderType == OrderType.Bd) {
      this.saveBdOrder();
    }
    else if (this.orderType == OrderType.AdSubscription) {
      this.saveAndUpdateAdSubscription();
    }
  }

  saveAdOrder(){
    this.adOrderInput.tranType = this.paymentType;
      this.adOrderInput.checkNo = this.checkNumber;
      this.adOrderInput.transactionID = this.paymentId.toString();
      this._adOrderService.create(this.adOrderInput).subscribe(data => {
        this.saving = false;
        abp.message.info('Your Agency Directory order has been successfully submitted');
        this.router.navigate(['/app/ad-orders']);
      });
  }

  saveAndUpdateAdSubscription(){
    this.adOrderInput.tranType = this.paymentType;
      this.adOrderInput.checkNo = this.checkNumber;
      this.adOrderInput.transactionID = this.paymentId.toString();
      this._customerService.update(this.customer).subscribe();
      this._userService.update(this.user).subscribe();
      this._adOrderService.create(this.adOrderInput).subscribe(data => {
        this.saving = false;
        localStorage.removeItem("customer");
        localStorage.removeItem("adOrderInput");
        localStorage.removeItem("user");
        abp.message.info('Your Agency Directory Subscription order has been successfully submitted');
        this.router.navigate(['/app/ad-new-search'])
        .then(() => {
          window.location.reload();
        });
      });
  }

  saveBdOrder(){
    this.bdOrderInput.checkNumber = this.checkNumber;
      this.bdOrderInput.paymentType = this.paymentType.toString();
      this.bdOrderInput.ccTransactionId = this.paymentId.toString();
      this._bdOrderService.create(this.bdOrderInput).subscribe(data => {
        this.purchaseInput.orderId = data;
        this._bdOrderService.createPurchase(this.purchaseInput).subscribe(() => {
          this.saving = false;
          abp.message.info('Your Business Directory order has been successfully submitted');
          this.router.navigate(['/app/bd-orders']);
        });
      });
  }
}
