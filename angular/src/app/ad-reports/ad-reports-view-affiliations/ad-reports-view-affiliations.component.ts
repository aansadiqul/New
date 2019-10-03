import { Component, OnInit , ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { ActivatedRoute} from '@angular/router';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import {
  ADOrderServiceProxy
} from '@shared/service-proxies/service-proxies';
import { ReportTypes, OrderStatus } from '@shared/AppEnums';
export interface CarrierElement {
  Id: string;
  AccountId: string;
  Account: string;
  SpecialAffiliation: number;
}
@Component({
  selector: 'app-ad-reports-view-affiliations',
  templateUrl: './ad-reports-view-affiliations.component.html',
  styleUrls: ['./ad-reports-view-affiliations.component.css']
})
export class AdReportsViewAffiliationsComponent implements OnInit {
  affiliationsJson : any;
  saving: boolean = false;
  displayedColumns: string[] = ['AccountId','Account','SpecialAffiliation'];
  dataSource: MatTableDataSource<CarrierElement>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @Input() AffiliationOrderId: any;
  @Output() adOrderJson = new EventEmitter();

  ErrorMsg : string = null;

  constructor(private route: ActivatedRoute,
    private _adOrders: ADOrderServiceProxy) { }

  ngOnInit() {
    this.saving = true;
    this._adOrders.getReportData(this.AffiliationOrderId, ReportTypes.AffliationsReport).subscribe(result => {
      this.affiliationsJson = JSON.parse(result.data);
       if(result.status == OrderStatus.Valid)
       {
          this.dataSource = new MatTableDataSource(this.affiliationsJson);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
          this.adOrderJson.emit(this.affiliationsJson);
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
  getVal(row: any, element: any){
    return row[element];
  }

  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

}
