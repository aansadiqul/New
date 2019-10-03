import { Component, Output, OnInit, EventEmitter, Injector } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, FormControl } from '@angular/forms';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import {
  CommonServiceProxy,
  CommonDto,
  ADSearchDto
} from '@shared/service-proxies/service-proxies';
@Component({
  selector: 'app-carriers',
  templateUrl: './carriers.component.html',
  animations: [appModuleAnimation()],
  styleUrls: ['./carriers.component.css']
})
export class CarriersComponent extends AppComponentBase implements OnInit {
  carrierDto: any = [];
  carrierTmp: any = [];
  show: boolean = true;
  //Child form Logic//
  @Output() formReady = new EventEmitter<FormGroup>()
  carrierForm: FormGroup;


  constructor(private fb: FormBuilder, injector: Injector,
    private _commonService: CommonServiceProxy) { super(injector); }

  ngOnInit() {
    this.carrierlist();
    this.carrierForm = this.fb.group({
      CarrierManageCrieteria: "INCLUDE",
      CompanyLines: new FormArray([])
    });
    this.formReady.emit(this.carrierForm);
  }
  onChangeCarrier(carrierName: string, event) {
    const FieldArray = <FormArray>this.carrierForm.controls.CompanyLines;
    if (event.checked) {
      FieldArray.push(new FormControl(carrierName));
    } else {
      let index = FieldArray.controls.findIndex(x => x.value == carrierName);
      FieldArray.removeAt(index);
    }

    //console.log(this.carrierForm);
  }
  filterItem(event: any) {
    var value = event.target.value;
    if (!value) {
      this.carrierTmp = this.carrierDto;
    } // when nothing has typed
    const companylinearr = <FormArray>this.carrierForm.controls.CompanyLines;
    this.carrierDto = this.carrierDto.map(item => {
      return {
        id: item.id,
        name: item.name,
        isChecked: companylinearr.value.indexOf(item.name) > -1 ? true : false,
      };
    });
    this.carrierTmp = Object.assign([], this.carrierDto).filter(
      item => item.name.toLowerCase().indexOf(value.toLowerCase()) > -1
    )
  }
  carrierlist(): void {

    this._commonService
      .getCarriers()
      .subscribe(result => {
        // console.log(result);
        this.carrierDto = result;
        if (this.carrierTmp.length == 0)
          this.carrierTmp = result;

        this.show = false;
      });

  }

  async editCarrierAdSearch(myAdSearchDto: ADSearchDto) {
    ///Select Company Types for Edit
    await this._commonService
      .getCarriers()
      .toPromise().then(result => {
        this.carrierDto = result;
        this.carrierTmp = result;
        this.show = false;
      });
    const CompanyLinesArray = <FormArray>this.carrierForm.controls.CompanyLines;
    this.carrierTmp = this.carrierTmp.map(item => {
      if (myAdSearchDto.companyLines.split(',').indexOf(item.name) > -1) {
        CompanyLinesArray.push(new FormControl(item.name));
      }
      return {
        name: item.name,
        id: item.id,
        isChecked: myAdSearchDto.companyLines.split(',').indexOf(item.name) > -1 ? true : false,
      };
    });
  }
  fun() {
    // e.preventDefault();

    // console.log();
    // for (let i = 0; i < this.carrierTmp.length; i++) {
    //   this.carrierTmp[i].isChecked = val;


    // }
    // const FieldArray = <FormArray>this.carrierForm.controls.CompanyLines;
    // while (FieldArray.length !== 0) {
    //   FieldArray.removeAt(0)
    // }
    console.log("lkashfkjg");

  }





}
