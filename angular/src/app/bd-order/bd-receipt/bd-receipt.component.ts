import { Component, OnInit } from '@angular/core';
import {
  CommonService
} from '@shared/service/common.service';
import {
  BdOrderServiceProxy,
  BdReceiptDto
} from '@shared/service-proxies/service-proxies';
import { ActivatedRoute } from '@angular/router';
import * as jspdf from 'jspdf';
import html2canvas from 'html2canvas';

@Component({
  selector: 'app-bd-receipt',
  templateUrl: './bd-receipt.component.html',
  styleUrls: ['./bd-receipt.component.css']
})
export class BdReceiptComponent implements OnInit {
  id: any;
  paymentId: number = 0;
  loading = true;
  bdReceipt: BdReceiptDto = new BdReceiptDto();
  constructor(private _bdOrders: BdOrderServiceProxy,
    private commonService: CommonService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.paymentId = this.commonService.getId;
    this.route.params.subscribe(params => {
      this.id = +params['id']; // (+) converts string 'id' to a number
    });
    this._bdOrders
      .getReceipt(this.id, this.paymentId)
      .subscribe((result: BdReceiptDto) => {
        this.bdReceipt.init(result);
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
      pdf.save('BDReceipt_'+this.id+'.pdf'); // Generated PDF
    });
 }
}
