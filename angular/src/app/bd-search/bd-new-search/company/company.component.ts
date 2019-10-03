import { Component, Output, EventEmitter, OnInit, Injector } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, FormControl } from '@angular/forms';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';

import {
  CommonServiceProxy,
  CommonDto,
  BDSearchDto
} from '@shared/service-proxies/service-proxies';
import { of } from 'rxjs';

@Component({
  selector: 'app-bd-company',
  templateUrl: './company.component.html',
  animations: [appModuleAnimation()],
  styleUrls: ['./company.component.css']
})
export class CompanyBdComponent extends AppComponentBase implements OnInit {
  companyTypes: CommonDto[] = [];

  RVFromval = '';
  Empval = '';
  siteList = [
    { id: '0', name: 'Single Location', isChecked: false },
    { id: '1', name: 'Headquarters', isChecked: false },
    { id: '2', name: 'Branch', isChecked: false }
  ];

  manufacturingList = [
    { id: '0', name: 'Manufacturing', isChecked: false },
    { id: '1', name: 'Non-Manufacturing', isChecked: false }
  ];
  isChecked = false;
  chkvalue = '';


  //Child form Logic//
  @Output() formReady = new EventEmitter<FormGroup>();
  companyForm: FormGroup;

  constructor(private fb: FormBuilder, injector: Injector,
    private _companyTypesService: CommonServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.companyForm = this.fb.group({
      CompanyAndOr: 'OR',
      SalesFrom: '',
      SalesTo: '',
      EmployeesFrom: '',
      EmployeesTo: '',
      Locations: new FormArray([]),
      Manufacturing: new FormArray([]),
      CompanyName: '',
      CompanyBeginsContains: 'Begins',
    });

    this.formReady.emit(this.companyForm);
  }


  onPremValChange(premValue: string) {

    this.RVFromval = (0.12 * parseFloat(premValue)) ? (0.12 * parseFloat(premValue)).toString() : "";
    this.Empval = parseFloat(premValue) >= 1 ? premValue.toString() : "";

  }

  checkCheckBoxvalue(event) {
    console.log(event)
  }

  onChangeSite(siteName: string, event) {
    const siteArray = <FormArray>this.companyForm.controls.Locations;
    if (event.checked) {
      siteArray.push(new FormControl(siteName));
    } else {
      let index = siteArray.controls.findIndex(x => x.value == siteName);
      siteArray.removeAt(index);
    }
  }

  onChangeManufacture(mName: string, event) {
    const mArray = <FormArray>this.companyForm.controls.Manufacturing;
    if (event.checked) {
      mArray.push(new FormControl(mName));
    } else {
      let index = mArray.controls.findIndex(x => x.value == mName);
      mArray.removeAt(index);
    }
  }

  async editCompany(bdSearchDto: BDSearchDto) {

    const LocationsArray = <FormArray>this.companyForm.controls.Locations;
    this.siteList = this.siteList.map(item => {
      if (bdSearchDto.locations.split(',').indexOf(item.name) > -1) {
        LocationsArray.push(new FormControl(item.name));
      }
      return {
        id: item.id,
        name: item.name,
        isChecked: bdSearchDto.locations.split(',').indexOf(item.name) > -1 ? true : false,
      };
    });
    ///Select Agency Management Edit
    const manufacturingArray = <FormArray>this.companyForm.controls.Manufacturing;
    this.manufacturingList = this.manufacturingList.map(item => {
      if (bdSearchDto.manufacturing.split(',').indexOf(item.name) > -1) {
        manufacturingArray.push(new FormControl(item.name));
      }
      return {
        id: item.id,
        name: item.name,
        isChecked: bdSearchDto.manufacturing.split(',').indexOf(item.name) > -1 ? true : false,
      };
    });


    this.companyForm.patchValue({
      CompanyAndOr: bdSearchDto.companyAndOr,
      SalesFrom: bdSearchDto.salesFrom,
      SalesTo: bdSearchDto.salesTo,
      EmployeesFrom: bdSearchDto.employeesFrom,
      EmployeesTo: bdSearchDto.employeesTo,
      CompanyName: bdSearchDto.companyName,
      CompanyBeginsContains: bdSearchDto.companyBeginsContains,
    });

  }
}
