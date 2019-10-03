import { Component, OnInit } from '@angular/core';
import {
  MatCheckboxChange
} from '@angular/material';
import { RecordPriceDto } from '@shared/service-proxies/service-proxies';
import { DataService } from '@shared/service/data.service';
import { OrderType, AdOrderTypes } from '@shared/AppEnums';
import { Router } from '@angular/router';

@Component({
  selector: 'app-ad-buy-names',
  templateUrl: './ad-buy-names.component.html',
  styleUrls: ['./ad-buy-names.component.css']
})
export class AdBuyNamesComponent implements OnInit {
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
  overrideTotal: number = 0;
  orderNotes: string = "";

  constructor(private dataService: DataService, private router: Router) {

  }

  ngOnInit() {
    this.parameters = this.dataService.getData();
    this.queryName = this.parameters.queryName;
    this.queryId = this.parameters.queryId;
    this.agencyQuery = this.parameters.agencyQuery;
    this.sqlQuery = this.parameters.sqlQuery;
    this.isCtPurchased = this.parameters.isCtPurchased;
    this.agencyCount = this.parameters.agencyCount ? this.parameters.agencyCount : 0;
    this.agencyRecPrice = this.parameters.agencyRecPrice ? this.parameters.agencyRecPrice : 0;
    this.contactRecPrice = this.parameters.contactRecPrice ? this.parameters.contactRecPrice : 0;
    if(this.isCtPurchased)
    {
      this.contactCount = this.parameters.contactCount ? this.parameters.contactCount : 0;
      this.totalAdditionalPrice = this.contactCount * this.contactRecPrice;
    }
    else{
      this.contactCount = 0;
      this.totalAdditionalPrice = 0;
    }

    this.totalAgencyPrice = this.agencyCount * this.agencyRecPrice;
    this.totalOrder = this.totalAgencyPrice + this.totalAdditionalPrice;
  }

  IsCheque : boolean = false;
  byCheque(event: MatCheckboxChange) {
    if ( event.checked ) {
      this.IsCheque = true;
    }
    else {
      this.IsCheque = false;
    }
  }

  calculatePrice(event: MatCheckboxChange) {
    if ( event.checked ) {
      this.contactCount = this.parameters.contactCount ? this.parameters.contactCount : 0;
      this.totalAdditionalPrice = this.contactCount * this.contactRecPrice;
      this.totalOrder = this.totalAgencyPrice + this.totalAdditionalPrice;
    }
    else {
      this.contactCount = 0;
      this.totalAdditionalPrice = 0;
      this.totalOrder = this.totalAgencyPrice + this.totalAdditionalPrice;
    }
  }

  onOverrideTotal(value) {
    this.overrideTotal = +value;
  }

  onOrderNotes(value) {
    this.orderNotes = value;
  }

  onContinue() {
    this.dataService.setData('subType', AdOrderTypes.RecordsPurchase);
    this.dataService.setData('orderType', OrderType.Ad);
    if (this.overrideTotal > 0) {
      this.dataService.setData('orderAmount', this.overrideTotal);
    }
    else {
      this.dataService.setData('orderAmount', this.totalOrder);
    }
    this.dataService.setData('orderNotes', this.orderNotes);
    this.router.navigate(['/app/checkout']);
  }

}
