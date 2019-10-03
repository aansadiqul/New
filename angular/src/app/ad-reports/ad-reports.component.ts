import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  ADOrderServiceProxy
} from '@shared/service-proxies/service-proxies';
import { ReportTypes, OrderStatus } from '@shared/AppEnums';
import * as XLSX from 'xlsx';
@Component({
  selector: 'app-ad-reports',
  templateUrl: './ad-reports.component.html',
  styleUrls: ['./ad-reports.component.css']
})
export class AdReportsComponent implements OnInit {
  id: number;
  private sub: any;
  agencies: any;
  contacts: any;
  carriers: any;
  siccodes: any;
  affiliations: any;

  constructor(private route: ActivatedRoute,
    private router: Router, private _adOrders: ADOrderServiceProxy) { }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      this.id = +params['id']; // (+) converts string 'id' to a number
    });
  }
  reportView() {
    this.router.navigate(['/app/ad-reports-view/' + this.id]);
  }
  mapView() {
    this.router.navigate(['/app/ad-reports-map/' + this.id]);
  }

  async ExportTOExcel() {
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    await this._adOrders.getReportData(this.id, ReportTypes.AgencyReport).toPromise().then(result => {
      this.agencies = JSON.parse(result.data);
      if (this.agencies) {
        const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.agencies);
        XLSX.utils.book_append_sheet(wb, ws, 'Agencies');
      }
    });
    await this._adOrders.getReportData(this.id, ReportTypes.ContactsReport).toPromise().then(result => {
      this.contacts = JSON.parse(result.data);
      if (this.contacts) {
        const wsc: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.contacts);
        XLSX.utils.book_append_sheet(wb, wsc, 'Contacts');
      }
    });
    await this._adOrders.getReportData(this.id, ReportTypes.CarriersReport).toPromise().then(result => {
      this.carriers = JSON.parse(result.data);
      if (this.carriers) {
        const wsca: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.carriers);
        XLSX.utils.book_append_sheet(wb, wsca, 'Carriers');
      }
    });
    await this._adOrders.getReportData(this.id, ReportTypes.SicCodesReport).toPromise().then(result => {
      this.siccodes = JSON.parse(result.data);
      if (this.siccodes) {
        const wssic: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.siccodes);
        XLSX.utils.book_append_sheet(wb, wssic, 'SicCodes');
      }
    });
    await this._adOrders.getReportData(this.id, ReportTypes.AffliationsReport).toPromise().then(result => {
      this.affiliations = JSON.parse(result.data);
      if (this.affiliations) {
        const wsaf: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.affiliations);
        XLSX.utils.book_append_sheet(wb, wsaf, 'Affiliations');
      }
    });

    XLSX.writeFile(wb, 'ADOrder-' + this.id + '.xlsx');
  }
}
