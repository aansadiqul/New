import { Component, OnInit, Input,ViewChild,EventEmitter, Output } from '@angular/core';
import { ActivatedRoute, Router} from '@angular/router';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';

import {
  ADOrderServiceProxy
} from '@shared/service-proxies/service-proxies';
import { ReportTypes, OrderStatus } from '@shared/AppEnums';
export interface ContactsElement {
  Id : number;
  Account: string;
  Address1: number;
  Address2: number;
  City: string;
  State: string;
  PostalCode: string;
  County: string;
  TimeZone: string;
  Country: string;
  Division: string;
  MainPhone: string;
  PhoneExtension: string;
  Fax: string;
  TollFree: string;
  WebAddress: string;
  Email: string;
  Type: string;
  Revenue: string;
  PercentComm: string;
  Employees: string;
  SpecialAffiliation: string;
  Producer: string;
  AgencyManagement: string;
  LastName: string;
  FirstName: string;
  Suffix: string;
  Title: string;
  Mobile: string;
  CEmail: string;
  AccountId: string;
  ContactId: string;
  IsPrimary: string;
  TitleSearch: string;
  LineSearch: string;
  MSA: string;
  PremiumVolume: string;
  DunsNum: string;
  CEmail2: string;
  LinkedUrl: string;
  BranchIndicator: string;
  Num_Locations: string;
  TwitterUrl: string;
  FacebookUrl: string;
  GoogleplusUrl: string;
  YoutubeUrl: string;
  Linkedin: string;
  CountyCode: string;
  
}
@Component({
  selector: 'app-ad-reports-view-contacts',
  templateUrl: './ad-reports-view-contacts.component.html',
  styleUrls: ['./ad-reports-view-contacts.component.css']
})
export class AdReportsViewContactsComponent implements OnInit {
  contactJson : any;
  ErrorMsg : string = null;
  saving: boolean = false;
  @Input() ContactOrderId: any;
  @Output() adOrderJson = new EventEmitter();

  displayedColumns: string[] = ['Account', 'Address1', 'Address2', 'City', 'State', 'PostalCode', 'County', 'TimeZone', 'Country', 'MainPhone', 'PhoneExtension', 'Fax', 'TollFree', 'WebAddress', 'Email', 'Type', 'Revenue', 'PercentComm', 'Employees', 'SpecialAffiliation', 'AgencyManagement', 'LastName', 'FirstName', 'Suffix', 'Title', 'Mobile', 'CEmail', 'AccountId', 'ContactId', 'IsPrimary', 'TitleSearch', 'MSA', 'PremiumVolume', 'DunsNum', 'CEmail2', 'LinkedUrl', 'BranchIndicator', 'Num_Locations', 'TwitterUrl', 'FacebookUrl', 'GoogleplusUrl', 'YoutubeUrl', 'Linkedin', 'CountyCode'];

  dataSource: MatTableDataSource<ContactsElement>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
 
  constructor(private route: ActivatedRoute,
   private _adOrders: ADOrderServiceProxy) { }

  ngOnInit() {
    this.viewReport(this.ContactOrderId);
  }

  viewReport(orderId: any): void{
    this.saving = true;
    this._adOrders.getReportData(orderId, ReportTypes.ContactsReport).subscribe(result => {
      this.contactJson = JSON.parse(result.data); 
        if(result.status == OrderStatus.Valid)
        {     
          this.dataSource = new MatTableDataSource(this.contactJson);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
          this.adOrderJson.emit(this.contactJson);
        }     
        else if(result.status == OrderStatus.Expired)
        {
          this.ErrorMsg = "Your report has expired.";
        }
        else if(result.status == OrderStatus.PurchaseRequired)
        {
         this.ErrorMsg = "A report is not available for this order.";
        }
        else
        {
          this.ErrorMsg = "Invalid Order Id";
        }
        this.saving = false;
     });      
  }
  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }
  
}
