import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource, MAT_AUTOCOMPLETE_SCROLL_STRATEGY_FACTORY } from '@angular/material';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FormGroup, FormArray, FormControl, Validators, FormBuilder, FormGroupDirective, NgForm } from '@angular/forms';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import {
  ADSearchServiceProxy,
  ADSearchInput,
  ADSearchDto,
  ADCountsDto,
  ADCounts,
  ADQueries,
  AnalyzeInput,
  BreakdownDto,

} from '@shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
//import { resolve } from 'dns';
//import { reject } from 'q';
import { forkJoin } from 'rxjs';
///Call Child Events for Edit
import { CompanyComponent } from './company/company.component';
import { GeographicComponent } from './geographic/geographic.component';
import { CarriersComponent } from './carriers/carriers.component';
import { AffiliationsComponent } from './affiliations/affiliations.component';
import { IndustryComponent } from './industry/industry.component';
import { ContactsComponent } from './contacts/contacts.component';

export interface AnalyzeElement {
  firstValue: string;
  secondValue: string;
  records: string;
  description: string;
}
import {
  DataService
} from '@shared/service/data.service';

@Component({
  selector: 'app-ad-new-search',
  templateUrl: './ad-new-search.component.html',
  animations: [appModuleAnimation()],
  styleUrls: ['./ad-new-search.component.css']
})
export class AdNewSearchComponent extends AppComponentBase implements OnInit {

  saving = false;
  hh = false;
  showCountButton = true;
  heading: string = "New Search";
  adSearchInput: ADSearchInput = new ADSearchInput();
  adSearchDto: ADSearchDto = new ADSearchDto();
  adCountsDto: ADCountsDto = new ADCountsDto();
  adCounts: ADCounts = new ADCounts();
  adQueries: ADQueries = new ADQueries();
  analyzeInput: AnalyzeInput = new AnalyzeInput();
  breakdownDto: BreakdownDto = new BreakdownDto();

  adForm: FormGroup;
  selectedIndex: number = 0;
  searchSummary: any = [{ 'page': '', 'criteria': '', 'value': '' }];
  activeFormName: any;

  ///Analyze variable///

  selectedBreakdown1: string = "";
  selectedBreakdown2: string = "";
  isAnalyze: boolean = false;
  displayedColumns: string[] = ['firstValue', 'secondValue', 'records'];//['firstValue', 'secondValue', 'records', 'description'];
  dataSource: MatTableDataSource<AnalyzeElement>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  hasSicCode: boolean = false;
  saveSearchID: any = null;

  cDate: any;
  currentdatetime = new Date().toLocaleString("en-US");

  //Lazy Loading Logic
  IndustryTabShow = false;
  CarrierTabShow = false;

