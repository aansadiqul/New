import { Component, OnInit, ViewChild, Input,EventEmitter, Output  } from '@angular/core';
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
  SicCode: number;
  Description: string;
}
@Component({
  selector: 'app-ad-reports-view-siccodes',
  templateUrl: './ad-reports-view-siccodes.component.html',
  styleUrls: ['./ad-reports-view-siccodes.component.css']
})
export class AdReportsViewSiccodesComponent implements OnInit {  
  siccodesJson : any;
  ErrorMsg : string = null;
  saving : boolean = false;
  displayedColumns: string[] = ['AccountId','Account','SicCode'];
  dataSource: MatTableDataSource<CarrierElement>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @Input() SicCodesOrderId: any;
  @Output() adOrderJson = new EventEmitter();

  constructor(private route: ActivatedRoute,
    private _adOrders: ADOrderServiceProxy) { }

  ngOnInit() {
    this.saving = true;
    this._adOrders.getReportData(this.SicCodesOrderId, ReportTypes.SicCodesReport).subscribe(result => {
        this.siccodesJson = JSON.parse(result.data);
        if(result.status == OrderStatus.Valid)
        {
          this.dataSource = new MatTableDataSource(this.siccodesJson);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort; 
          this.adOrderJson.emit(this.siccodesJson);
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
