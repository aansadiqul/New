import { Component, OnInit,ViewChild } from '@angular/core';
import { ActivatedRoute, Router} from '@angular/router';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import {
  ADOrderServiceProxy
} from '@shared/service-proxies/service-proxies';
import { ReportTypes, OrderStatus } from '@shared/AppEnums';
import * as XLSX from 'xlsx';

export interface AgencyElement {
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
  AgencyManagement: string;
  LastName: string;
  FirstName: string;
  Suffix: string;
  Title: string;
  Mobile: string;
  CEmail: string;
  AccountId: string;
  SpLines: string; 
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
  selector: 'app-ad-reports-view',
  templateUrl: './ad-reports-view.component.html',
  styleUrls: ['./ad-reports-view.component.css']
})
export class AdReportsViewComponent implements OnInit {
  id: number;
  private sub: any;
  agencyJson : any;  
  ErrorMsg : string = null;
  displayedColumns: string[] = ['Account','Address1','Address2','City','State','PostalCode','County','TimeZone','Country','Division','MainPhone','PhoneExtension','Fax','TollFree','WebAddress','Email','Type','Revenue','PercentComm','Employees','SpecialAffiliation','AgencyManagement','LastName','FirstName','Suffix','Title','Mobile','CEmail','AccountId','SpLines','TitleSearch','LineSearch','Msa','PremiumVolume','DunsNum','CEmail2','LinkedUrl','BranchIndicator','Num_Locations','TwitterUrl','FacebookUrl','GoogleplusUrl','YoutubeUrl','Linkedin','CountyCode'];
  dataSource: MatTableDataSource<AgencyElement>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  /// Variables from partial views
  saving: boolean = false;
  contacts : any;
  carriers : any;
  siccodes : any;
  affiliations : any;
  
  constructor(private route: ActivatedRoute,
    private router: Router,private _adOrders: ADOrderServiceProxy) { }

  ngOnInit() {
    
    this.sub = this.route.params.subscribe(params => {
      this.saving = true;
      this.id = +params['id']; // (+) converts string 'id' to a number     
      this.viewReport(this.id);
    });
  }
  viewReport(orderId: any): void{
    //console.log(orderId);
    ///Load Agency Json
    this._adOrders.getReportData(orderId, ReportTypes.AgencyReport).subscribe(result => {
       this.agencyJson = JSON.parse(result.data); 
       if(result.status == OrderStatus.Valid)
       {
        this.dataSource = new MatTableDataSource(this.agencyJson);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;     
       }     
       else if(result.status == OrderStatus.Expired)
       {
         this.ErrorMsg = "Your report has expired.";
       }
       else if(result.status == OrderStatus.PurchaseRequired)
       {
        this.ErrorMsg = "A report is not available for this order.";
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
  getContacts(contactJson: any): void{ this.contacts = contactJson; }
  getsiccodes(siccodesJson: any): void{ this.siccodes = siccodesJson; }
  getcarriers(carriersJson: any): void{ this.carriers = carriersJson; }
  getaffiliations(affiliationsJson: any): void{ this.affiliations = affiliationsJson; }

  ExportTOExcel() {
    ///Agency List
    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet( this.agencyJson);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Agencies');
    ///Contact List
    const wsc: XLSX.WorkSheet = XLSX.utils.json_to_sheet( this.contacts);
    XLSX.utils.book_append_sheet(wb, wsc, 'Contacts');    
    ///Carrier List
    const wsca: XLSX.WorkSheet = XLSX.utils.json_to_sheet( this.carriers);
    XLSX.utils.book_append_sheet(wb, wsca, 'Carriers');
    ///Siccodes List
    const wssic: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.siccodes);
    XLSX.utils.book_append_sheet(wb, wssic, 'SicCodes'); 
    ///Affiliations List
    const wsaf: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.affiliations);    
    XLSX.utils.book_append_sheet(wb, wsaf, 'Affiliations');
    ////Now write the files and download
    XLSX.writeFile(wb, 'ADOrder-' + this.id + '.xlsx');
  }

  mapView() {
    this.router.navigate(['/app/ad-reports-map/' + this.id]);
  }
  
}