  //For Edit Only
  id: number;
  private sub: any;
  ///Call Child event by viewchild
  @ViewChild(CompanyComponent) private p_Company: CompanyComponent;
  @ViewChild(GeographicComponent) private p_Geographic: GeographicComponent;
  @ViewChild(CarriersComponent) private p_Carrier: CarriersComponent;
  @ViewChild(AffiliationsComponent) private p_Affiliation: AffiliationsComponent;
  @ViewChild(IndustryComponent) private p_Industry: IndustryComponent;
  @ViewChild(ContactsComponent) private p_Contacts: ContactsComponent;
  @ViewChild('fieldName1') fieldName1;
  constructor(injector: Injector,
    public _adService: ADSearchServiceProxy,
    private route: ActivatedRoute,
    private router: Router,
    private adFformbuilder: FormBuilder,
    private dataService: DataService
  ) {
    super(injector);
  }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      this.id = +params['id']; // (+) converts string 'id' to a number
      if (this.id) {
        this.heading = "Load Search";
        this.CarrierTabShow = true;
        this.IndustryTabShow = true;
        this.saving = true;
      }
      else {
        this.heading = "New Search";
        this.CarrierTabShow = false;
        this.IndustryTabShow = false;
        this.saving = false;
      }
    });

    var tempQueryName = "Query" + this.currentdatetime.replace(',', '').trim();
    this.cDate = tempQueryName.replace(/\s+|[,\/]/g, "/");
    this.adForm = this.adFformbuilder.group({
      QueryName: this.cDate,
    });
  }
  ngAfterViewInit() {

    if (this.id) ///Edit Logic
    {
      this.saveSearchID = this.id;
      this._adService.getAdSearch(this.saveSearchID).subscribe(data => {
        this.adSearchDto = data;


        forkJoin(
          this.p_Company.editCompany(this.adSearchDto),
          this.p_Geographic.editGeographic(this.adSearchDto),
          this.p_Carrier.editCarrierAdSearch(this.adSearchDto),
          this.p_Affiliation.editAffiliationAdSearch(this.adSearchDto),
          this.p_Industry.editIndustryAdSearch(this.adSearchDto),
          this.p_Contacts.editContactsAdSearch(this.adSearchDto)
        ).subscribe((data) => {
          this.selectedIndex = 6;
          this.saving = false;
        }, (err) => {
          // error handling
        });
      });
    }
    else {
      this.saveSearchID = null;
      this.selectedIndex = 0;
    }
  }
  ngOnDestroy() {
    this.sub.unsubscribe();
  }
  changeQueryName(event: any) { // without type info
    this.cDate = event.target.value;
  }
  getAnalyzedData() {
    this.saving = true;
    this.analyzeInput.firstValue = this.selectedBreakdown1;
    this.analyzeInput.secondValue = this.selectedBreakdown2;
    this.analyzeInput.sicCodes = this.adSearchInput.sicCodes;
    this.analyzeInput.agencyQuery = this.adQueries.agencyQuery;

    this.selectedBreakdown1.indexOf('SIC') !== -1 || this.selectedBreakdown2.indexOf('SIC') !== -1 ? this.hasSicCode = true : this.hasSicCode = false;
    this.selectedBreakdown1.indexOf('SIC') !== -1 || this.selectedBreakdown2.indexOf('SIC') !== -1 ? this.displayedColumns = ['firstValue', 'secondValue', 'description', 'records'] : this.displayedColumns = ['firstValue', 'secondValue', 'records'];

    this._adService.postAnalyzeData(this.analyzeInput).subscribe(data => {
      const myanalizeData = data;
      this.dataSource = new MatTableDataSource(myanalizeData);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.saving = false;
    });
  }
  saveSearch() {

    this.saving = true;
    this.adSearchInput.queryCriteria = this.adQueries.sqlQuery;
    //alert(this.cDate);
    this.adSearchInput.queryName = this.cDate;
    if (this.saveSearchID != null) {
      this._adService.getAdSearch(this.saveSearchID).subscribe(data => {
        this.adSearchDto = data;
        Object.keys(this.adSearchInput).forEach(key => this.adSearchDto[key] = this.adSearchInput[key]);
        this._adService.update(this.adSearchDto).subscribe(data => {
          this.saving = false;
          abp.message.info('Successfully Updated');
        });
      });
    }
    else {

      this.saving = false;
      abp.message.confirm(
        this.l('ADSearchDeleteWarningMessage'),
        (result: boolean) => {
          if (result) {
            this._adService.create(this.adSearchInput).subscribe(data => {
              this.saveSearchID = data;
              this.currentdatetime = new Date().toLocaleString("en-US");
              var tempQueryName = "Query" + this.currentdatetime.replace(',', '').trim();
              this.cDate = tempQueryName.replace(/\s+|[,\/]/g, "/");
              this.saving = true;
              //this.savingtrue;
              abp.message.info('Successfully Saved');
            });


          }
          else {
            //  this.saving = true;
            this.fieldName1.nativeElement.focus();
          }
        }
      );
      console.log("after");


    }

  }
  save(): void {
    //initialize the variables
    this.showCountButton = false;
    if (this.saving != true)
      this.saving = true;

    this.id ? this.saveSearchID = this.id : this.saveSearchID = null;
    if (this.id) {
      this.cDate = this.adSearchDto.queryName;
      this.adForm.patchValue({
        QueryName: this.cDate,
      });
    }
    else {
      this.currentdatetime = new Date().toLocaleString("en-US");
      var tempQueryName = "Query" + this.currentdatetime.replace(',', '').trim();
      this.cDate = tempQueryName.replace(/\s+|[,\/]/g, "/");
    }

    if (this.selectedIndex != 6) {
      this.selectedIndex = 6;
    }

    var formValue = this.adForm.value;

    this.adSearchInput.queryName = formValue.QueryName;
    this.adSearchInput.affiliations = formValue.affiliation.Affiliations.length != 0 ? formValue.affiliation.Affiliations.toString() : "";

    this.adSearchInput.agencyManagement = formValue.company.AgencyMngTypes.length != 0 ? formValue.company.AgencyMngTypes.toString() : "";
    this.adSearchInput.agencyMgntCriteria = formValue.company.AgencyMgntCriteria;
    this.adSearchInput.areaCode = formValue.geographic.AreaCode.length != 0 ? formValue.geographic.AreaCode.toString() : "";

    if (this.CarrierTabShow) {
      this.adSearchInput.companyLines = formValue.carrier.CompanyLines.length != 0 ? formValue.carrier.CompanyLines.toString() : "";
    }
    else {
      this.adSearchInput.companyLines = "";
    }


    this.adSearchInput.companyName = formValue.company.CompanyName;
    //adding minority owned
    this.adSearchInput.minorityOwned = formValue.company.MinorityOwned;
    this.adSearchInput.companyNameType = formValue.company.CompanyNameType;
    this.adSearchInput.country = formValue.geographic.Country;
    this.adSearchInput.county = formValue.geographic.County.length != 0 ? formValue.geographic.County.toString() : "";

    this.adSearchInput.countyIDs = formValue.geographic.CountyIDs.length != 0 ? formValue.geographic.CountyIDs.toString() : "";
    var empsize = this.getVolumn(formValue.company.Empval, formValue.company.EmpvalTo);
    this.adSearchInput.empSize = empsize;
    this.adSearchInput.excludeAreaCode = formValue.geographic.ExcludeAreaCode.length != 0 ? formValue.geographic.ExcludeAreaCode.toString() : "";
    this.adSearchInput.excludeCountiesList = formValue.geographic.ExcludeCountiesList.length != 0 ? formValue.geographic.ExcludeCountiesList.toString() : "";
    this.adSearchInput.excludeCountyIds = formValue.geographic.ExcludeCountyIds.length != 0 ? formValue.geographic.ExcludeCountyIds.toString() : "";
    this.adSearchInput.excludeMSA = formValue.geographic.ExcludeMSA.length != 0 ? formValue.geographic.ExcludeMSA.toString() : "";
    this.adSearchInput.excludeState = formValue.geographic.ExcludeState.length != 0 ? formValue.geographic.ExcludeState.toString() : "";
    this.adSearchInput.excludeZip = formValue.geographic.ExcludeZip.length != 0 ? formValue.geographic.ExcludeZip.toString() : "";

    this.adSearchInput.linesSearch = formValue.contact.LinesSearch.length != 0 ? formValue.contact.LinesSearch.toString() : "";
    this.adSearchInput.linesSearchCriteria = formValue.contact.LinesSearchCriteria;
    this.adSearchInput.msa = formValue.geographic.MSA.length != 0 ? formValue.geographic.MSA.toString() : "";
    this.adSearchInput.pEmpCriteria = formValue.company.PEmpCriteria;
    var pvolume = this.getVolumn(formValue.company.PremValFrom, formValue.company.PremValTo);
    this.adSearchInput.pVolume = pvolume;
    var revenue = this.getVolumn(formValue.company.RVFromval, formValue.company.RVTo);
    this.adSearchInput.revenueValue = revenue;
    this.adSearchInput.revenueCriteria = formValue.company.RevenueCriteria;

    if (this.IndustryTabShow) {
      this.adSearchInput.sicCodes = formValue.industry.SICCodes.length != 0 ? formValue.industry.SICCodes.toString() : "";
      this.adSearchInput.sicids = formValue.industry.SICIDS.length != 0 ? formValue.industry.SICIDS.toString() : "";
    }
    else {
      this.adSearchInput.sicCodes = "";
      this.adSearchInput.sicids = "";
    }

    this.adSearchInput.state = formValue.geographic.State.length != 0 ? formValue.geographic.State.toString() : "";
    this.adSearchInput.titleSearch = formValue.contact.TitleSearch.length != 0 ? formValue.contact.TitleSearch.toString() : "";
    this.adSearchInput.titleSearchCriteria = formValue.contact.TitleSearchCriteria;
    this.adSearchInput.typeCriteria = formValue.company.TypeCriteria;
    this.adSearchInput.typeField = formValue.company.TypeField.length != 0 ? formValue.company.TypeField.toString() : "";
    this.adSearchInput.zip = formValue.geographic.Zip.length != 0 ? formValue.geographic.Zip.toString() : "";

    this._adService.postADSearchInput(this.adSearchInput).subscribe(data => {

      this.adCountsDto = data;
      this.adCounts = this.adCountsDto.adCounts;
      this.adQueries = this.adCountsDto.adQueries;
      this.getSearchSummary();
      this.saving = false;
    });

  }

  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  formInitialized(name: string, form: FormGroup) {
    this.adForm.setControl(name, form);
    this.activeFormName = name;




  }

  getVolumn(From: string, To: string): string {
    if (From != "" && To != "") {
      return From + "-" + To;
    }
    else if (From != "" && To == "") {
      return From;// + "-" + "0";
    }
    else if (From == "" && To != "") {
      return "0" + "-" + To;
    }
    else {
      return "";
    }
  }
  onChangeBrk1(breakdownValue) {
    this.selectedBreakdown1 = breakdownValue;
    this.selectedBreakdown1 != "" && this.selectedBreakdown2 != "" && this.selectedBreakdown1 != this.selectedBreakdown2 ? this.isAnalyze = true : this.isAnalyze = false;
  }
  onChangeBrk2(breakdownValue) {
    this.selectedBreakdown2 = breakdownValue;
    this.selectedBreakdown1 != "" && this.selectedBreakdown2 != "" && this.selectedBreakdown1 != this.selectedBreakdown2 ? this.isAnalyze = true : this.isAnalyze = false;
  }
  tabChange(index: number) {
    this.showCountButton = true;
    if (index === 5 && this.IndustryTabShow != true) {
      this.IndustryTabShow = true;
    }
    else if (index === 2 && this.CarrierTabShow != true) {
      this.CarrierTabShow = true;
    }
    else if (index === 6) {
      this.save();
    }
  }

  getSearchSummary() {
    this.searchSummary = [];
    var formValue = this.adForm.value;
    if (this.adSearchInput.typeField != "") {
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Business Type Criteria', 'value': this.adSearchInput.typeCriteria });
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Business Type', 'value': this.adSearchInput.typeField.replace(",", ", ") });
    }
    if (this.adSearchInput.pVolume != "")
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Premium Volume', 'value': this.adSearchInput.pVolume });

    if (this.adSearchInput.revenueValue != "")
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Revenue', 'value': this.adSearchInput.revenueValue });

    if (this.adSearchInput.empSize != "")
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Employee Size', 'value': this.adSearchInput.empSize });

    if (this.adSearchInput.companyName != "") {
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Company Search Type', 'value': this.adSearchInput.companyNameType });
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Company Name', 'value': this.adSearchInput.companyName });
    }
    //adding  minority owned
    if (this.adSearchInput.minorityOwned != "")
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Minority Owned', 'value': this.adSearchInput.minorityOwned });
    if (this.adSearchInput.agencyManagement != "")
      this.searchSummary.push({ 'page': 'COMPANY', 'criteria': 'Agency Mgnt', 'value': this.adSearchInput.agencyManagement.replace(",", ", ") + " {" + this.adSearchInput.agencyMgntCriteria + "}" });

    if (this.adSearchInput.country != "")
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'Country List', 'value': this.adSearchInput.country.replace(",", ", ") });

    if (this.adSearchInput.state != "")
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'States-Include', 'value': this.adSearchInput.state.replace(",", ", ") });

    if (this.adSearchInput.excludeState != "")
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'States-Exclude', 'value': this.adSearchInput.excludeState.replace(",", ", ") });

    if (this.adSearchInput.county != "")
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'County Names-Include', 'value': this.adSearchInput.county.replace(",", ", ") + ': [' + this.adSearchInput.countyIDs + ']' });

    if (this.adSearchInput.excludeCountiesList != "")
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'County Names-Exclude', 'value': this.adSearchInput.excludeCountiesList.replace(",", ", ") + ': [' + this.adSearchInput.excludeCountyIds + ']' });

    if (this.adSearchInput.zip != "")
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'Zip Codes-Include', 'value': this.adSearchInput.zip.replace(",", ", ") });

    if (this.adSearchInput.excludeZip != "")
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'Zip Codes-Exclude', 'value': this.adSearchInput.excludeZip.replace(",", ", ") });


    if (this.adSearchInput.msa != "")
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'Metro Areas-Include', 'value': formValue.geographic.MSAString.toString().replace(",", ", ") + ' [' + this.adSearchInput.msa + ']' });

    if (this.adSearchInput.excludeMSA != "")
      this.searchSummary.push({ 'page': 'GEOGRAPHIC', 'criteria': 'Metro Areas-Exclude', 'value': formValue.geographic.MSAStringEx.toString().replace(",", ", ") + ' [' + this.adSearchInput.excludeMSA + ']' });


    if (this.adSearchInput.affiliations != "")
      this.searchSummary.push({ 'page': 'AFFILIATIONS', 'criteria': 'Affiliations List', 'value': this.adSearchInput.affiliations.replace(",", ", ") });

    if (this.adSearchInput.sicids != "" && this.IndustryTabShow)
      this.searchSummary.push({ 'page': 'CLASS OF BUSINESS', 'criteria': 'SIC List', 'value': formValue.industry.sicNames.toString().replace(",", ", ") });

    if (this.adSearchInput.titleSearch != "")
      this.searchSummary.push({ 'page': 'CONTACTS', 'criteria': 'Title Search', 'value': this.adSearchInput.titleSearch.replace(",", ", ") + " {" + this.adSearchInput.titleSearchCriteria + "}" });

    if (this.adSearchInput.linesSearch != "")
      this.searchSummary.push({ 'page': 'CONTACTS', 'criteria': 'Lines Search', 'value': this.adSearchInput.linesSearch.replace(",", ", ") + " {" + this.adSearchInput.linesSearchCriteria + "}" });

  }

  onPurchaseAgencyClick() {
    this.adSearchInput.queryCriteria = this.adQueries.agencyQuery;
    this.dataService.setData('isCtPurchased', false);
    this.onPurchase();
  }

  onPurchaseContactsClick() {
    this.adSearchInput.queryCriteria = this.adQueries.sqlQuery;
    this.dataService.setData('isCtPurchased', true);
    this.onPurchase();
  }

  onPurchase() {
    if (this.saveSearchID != null) {
      this._adService.getAdSearch(this.saveSearchID).subscribe(data => {
        this.adSearchDto = data;
        this.adSearchDto.queryCriteria = this.adSearchInput.queryCriteria;
        this._adService.update(this.adSearchDto).subscribe(() => {
          this.setData();
          this.router.navigate(['/app/ad-names']);
        });
      });
    } else {
      this._adService.create(this.adSearchInput).subscribe(data => {
        this.saveSearchID = data;
        this.setData();
        this.router.navigate(['/app/ad-names']);
      });
    }
  }

  setData() {
    this.dataService.setData('queryId', this.saveSearchID);
    this.dataService.setData('queryName', this.adSearchInput.queryName);
    this.dataService.setData('agencyQuery', this.adQueries.agencyQuery);
    this.dataService.setData('sqlQuery', this.adQueries.sqlQuery);
    this.dataService.setData('agencyCount', +this.adCounts.agencyListCount);
    this.dataService.setData('contactCount', +this.adCounts.adContactsCount);
  }
  fun() {

    this.saving = true;
    this.showCountButton = false;
    abp.message.confirm(
      this.l('BDSearchDeleteWarningMessage'),
      (result: boolean) => {
        if (result) {

        }
      }
    );
  }

}

