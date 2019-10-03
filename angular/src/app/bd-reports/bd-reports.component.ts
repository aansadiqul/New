import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as XLSX from 'xlsx';
import {
  BdOrderServiceProxy
} from '@shared/service-proxies/service-proxies';
import { PurchaseTypes } from '@shared/AppEnums';

@Component({
  selector: 'app-bd-reports',
  templateUrl: './bd-reports.component.html',
  styleUrls: ['./bd-reports.component.css']
})
export class BdReportsComponent implements OnInit {
  reportData: any;
  id: number;
  private sub: any;
  constructor(private route: ActivatedRoute,
    private router: Router, private _bdOrders: BdOrderServiceProxy) { }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      this.id = +params['id']; // (+) converts string 'id' to a number
    });
  }

  reportView() {
    this.router.navigate(['/app/bd-report-view/' + this.id]);
  }

  exportToExcel() {
    this._bdOrders.getReportData(this.id, PurchaseTypes.All).subscribe(result => {
      this.reportData = JSON.parse(result);
      if (!this.reportData.length) {
        const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.reportData);
        const wb: XLSX.WorkBook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, 'BDOrder-' + this.id);
        /* save to file */
        XLSX.writeFile(wb, 'BDOrder-' + this.id + '.xlsx');
      }
      else {
        abp.notify.success('No Data Available to Export!');
      }

    });
  }

}
