import * as XLSX from 'xlsx';
import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  BdOrderServiceProxy
} from '@shared/service-proxies/service-proxies';
import { PurchaseTypes } from '@shared/AppEnums';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';

export interface ReportElement {
  DunsNumber: string;
  Company: number;
  TradeName: number;
  Address: number;
  City: string;
  State: string;
  Zip: string;
  StateCode: string;
  CountyCode: string;
  CityCode: string;
  SMSACode: string;
  Phone: string;
  CEOName: string;
  FirstName: string;
  MiddleInitial: string;
  LastName: string;
  Suffix: string;
  Prefix: string;
  Title: string;
  MRCCode: string;
  BranchIndicator: string;
  Manufacturing: string;
  LineOfBusiness: string;
  EmployeesTotal: string;
  EmployeesHere: string;
  SIC: string;
  Sales: string;
  AreaCode: string;
  StateCounty: string;
  SIC8: string;
  SIC8Description: string;
  WORKERSCOMPXDATE: string;
  WORKERSCOMPCARRIER: string;
}

@Component({
  selector: 'app-bd-report-view',
  templateUrl: './bd-report-view.component.html',
  styleUrls: ['./bd-report-view.component.css']
})
export class BdReportViewComponent implements OnInit {
  id: number;
  private sub: any;
  reportData: any;
  jsonData: any;
  ErrorMsg : string = null;
  saving: boolean = false;
  displayedColumns: string[] = ['DunsNumber', 'Company', 'TradeName', 'Address', 'City', 'State', 'Zip', 'StateCode', 'CountyCode', 'CityCode', 'SMSACode', 'Phone', 'CEOName', 'FirstName', 'MiddleInitial', 'LastName', 'Suffix', 'Prefix', 'Title', 'MRCCode', 'BranchIndicator', 'Manufacturing', 'LineOfBusiness', 'EmployeesTotal', 'EmployeesHere', 'SIC', 'Sales', 'AreaCode', 'StateCounty', 'CountyName', 'SIC8', 'SIC8Description', ' WORKERSCOMPXDATE', 'WORKERSCOMPCARRIER'];
  dataSource: MatTableDataSource<ReportElement>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild('TABLE') table: ElementRef;


  constructor(private route: ActivatedRoute,private _bdOrders: BdOrderServiceProxy) { }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      this.saving = true;
      this.id = +params['id']; // (+) converts string 'id' to a number
      this.viewReport(this.id);
    });
  }
  viewReport(orderId: any): void {
    this._bdOrders.getReportData(orderId, PurchaseTypes.All).subscribe(result => {
      this.jsonData = result;
      this.reportData = JSON.parse(result);
      this.dataSource = new MatTableDataSource(this.reportData);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.saving = false;
      if(!this.reportData.length)
      {
         this.ErrorMsg = "No data is avaialble";
      }
    });
  }

  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  ExportTOExcel() {
    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.reportData);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'BDOrder-' + this.id);

    /* save to file */
    XLSX.writeFile(wb, 'BDOrder-' + this.id + '.xlsx');

  }

  getValue(row: any, element: any) {
    return row[element];
  }
}
