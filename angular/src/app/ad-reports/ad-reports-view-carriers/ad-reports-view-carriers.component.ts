import { Component, OnInit, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { ActivatedRoute} from '@angular/router';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import {
  ADOrderServiceProxy
} from '@shared/service-proxies/service-proxies';
import { ReportTypes, OrderStatus } from '@shared/AppEnums';
export interface CarrierElement {
  Id: string;
  AccountId: string;
  Account: number;
  CompanyLine: string;
}
@Component({
  selector: 'app-ad-reports-view-carriers',
  templateUrl: './ad-reports-view-carriers.component.html',
  styleUrls: ['./ad-reports-view-carriers.component.css']
})
export class AdReportsViewCarriersComponent implements OnInit {
  carrierJson : any;
  ErrorMsg : string = "";
  saving: boolean = false;
  displayedColumns: string[] = ['AccountId','Account','CompanyLine'];
  dataSource: MatTableDataSource<CarrierElement>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  @Input() CarrierOrderId: any;
  @Output() adOrderJson = new EventEmitter();

  constructor(private route: ActivatedRoute,
    private _adOrders: ADOrderServiceProxy) { }

  ngOnInit() {
    this.saving = true;
    this._adOrders.getReportData(this.CarrierOrderId, ReportTypes.CarriersReport).subscribe(result => {
      this.carrierJson = JSON.parse(result.data);
      if(result.status == OrderStatus.Valid)
       {
          this.dataSource = new MatTableDataSource(this.carrierJson);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
          this.adOrderJson.emit(this.carrierJson);
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

}
