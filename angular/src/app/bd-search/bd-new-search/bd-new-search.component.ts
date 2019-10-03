import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FormGroup, FormArray, FormControl, Validators, FormBuilder, FormGroupDirective, NgForm } from '@angular/forms';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import {
  BDSearchServiceProxy,
  BDSearchInput,
  BDSearchDto,
  BDCountsDto,
  BDCounts,
  BDQueries,
  AnalyzeBDInput,
  BreakdownDto,
  PricingRuleServiceProxy,
  PricingRuleDto
} from '@shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { CompanyBdComponent } from './company/company.component';
import { GeographicComponent } from '../../ad-search/ad-new-search/geographic/geographic.component';
import { IndustryComponent } from '../../ad-search/ad-new-search/industry/industry.component';
import {
  MatCheckboxChange
} from '@angular/material';
export interface AnalyzeElement {
  firstValue: string;
  secondValue: string;
  thirdValue: string;
  records: string;
  description: string;
}
import { OrderType, PurchaseTypes } from '@shared/AppEnums';
import {
  DataService
} from '@shared/service/data.service';


@Component({
  selector: 'app-bd-new-search',
  templateUrl: './bd-new-search.component.html',
  animations: [appModuleAnimation()],
  styleUrls: ['./bd-new-search.component.css']
})
export class BdNewSearchComponent extends AppComponentBase implements OnInit {
  IsXdate: boolean = false;
  IsRecord: boolean = true;
  IsCheque: boolean = false;
  showCountButton: boolean = true;
  recordCount: number = 0;
  xDatesPurchasedCnt: number = 0;
  recordOrderAmt: number = 0;
  xDatesOrderAmt: number = 0;
  orderValue: number = 0;
  overrideTotal: number = 0;
  recordPrice: number = 0;
  xDatePrice: number = 0;
  orderNotes: string = "";
  saving = false;
  bdSearchInput: BDSearchInput = new BDSearchInput();
  bdSearchDto: BDSearchDto = new BDSearchDto();
  bdCountsDto: BDCountsDto = new BDCountsDto();
  bdCounts: BDCounts = new BDCounts();
  records: 100;
  xdates: 200;
  bdQueries: BDQueries = new BDQueries();
  analyzeBDInput: AnalyzeBDInput = new AnalyzeBDInput();
  breakdownDto: BreakdownDto = new BreakdownDto();
  bdForm: FormGroup;
  heading: string = "New Search";
  selectedIndex = 0;
  searchSummary: any = [{ 'page': '', 'criteria': '', 'value': '' }];
  selectedBreakdown1 = '';
  selectedBreakdown2 = '';
  selectedBreakdown3 = '';
  isAdmin: boolean = false;
  selectedBreakdownTxt1 = '';
  selectedBreakdownTxt2 = '';
  selectedBreakdownTxt3 = '';

  isAnalyze = false;
  displayedColumns: string[] = ['firstValue', 'secondValue', 'thirdValue', 'records'];
  dataSource: MatTableDataSource<AnalyzeElement>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  hasSicCode = false;
  keys: string[] = ['firstValue', 'secondValue', 'thirdValue', 'records'];
  saveSearchID: any = null;
  searchName: any;
  currentdatetime = new Date().toLocaleString('en-US');
  // Lazy Lobding Logic
  IndustryTabShow = false;
  CarrierTabShow = false;

  breakdownByMonths: any = [];
  pricingRuleDto: PricingRuleDto[];
  // For Edit Only
  id: number;
  private sub: any;

  ///Call Child event by viewchild
  @ViewChild(CompanyBdComponent) private p_Company: CompanyBdComponent;
  @ViewChild(GeographicComponent) private p_Geographic: GeographicComponent;
  @ViewChild(IndustryComponent) private p_Industry: IndustryComponent;

