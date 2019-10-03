import { Component, OnInit , ViewChild} from '@angular/core';
import { Location } from '@angular/common';
import {
  DataService
} from '@shared/service/data.service';
import {
  ADSearchServiceProxy,
  ADName,
  RecordPriceDto
 } from '@shared/service-proxies/service-proxies';
 import { ActivatedRoute, Router} from '@angular/router';
 import {MatPaginator, MatSort, MatTableDataSource} from '@angular/material';

 export interface AdNameElement {
  accountName: string;
  state: string;
 }

@Component({
  selector: 'app-ad-review-names',
  templateUrl: './ad-review-names.component.html',
  styleUrls: ['./ad-review-names.component.css']
})
export class AdReviewNamesComponent implements OnInit {
  parameters: any;
  recordPrice: RecordPriceDto;
  dataSource: MatTableDataSource<ADName>;
  displayedColumns: string[] = ['accountName', 'state'];
  queryName: string;
  queryId: string;
  agencyQuery: string;
  sqlQuery: string;
  isCtPurchased: string;
  agencyCount: number;
  contactCount: number;
  loading: boolean = true;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private location: Location,private _dataService: DataService,
              public _adService: ADSearchServiceProxy, private router: Router) {

   }

  ngOnInit() {
    this.parameters = this._dataService.getData();
    this.queryName = this.parameters.queryName;
    this.queryId = this.parameters.queryId;
    this.agencyQuery = this.parameters.agencyQuery;
    this.sqlQuery = this.parameters.sqlQuery;
    this.isCtPurchased = this.parameters.isCtPurchased;
    this.agencyCount = this.parameters.agencyCount;
    if(this.isCtPurchased)
    {
      this.contactCount = this.parameters.contactCount;
    }
    else{
      this.contactCount = 0;
    }
    this._adService.getAdNames(this.parameters.queryId).subscribe(data => {
      const adNames = data.adNames;
      this.recordPrice = data.recordPrice;
      this.dataSource = new MatTableDataSource(adNames);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.loading = false;
    });
  }

  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }
  goBack() {
    this.router.navigate(['/app/ad-new-search/'+this.queryId]);
    //this.location.back(); // <-- go back to previous location on cancel
  }

  submit() {
    this._dataService.setData('agencyRecPrice', this.recordPrice.agencyRecPrice);
    this._dataService.setData('contactRecPrice', this.recordPrice.contactRecPrice);
    this.router.navigate(['/app/ad-buy-names']);
  }
}
