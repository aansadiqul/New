import { Component, Output, Input, EventEmitter, OnInit, Injector } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, FormControl } from '@angular/forms';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { NgForm } from '@angular/forms';
import {
  MatCheckboxChange
} from '@angular/material';
import {
  CommonServiceProxy,
  CommonDto,
  ADSearchDto
} from '@shared/service-proxies/service-proxies';
import { listLazyRoutes } from '@angular/compiler/src/aot/lazy_routes';
//import { of } from 'rxjs';

@Component({
  selector: 'app-company',
  templateUrl: './company.component.html',
  animations: [appModuleAnimation()],
  styleUrls: ['./company.component.css']
})
export class CompanyComponent extends AppComponentBase implements OnInit {
  companyTypes: any = [];
  companyTypesDto: any = [];
  //adCountsDto: ADSearchDto = new ADSearchDto();
  RVFromval: string = "";
  Empval: string = "";
  amgList: any = [];
  amgListDto: any = [];

  isChecked: boolean = false;
  chkvalue: string = "";
  ss: string = "";
  sst: string = "";


  //Child form Logic//

  @Output() formReady = new EventEmitter<FormGroup>()
  companyForm: FormGroup;

  constructor(private fb: FormBuilder, injector: Injector,
    private _companyTypesService: CommonServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();
    this.companyForm = this.fb.group({
      TypeCriteria: "CONTAINS",
      TypeField: new FormArray([]),
      PremValFrom: "",
      PremValTo: "",
      PEmpCriteria: "OR",
      RVFromval: "",
      RVTo: "",
      RevenueCriteria: "OR",
      Empval: "",
      EmpvalTo: "",
      AgencyMgntCriteria: "INCLUDE",
      AgencyMngTypes: new FormArray([]),
      CompanyName: "",
      CompanyNameType: "BEGINS",
      MinorityOwned: "0",

    });
    this.formReady.emit(this.companyForm);
    console.log(this.companyForm + "company form in company page");
  }


  onChange(companyType: string, event) {
    const TypeFieldArray = <FormArray>this.companyForm.controls.TypeField;
    if (event.checked) {
      TypeFieldArray.push(new FormControl(companyType));

    } else {
      let index = TypeFieldArray.controls.findIndex(x => x.value == companyType);
      TypeFieldArray.removeAt(index);
    }
  }
  onChangeAgencyManage(AgencyManageType: string, event) {
    //alert(AgencyManageType);
    const TypeFieldArray = <FormArray>this.companyForm.controls.AgencyMngTypes;
    if (event.checked) {
      //alert('Paisi' + AgencyManageType);
      TypeFieldArray.push(new FormControl(AgencyManageType));
    } else {
      let index = TypeFieldArray.controls.findIndex(x => x.value == AgencyManageType);
      TypeFieldArray.removeAt(index);
    }
  }
  async list() {

    await this._companyTypesService
      .getCompanyTypes()
      .toPromise().then(result => {
        this.companyTypes = result;
        this.companyTypesDto = result;

      });
    await this._companyTypesService
      .getAMGList()
      .toPromise().then(result => {
        this.amgList = result;
        this.amgListDto = result;
      });
  }

  onPremValChange(premValue: string) {
    this.RVFromval = (0.12 * parseFloat(premValue)) ? (0.12 * parseFloat(premValue)).toString() : "";
    this.Empval = parseFloat(premValue) >= 1 ? premValue.toString() : "";
    //  this.reset(premValue);
  }
  filterItem(event: any) {
    var value = event.target.value;
    if (!value) {
      this.companyTypes = this.companyTypesDto;
    } // when nothing has typed

    const Typefieldarr = <FormArray>this.companyForm.controls.TypeField;
    this.companyTypesDto = this.companyTypesDto.map(item => {
      return {
        id: item.id,
        name: item.name,
        isChecked: Typefieldarr.value.indexOf(item.name) > -1 ? true : false,
      };
    });

    this.companyTypes = Object.assign([], this.companyTypesDto).filter(
      item => item.name.toLowerCase().indexOf(value.toLowerCase()) > -1
    );
  }