  constructor(injector: Injector,
    public _bdService: BDSearchServiceProxy,
    private bdFformbuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private dataService: DataService,
    private _pricingRuleServiceProxy: PricingRuleServiceProxy
  ) {
    super(injector);
    //console.warn(bdFformbuilder);

  }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      this.id = +params['id']; // (+) converts string 'id' to a number
      this.id ? this.heading = "Load Search" : this.heading = "New Search";
      this.id ? this.CarrierTabShow = true : this.CarrierTabShow = false;
      this.id ? this.IndustryTabShow = true : this.IndustryTabShow = false;
      this.id ? this.showCountButton = false : this.showCountButton = true;

    });
    const tempQueryName = 'Query' + this.currentdatetime.replace(',', '').trim();
    this.searchName = tempQueryName.replace(/\s+|[,\/]/g, '/');
    this.bdForm = this.bdFformbuilder.group({
      dummy: null,
      QueryName: this.searchName,
    });

  }

  ngAfterViewInit() {
    if (this.id) {
      //this.saving = true;
      this.saveSearchID = this.id;
      this._bdService.getBdSearch(this.saveSearchID).subscribe(data => {
        this.bdSearchDto = data;
        forkJoin(
          this.p_Company.editCompany(this.bdSearchDto),
          this.p_Geographic.editBdGeographic(this.bdSearchDto),
          this.p_Industry.editIndustryBdSearch(this.bdSearchDto),
        ).subscribe((data) => {
          this.selectedIndex = 3;
          this.saving = false;
        }, (err) => {
          // error handling
        });
      });
    } else {
      this.saveSearchID = null;
      this.selectedIndex = 0;
    }
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  changeQueryName(event: any) { // without type info
    this.searchName = event.target.value;
  }

  getAnalyzedData() {
    this.saving = true;
    this.analyzeBDInput.firstValue = this.selectedBreakdown1;
    this.analyzeBDInput.secondValue = this.selectedBreakdown2;
    this.analyzeBDInput.thirdValue = this.selectedBreakdown3;
    this.analyzeBDInput.businessQuery = this.bdQueries.businessQuery;

    this.hasSicCode = this.selectedBreakdown1.indexOf('sic') !== -1 || this.selectedBreakdown2.indexOf('sic') !== -1 || this.selectedBreakdown3.indexOf('sic') !== -1 ?
      true : false;
    this.displayedColumns = this.selectedBreakdown1.indexOf('sic') !== -1 || this.selectedBreakdown2.indexOf('sic') !== -1 || this.selectedBreakdown3.indexOf('sic') !== -1 ?
      ['firstValue', 'secondValue', 'thirdValue', 'records', 'description'] : ['firstValue', 'secondValue', 'thirdValue', 'records'];

    this._bdService.postAnalyzeData(this.analyzeBDInput).subscribe(data => {
      const myanalizeData = data;
      this.keys = Object.keys(data);

      this.dataSource = new MatTableDataSource(myanalizeData);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.saving = false;
    });
  }

  formInitialized(name: string, form: FormGroup) {
    this.bdForm.setControl(name, form);

  }

  getVolumn(From: string, To: string): string {
    if (From !== '' && To !== '') {
      return From + '-' + To;
    }
    else if (From !== '' && To == '') {
      return From + '-' + '0';
    }
    else if (From == '' && To != '') {
      return '0' + '-' + To;
    }
    else {
      return '';
    }
  }

  saveSearch() {
    this.saving = true;
    this.bdSearchInput.sqlQuery = this.bdQueries.businessQuery + ' UNION ALL ' + this.bdQueries.bdxDateQuery;
    this.bdSearchInput.searchName = this.searchName;
    this.bdSearchInput.originalCount = this.bdCounts.businessListCount;
    if (this.saveSearchID != null) {
      this._bdService.getBdSearch(this.saveSearchID).subscribe(data => {
        this.bdSearchDto = data;
        Object.keys(this.bdSearchInput).forEach(key => this.bdSearchDto[key] = this.bdSearchInput[key]);
        console.log(this.bdSearchDto);
        this._bdService.update(this.bdSearchDto).subscribe(data => {
          this.saving = false;
          abp.message.info('Successfully Updated');
        });
      });
    } else {
      this._bdService.create(this.bdSearchInput).subscribe(data => {
        this.saveSearchID = data;
        this.currentdatetime = new Date().toLocaleString('en-US');
        const tempQueryName = 'Query' + this.currentdatetime.replace(',', '').trim();
        this.searchName = tempQueryName.replace(/\s+|[,\/]/g, '/');
        this.saving = false;
        abp.message.info('Successfully Saved');
      });
    }
  }

  save(): void {
    if (this.saving !== true) {
      this.saving = true;
    }

    this.id ? this.saveSearchID = this.id : this.saveSearchID = null;
    if (this.id) {
      this.searchName = this.bdSearchDto.searchName;
      this.bdForm.patchValue({
        QueryName: this.searchName,
      });
    }
    else {
      this.currentdatetime = new Date().toLocaleString('en-US');
      const tempQueryName = 'Query' + this.currentdatetime.replace(',', '').trim();
      this.searchName = tempQueryName.replace(/\s+|[,\/]/g, '/');
    }

    if (this.selectedIndex != 3) {
      this.selectedIndex = 3;
    }

    const formValue = this.bdForm.value;
    //console.log(formValue);
    this.bdSearchInput.searchName = formValue.QueryName;
    this.bdSearchInput.companyAndOr = formValue.company.CompanyAndOr;
    this.bdSearchInput.salesFrom = formValue.company.SalesFrom == '' ? null : formValue.company.SalesFrom;
    this.bdSearchInput.salesTo = formValue.company.SalesTo == '' ? null : formValue.company.SalesTo;
    this.bdSearchInput.employeesFrom = formValue.company.EmployeesFrom == '' ? null : formValue.company.EmployeesFrom;
    this.bdSearchInput.employeesTo = formValue.company.EmployeesTo == '' ? null : formValue.company.EmployeesTo;
    this.bdSearchInput.locations = formValue.company.Locations.length !== 0 ? formValue.company.Locations.toString() : '';
    this.bdSearchInput.manufacturing = formValue.company.Manufacturing.length !== 0 ? formValue.company.Manufacturing.toString() : '';
    this.bdSearchInput.companyName = formValue.company.CompanyName;
    this.bdSearchInput.companyBeginsContains = formValue.company.CompanyBeginsContains;
    this.bdSearchInput.county = formValue.geographic.CountyIDs.length !== 0 ? formValue.geographic.CountyIDs.toString() : '';
    this.bdSearchInput.exclude_AreaCode = formValue.geographic.ExcludeAreaCode.length !== 0 ?
      formValue.geographic.ExcludeAreaCode.toString() : '';
    this.bdSearchInput.exclude_County = formValue.geographic.ExcludeCountyIds.length !== 0 ?
      formValue.geographic.ExcludeCountyIds.toString() : '';
    this.bdSearchInput.exclude_MSA = formValue.geographic.ExcludeMSA.length !== 0 ? formValue.geographic.ExcludeMSA.toString() : '';
    this.bdSearchInput.exclude_State = formValue.geographic.ExcludeState.length !== 0 ? formValue.geographic.ExcludeState.toString() : '';
    this.bdSearchInput.exclude_Zip = formValue.geographic.ExcludeZip.length !== 0 ? formValue.geographic.ExcludeZip.toString() : '';
    this.bdSearchInput.msa = formValue.geographic.MSA.length !== 0 ? formValue.geographic.MSA.toString() : '';
    this.bdSearchInput.areaCode = formValue.geographic.AreaCode.length !== 0 ? formValue.geographic.AreaCode.toString() : '';
    this.bdSearchInput.exclude_AreaCode = formValue.geographic.ExcludeAreaCode.length !== 0 ?
      formValue.geographic.ExcludeAreaCode.toString() : '';
    this.bdSearchInput.state = formValue.geographic.State.length !== 0 ? formValue.geographic.State.toString() : '';
    this.bdSearchInput.zip = formValue.geographic.Zip.length !== 0 ? formValue.geographic.Zip.toString() : '';
    if (this.IndustryTabShow) {
      this.bdSearchInput.sic = formValue.industry.SICCodes.length !== 0 ? formValue.industry.SICCodes.toString() : '';
      this.bdSearchInput.siciDs = formValue.industry.SICIDS.length !== 0 ? formValue.industry.SICIDS.toString() : '';
    }
    else {
      this.bdSearchInput.sic = '';
      this.bdSearchInput.siciDs = '';
    }

    this._bdService.postBDSearchInput(this.bdSearchInput).subscribe(data => {
      this.bdCountsDto = data;
      this.bdCounts = this.bdCountsDto.bdCounts;
      this.bdQueries = this.bdCountsDto.bdQueries;
      this.breakdownByMonths = this.bdCounts.bdxDateBreakDown.map(item => {
        var monthname = '';
        if (item.workerscompmonth == '1') monthname = 'January';
        else if (item.workerscompmonth == '2') monthname = 'February';
        else if (item.workerscompmonth == '3') monthname = 'March';
        else if (item.workerscompmonth == '4') monthname = 'April';
        else if (item.workerscompmonth == '5') monthname = 'May';
        else if (item.workerscompmonth == '6') monthname = 'June';
        else if (item.workerscompmonth == '7') monthname = 'July';
        else if (item.workerscompmonth == '8') monthname = 'August';
        else if (item.workerscompmonth == '9') monthname = 'September';
        else if (item.workerscompmonth == '10') monthname = 'October';
        else if (item.workerscompmonth == '11') monthname = 'November';
        else monthname = 'December';
        return {
          workerscompmonth: item.workerscompmonth,
          xdatecount: item.xdatecount,
          month: monthname,
        };
      });
      this.getSearchSummary();
      this.saving = false;
    });
  }

  getSearchSummary() {
    this.searchSummary = [];
    const formValue = this.bdForm.value;

    if (this.bdSearchInput.companyName !== '') {
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Company Name', 'value': this.bdSearchInput.companyName + ' (' + this.bdSearchInput.companyBeginsContains + ')' });
    }

    if (this.bdSearchInput.employeesFrom && this.bdSearchInput.employeesTo) {
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Employee Size', 'value': this.bdSearchInput.employeesFrom + ' to ' + this.bdSearchInput.employeesTo + ' (' + this.bdSearchInput.companyAndOr + ')' });
    }
    else if (this.bdSearchInput.employeesTo) {
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Employee Size', 'value': this.bdSearchInput.employeesTo + ' or Less (' + this.bdSearchInput.companyAndOr + ')' });
    }
    else if (this.bdSearchInput.employeesFrom) {
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Employee Size', 'value': this.bdSearchInput.employeesFrom + ' or More (' + this.bdSearchInput.companyAndOr + ')' });
    }

    if (this.bdSearchInput.salesFrom && this.bdSearchInput.salesTo) {
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Annual Sales', 'value': this.bdSearchInput.salesFrom + ' to ' + this.bdSearchInput.salesTo });
    }
    else if (this.bdSearchInput.salesTo) {
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Annual Sales', 'value': this.bdSearchInput.salesTo + ' or Less' });
    }
    else if (this.bdSearchInput.salesFrom) {
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Annual Sales', 'value': this.bdSearchInput.salesFrom + ' or More' });
    }

    if (this.bdSearchInput.locations !== '')
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Locations', 'value': this.bdSearchInput.locations.replace(',', ', ') });
    if (this.bdSearchInput.manufacturing !== '')
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Manufacturing', 'value': this.bdSearchInput.manufacturing.replace(',', ', ') });
    if (this.bdSearchInput.state !== '')
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'States-Include', 'value': this.bdSearchInput.state.replace(',', ', ') });

    if (this.bdSearchInput.exclude_State !== '')
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'States-Exclude', 'value': this.bdSearchInput.exclude_State.replace(',', ', ') });

    if (this.bdSearchInput.county !== '')
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'County Names-Include', 'value': this.bdSearchInput.county.replace(',', ', ') + ': [' + formValue.geographic.County + ']' });

    if (this.bdSearchInput.exclude_County !== '')
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'County Names-Exclude', 'value': this.bdSearchInput.exclude_County.replace(',', ', ') + ': [' + formValue.geographic.ExcludeCountiesList + ']' });

    if (this.bdSearchInput.zip !== '')
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'Zip Codes-Include', 'value': this.bdSearchInput.zip.replace(',', ', ') });

    if (this.bdSearchInput.exclude_Zip !== '')
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'Zip Codes-Exclude', 'value': this.bdSearchInput.exclude_Zip.replace(',', ', ') });


    if (this.bdSearchInput.msa !== '')
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'Metro Areas-Include', 'value': formValue.geographic.MSAString.toString().replace(',', ', ') + ' [' + this.bdSearchInput.msa + ']' });

    if (this.bdSearchInput.exclude_MSA !== '')
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'Metro Areas-Exclude', 'value': formValue.geographic.MSAStringEx.toString().replace(',', ', ') + ' [' + this.bdSearchInput.exclude_MSA + ']' });

    if (this.bdSearchInput.siciDs !== '' && this.IndustryTabShow)
      this.searchSummary.push({ 'page': 'CLASS OF BUSINESS', 'criteria': 'SIC List', 'value': formValue.industry.sicNames.toString().replace(',', ', ') });
  }

  tabChange(index: number) {
    this.showCountButton = true;
    if (index === 2 && this.IndustryTabShow !== true) {
      this.IndustryTabShow = true;
    } else if (index === 3) {
      this.showCountButton = false;
      this.save();
    }
    else if (index === 4) {
      this.showCountButton = false;
      if(!this.saveSearchID){
        this.save();
      }
    }
  }

  onPurchaseRecords() {
    this.bdSearchInput.sqlQuery = this.bdQueries.businessQuery;
    this.dataService.setData('purchaseType', PurchaseTypes.Records);
    this.dataService.setData('businessQuery', this.bdQueries.businessQuery);
    this.IsXdate = false;
    this.onPurchase();
  }

  onPurchaseXDates() {
    this.bdSearchInput.sqlQuery = this.bdQueries.bdxDateQuery;
    this.dataService.setData('purchaseType', PurchaseTypes.XDates);
    this.dataService.setData('bdxDateQuery', this.bdQueries.bdxDateQuery);
    this.IsXdate = true;
    this.onPurchase();
  }

  onPurchaseAll() {
    this.dataService.setData('purchaseType', PurchaseTypes.All);
    this.dataService.setData('businessQuery', this.bdQueries.businessQuery);
    this.dataService.setData('bdxDateQuery', this.bdQueries.bdxDateQuery);
    this.bdSearchInput.sqlQuery = this.bdQueries.businessQuery + ' UNION ALL ' + this.bdQueries.bdxDateQuery;
    this.IsXdate = true;
    this.onPurchase();
  }

  onMap(type) {
    this.bdSearchInput.sqlQuery = this.bdQueries.businessQuery + ' UNION ALL ' + this.bdQueries.bdxDateQuery;
    this.onPurchase();
  }

  onPurchase() {

    this._pricingRuleServiceProxy.getAll(4, 0, "").subscribe(result => {
      this.pricingRuleDto = result.items;
      this.recordPrice = this.pricingRuleDto[2].perCreditRate;
      this.xDatePrice = this.pricingRuleDto[3].perCreditRate;
    });
    this.isAdmin = localStorage.getItem("isImpersonated") ? true : false;
    this.showCountButton = false;
    if (this.saveSearchID != null) {
      this._bdService.getBdSearch(this.saveSearchID).subscribe(data => {
        this.bdSearchDto = data;
        this.bdSearchDto.sqlQuery = this.bdSearchInput.sqlQuery;
        Object.keys(this.bdSearchInput).forEach(key => this.bdSearchDto[key] = this.bdSearchInput[key]);
        this._bdService.update(this.bdSearchDto).subscribe(() => {
          localStorage.setItem('BDSEARCHDTO', JSON.stringify(this.bdSearchDto));
          this.selectedIndex = 4;
          //this.router.navigate(['/app/bd-purchase/' + type]);
        });
      });
    } else {
      this._bdService.create(this.bdSearchInput).subscribe(data => {
        this.saveSearchID = data;
        localStorage.setItem('BDSEARCHDTO', JSON.stringify(this.bdSearchDto));
        this.selectedIndex = 4;
        //this.router.navigate(['/app/bd-purchase/' + type]);
      });
    }
  }
  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }
  onChangeBrk1(event) {
    let text = event.target.options[event.target.options.selectedIndex].text;
    this.selectedBreakdownTxt1 = text;
    this.selectedBreakdown1 = event.target.value;
    this.selectedBreakdown1 !== '' && this.selectedBreakdown2 !== '' && this.selectedBreakdown3 !== '' && this.selectedBreakdown1 !== this.selectedBreakdown2 && this.selectedBreakdown1 !== this.selectedBreakdown3 && this.selectedBreakdown2 !== this.selectedBreakdown3 ?
      this.isAnalyze = true : this.isAnalyze = false;
  }
  onChangeBrk2(event) {
    let text = event.target.options[event.target.options.selectedIndex].text;
    this.selectedBreakdownTxt2 = text;
    this.selectedBreakdown2 = event.target.value;
    this.selectedBreakdown1 !== '' && this.selectedBreakdown2 !== '' && this.selectedBreakdown3 !== '' && this.selectedBreakdown1 !== this.selectedBreakdown2 && this.selectedBreakdown1 !== this.selectedBreakdown3 && this.selectedBreakdown2 !== this.selectedBreakdown3 ?
      this.isAnalyze = true : this.isAnalyze = false;
  }
  onChangeBrk3(event) {
    let text = event.target.options[event.target.options.selectedIndex].text;
    this.selectedBreakdownTxt3 = text;
    this.selectedBreakdown3 = event.target.value;
    this.selectedBreakdown1 !== '' && this.selectedBreakdown2 !== '' && this.selectedBreakdown3 !== '' && this.selectedBreakdown1 !== this.selectedBreakdown2 && this.selectedBreakdown1 !== this.selectedBreakdown3 && this.selectedBreakdown2 !== this.selectedBreakdown3 ?
      this.isAnalyze = true : this.isAnalyze = false;
  }
  ///Logic for selec all & deselect All
  selected3 = [];
  toggle(item, event: MatCheckboxChange) {
    if (event.checked) {
      this.selected3.push(item);
    } else {
      const index = this.selected3.indexOf(item);
      if (index >= 0) {
        this.selected3.splice(index, 1);
      }
    }
    this.calculateTotalxDatesPurchasedCnt();
  }

  exists(item) {
    return this.selected3.indexOf(item) > -1;
  };

  isIndeterminate() {
    return (this.selected3.length > 0 && !this.isChecked());
  };

  isChecked() {
    return this.selected3.length === this.breakdownByMonths.length;
  };

  toggleAll(event: MatCheckboxChange) {

    if (event.checked) {
      this.breakdownByMonths.forEach(row => {
        // console.log('checked row', row);
        this.selected3.push(row)
      });

      // console.log('checked here');
    } else {
      // console.log('checked false');
      this.selected3.length = 0;
    }
    this.calculateTotalxDatesPurchasedCnt();
  }

  calculateTotalxDatesPurchasedCnt() {
    this.xDatesPurchasedCnt = this.selected3.length > 0 ? this.selected3.filter((item) => item.xdatecount)
      .map((item) => +item.xdatecount)
      .reduce((sum, current) => sum + current) : 0;
    this.calculateorderValue();
  }

  calculateorderValue() {
    this.recordOrderAmt = this.recordCount * this.recordPrice;
    this.xDatesOrderAmt = this.xDatesPurchasedCnt * this.xDatePrice;
    this.orderValue = this.recordOrderAmt + this.xDatesOrderAmt;
  }
  byCheque(event: MatCheckboxChange) {
    if (event.checked) {
      this.IsCheque = true;
    }
    else {
      this.IsCheque = false;
    }
  }


  withXdates(event: MatCheckboxChange) {
    if (event.checked) {
      this.IsXdate = true;
    }
    else {
      this.IsXdate = false;
    }
  }

  onRecordsPurchase(value) {
    if (+value > this.bdCounts.businessListCount) {
      this.recordCount = this.bdCounts.businessListCount;
    }
    else {
      this.recordCount = +value;
    }
    this.calculateorderValue();
  }


  onRecordPrice(value) {
    this.recordPrice = +value;
    this.calculateorderValue();
  }

  onxDatesPurchasedCnt(value) {
    if (+value > this.bdCounts.bdxDateListCount) {
      this.xDatesPurchasedCnt = this.bdCounts.bdxDateListCount;
    }
    else {
      this.xDatesPurchasedCnt = +value;
    }
    this.calculateorderValue();
  }

  onOverrideTotal(value) {
    this.overrideTotal = +value;
  }

  onOrderNotes(value) {
    this.orderNotes = value;
  }


  onContinue() {
    let months = [];
    for(let month of this.selected3) {
      months.push(month.workerscompmonth);
    }
    this.dataService.setData('orderType', OrderType.Bd);
    this.dataService.setData('queryId', this.saveSearchID);
    this.dataService.setData('queryName', this.searchName);
    this.dataService.setData('xDateMonths', months.toString());
    if (this.overrideTotal > 0) {
      this.dataService.setData('orderAmount', this.overrideTotal);
    }
    else {
      this.dataService.setData('orderAmount', this.orderValue);
    }
    this.dataService.setData('totalAdditionalPrice', this.xDatesOrderAmt);
    this.dataService.setData('contactCount', this.xDatesPurchasedCnt);
    this.dataService.setData('contactRecPrice', this.xDatePrice);
    this.dataService.setData('agencyRecPrice', this.recordPrice);
    this.dataService.setData('agencyCount', this.recordCount);
    this.dataService.setData('isCtPurchased', this.xDatesPurchasedCnt > 0 ? true : false);
    this.dataService.setData('orderNotes', this.orderNotes);
    this.router.navigate(['/app/checkout']);
  }

  onPricePerXdate(value) {
    this.xDatePrice = +value;
    this.calculateorderValue();
  }
}
