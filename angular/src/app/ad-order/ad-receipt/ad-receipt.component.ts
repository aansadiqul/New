import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as jspdf from 'jspdf';
import html2canvas from 'html2canvas';
import {
  CommonService
} from '@shared/service/common.service';
import {
  ADOrderServiceProxy,
  AdReceiptDto
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-ad-receipt',
  templateUrl: './ad-receipt.component.html',
  styleUrls: ['./ad-receipt.component.css']
})
export class AdReceiptComponent implements OnInit {
  id: any;
  paymentId: number = 0;
  loading: boolean = true;
  adReceipt: AdReceiptDto = new AdReceiptDto();
  constructor(private _adOrders: ADOrderServiceProxy,
    private commonService: CommonService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.paymentId = this.commonService.getId;
    this.route.params.subscribe(params => {
      this.id = +params['id']; // (+) converts string 'id' to a number
    });
    this._adOrders
      .getReceipt(this.id, this.paymentId)
      .subscribe((result: AdReceiptDto) => {
        this.adReceipt.init(result);
        this.loading = false;
      });
  }

  captureScreen()
  {
      var data = document.getElementById('contentToConvert');
      html2canvas(data).then(canvas => {
      // Few necessary setting options
      var imgWidth = 266;//208;
      var pageHeight = 295;
      var imgHeight = canvas.height * imgWidth / canvas.width;
      var heightLeft = imgHeight;

      const contentDataURL = canvas.toDataURL('image/png')
      let pdf = new jspdf('p', 'mm', 'a4'); // A4 size page of PDF
      var position = 0;
      //pdf.addImage(contentDataURL, 'PNG', 0, position, imgWidth, imgHeight)
      pdf.addImage(contentDataURL, 'PNG', 0, position, imgWidth, imgHeight)
      pdf.save('ADReceipt_'+this.id+'.pdf'); // Generated PDF
    });
 }
}
