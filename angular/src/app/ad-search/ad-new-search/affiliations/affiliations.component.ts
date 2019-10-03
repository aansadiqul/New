import { Component, Output, OnInit,EventEmitter, Injector } from '@angular/core';
import { FormGroup, FormBuilder,FormArray,FormControl } from '@angular/forms';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import {
  CommonServiceProxy,
  CommonDto,
  ADSearchDto
} from '@shared/service-proxies/service-proxies';
@Component({
  selector: 'app-affiliations',
  templateUrl: './affiliations.component.html',
  animations: [appModuleAnimation()],
  styleUrls: ['./affiliations.component.css']
})
export class AffiliationsComponent extends AppComponentBase implements OnInit {
  affiliationsDto: any = [];
  affiliationsTmp : any = [];
  show:boolean = true;

  @Output() formReady = new EventEmitter<FormGroup>()
  affiliationForm: FormGroup;
   
  constructor(private fb: FormBuilder,injector: Injector,
    private _commonService: CommonServiceProxy) { super(injector);}

  ngOnInit() {
    this.affiliationlist();
    this.affiliationForm = this.fb.group({     
      Affiliations: new FormArray([])
    });
    this.formReady.emit(this.affiliationForm);     
  }

  affiliationlist(): void {

    this._commonService
        .getAffiliations()      
        .subscribe(result => {
         // console.log(result);
        this.affiliationsDto = result; 
        if(this.affiliationsTmp.length==0)
        this.affiliationsTmp = result;    
        this.show = false;      
    });   
         
  }
  filterItem(event: any){
    var value=event.target.value;
    if(!value){
      this.affiliationsTmp=this.affiliationsDto;
    } // when nothing has typed
    //Assign Selected Items
    const Affiliationsarr=<FormArray>this.affiliationForm.controls.Affiliations;   
    this.affiliationsDto=this.affiliationsDto.map(item => {      
      return {
        id: item.id,
        name: item.name,
        isChecked: Affiliationsarr.value.indexOf(item.name) > -1 ? true : false,
      };
    });
    this.affiliationsTmp = Object.assign([], this.affiliationsDto).filter(
       item => item.name.toLowerCase().indexOf(value.toLowerCase()) > -1
    )
 }
  onChangeAffiliations(affiliationName:string, event) {
    const FieldArray = <FormArray>this.affiliationForm.controls.Affiliations;
    if(event.checked) {
      FieldArray.push(new FormControl(affiliationName));
    } else {
      let index = FieldArray.controls.findIndex(x => x.value == affiliationName);
      FieldArray.removeAt(index);
    }
    //console.log(this.carrierForm);
  }

  async editAffiliationAdSearch(myAdSearchDto: ADSearchDto){
    ///Select Company Types for Edit
    await this._commonService
        .getAffiliations()      
        .toPromise().then(result => {     
        this.affiliationsDto = result; 
        this.affiliationsTmp = result;    
        this.show = false;      
    });   
    const AffiliationsArray = <FormArray>this.affiliationForm.controls.Affiliations;
      this.affiliationsTmp =  this.affiliationsTmp.map(item => {     
      if(myAdSearchDto.affiliations.split(',').indexOf(item.name) > -1) {
        AffiliationsArray.push(new FormControl(item.name));
      }
      return {
        name : item.name,
        id: item.id,      
        isChecked: myAdSearchDto.affiliations.split(',').indexOf(item.name) > -1 ? true : false,
      };
    });
  }
}
