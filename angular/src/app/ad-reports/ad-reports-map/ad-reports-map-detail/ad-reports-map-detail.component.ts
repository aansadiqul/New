import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router} from '@angular/router';
import {
  ADOrderServiceProxy,
  AccountDto,
  TargetSectorDto,
  AgencyDto,
  AffiliationDto,
  CarrierDto
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-ad-reports-map-detail',
  templateUrl: './ad-reports-map-detail.component.html',
  styleUrls: ['./ad-reports-map-detail.component.css']
})
export class AdReportsMapDetailComponent implements OnInit {
  accountID: any;
  private sub: any;
  accountDetail:any;
  agencyDto: AgencyDto = new AgencyDto();
  targets: TargetSectorDto[] = [];
  affiliations: AffiliationDto[] = [];
  carriers: CarrierDto[] = [];


  constructor(private route: ActivatedRoute,
    private router: Router,private _adOrders: ADOrderServiceProxy) { }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      this.accountID = params['id'];
      //console.log( this.accountID);
      this._adOrders.getAccountDetails(this.accountID).subscribe(result => {
        this.accountDetail = result;
        console.log( this.accountDetail);
        this.agencyDto = result.agencyDetails;
        this.targets = result.targetSectors;
        this.affiliations = result.specialAffiliations;
        this.carriers = result.carriers;
      });
    });
  }

}