  filterAMSItem(event: any) {
    let value = event.target.value;
    if (!value) {
      this.amgList = this.amgListDto;
    } // when nothing has typed

    const TypeFieldArray = <FormArray>this.companyForm.controls.AgencyMngTypes;

    this.amgListDto = this.amgListDto.map(item => {
      return {
        id: item.id,
        name: item.name,
        isChecked: TypeFieldArray.value.indexOf(item.name) > -1 ? true : false,
      };
    });

    this.amgList = Object.assign([], this.amgListDto).filter(
      item => item.name.toLowerCase().indexOf(value.toLowerCase()) > -1
    );
  }

  async editCompany(myAdSearchDto: ADSearchDto) {
    ///Select Company Types for Edit
    if (this.companyTypes.length == 0 || this.amgList.length == 0) {
      await this.list();
    }
    const TypeFieldArray = <FormArray>this.companyForm.controls.TypeField;
    this.companyTypes = this.companyTypes.map(item => {
      if (myAdSearchDto.typeField.split(',').indexOf(item.name) > -1) {
        TypeFieldArray.push(new FormControl(item.name));
      }
      return {
        id: item.id,
        name: item.name,
        isChecked: myAdSearchDto.typeField.split(',').indexOf(item.name) > -1 ? true : false,
      };
    });
    ///Select Agency Management Edit
    const AgencyMngTypesArray = <FormArray>this.companyForm.controls.AgencyMngTypes;
    this.amgList = this.amgList.map(item => {
      if (myAdSearchDto.agencyManagement.split(',').indexOf(item.name) > -1) {
        AgencyMngTypesArray.push(new FormControl(item.name));
      }
      return {
        id: item.id,
        name: item.name,
        isChecked: myAdSearchDto.agencyManagement.split(',').indexOf(item.name) > -1 ? true : false,
      };
    });


    this.companyForm.patchValue({
      TypeCriteria: myAdSearchDto.typeCriteria,
      PremValFrom: myAdSearchDto.pVolume != "" ? myAdSearchDto.pVolume.split('-')[0] : "",
      PremValTo: myAdSearchDto.pVolume.split('-')[1] ? myAdSearchDto.pVolume.split('-')[1] : "",
      PEmpCriteria: myAdSearchDto.pEmpCriteria,
      RVFromval: myAdSearchDto.revenueValue != "" ? myAdSearchDto.revenueValue.split('-')[0] : "",
      RVTo: myAdSearchDto.revenueValue.split('-')[1] ? myAdSearchDto.revenueValue.split('-')[1] : "",
      RevenueCriteria: myAdSearchDto.revenueCriteria,
      Empval: myAdSearchDto.empSize != "" ? myAdSearchDto.empSize.split('-')[0] : "",
      EmpvalTo: myAdSearchDto.empSize.split('-')[1] ? myAdSearchDto.empSize.split('-')[1] : "",
      AgencyMgntCriteria: myAdSearchDto.agencyMgntCriteria,
      //AgencyMngTypes : myAdSearchDto.agencyManagement.split(','),
      CompanyName: myAdSearchDto.companyName,
      CompanyNameType: myAdSearchDto.companyNameType,
      MinorityOwned: myAdSearchDto.minorityOwned,
    });

  }
  SelectAll(event: MatCheckboxChange) {
    const TypeFieldArray = <FormArray>this.companyForm.controls.TypeField;
    if (event.checked) {
      this.companyTypes = this.companyTypes.map(item => {
        TypeFieldArray.push(new FormControl(item.name));
        return {
          id: item.id,
          name: item.name,
          isChecked: true,
        };
      });
    }
    else {
      this.companyTypes = this.companyTypes.map(item => {
        let index = TypeFieldArray.controls.findIndex(x => x.value == item.name);
        TypeFieldArray.removeAt(index);
        return {
          id: item.id,
          name: item.name,
          isChecked: false,
        };
      });
    }
  }
  fun(e, val) {
    e.preventDefault();

    console.log(this.amgList);
    for (let i = 0; i < this.amgList.length; i++) {
      this.amgList[i].isChecked = val;
    }


    // }
    const FieldArray = <FormArray>this.companyForm.controls.AgencyMngTypes;
    while (FieldArray.length !== 0) {
      FieldArray.removeAt(0)
    }

  }
  reset(form: NgForm) {
    form.resetForm();
    // this.ss = "";
    //this.sst = "";
  }



}
