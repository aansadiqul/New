import { Component, Output, EventEmitter, OnInit,Injector } from '@angular/core';
import { FormGroup, FormBuilder,FormArray,FormControl } from '@angular/forms';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import {
  CommonServiceProxy,
  CommonDto,
  ADSearchDto 
} from '@shared/service-proxies/service-proxies';
import { of } from 'rxjs';

@Component({
  selector: 'app-contacts',
  templateUrl: './contacts.component.html',
  animations: [appModuleAnimation()],
  styleUrls: ['./contacts.component.css']
})
export class ContactsComponent extends AppComponentBase implements OnInit {
  contactTitles: any = [];
  productLines: any = [];

  //Child form Logic//
  @Output() formReady = new EventEmitter<FormGroup>()
  contactsForm: FormGroup;

  constructor(private fb: FormBuilder,injector: Injector,
    private _commonService: CommonServiceProxy
    ) { 
      super(injector);
  }

  ngOnInit() {
    this.list();

    this.contactsForm = this.fb.group({ 
      TitleSearchCriteria : "INCLUDE",     
      TitleSearch : new FormArray([]),
      LinesSearchCriteria  :"INCLUDE",
      LinesSearch:  new FormArray([])
    });
    
    this.formReady.emit(this.contactsForm);   
  }

  list(): void {

    this._commonService
    .getContactTitles()
    .subscribe(result => {   
        this.contactTitles = result;   
    });

    this._commonService
    .getProductLines()
    .subscribe(result => {   
        this.productLines = result;   
    });
  }

  onChangeContactTitle(contacttitle:string, event) {
    const FieldArray = <FormArray>this.contactsForm.controls.TitleSearch;
    if(event.checked) {
      FieldArray.push(new FormControl(contacttitle));
    } else {
      let index = FieldArray.controls.findIndex(x => x.value == contacttitle);
      FieldArray.removeAt(index);
    }
  }

  onChangeProductLine(productline:string, event) {
    const FieldArray = <FormArray>this.contactsForm.controls.LinesSearch;
    if(event.checked) {
      FieldArray.push(new FormControl(productline));
    } else {
      let index = FieldArray.controls.findIndex(x => x.value == productline);
      FieldArray.removeAt(index);
    }
  }

  async editContactsAdSearch(myAdSearchDto: ADSearchDto){
    await this._commonService
    .getContactTitles()
    .toPromise().then(result => { 
        //this.contactTitles = result;   
        this.contactTitles=result.map(item => {
          return {
            id: item.id,
            name: item.name,
            isChecked : myAdSearchDto.titleSearch.split(',').indexOf(item.name) > -1 ? true : false,
          };
        });  
    });
    const TitleSearchArray = <FormArray>this.contactsForm.controls.TitleSearch;
    for (let contacttitle of this.contactTitles) {        
      if( myAdSearchDto.titleSearch.split(',').indexOf(contacttitle.name) > -1 ){
        TitleSearchArray.push(new FormControl(contacttitle.name));
      }
    }
    await this._commonService
    .getProductLines()
    .toPromise().then(result => {           
        this.productLines=result.map(item => {
          return {
            id: item.id,
            name: item.name,
            isChecked : myAdSearchDto.linesSearch.split(',').indexOf(item.name) > -1 ? true : false,
          };
        });  
    });
    const LinesSearchArray = <FormArray>this.contactsForm.controls.LinesSearch;
    for (let product of this.productLines) {        
      if( myAdSearchDto.linesSearch.split(',').indexOf(product.name) > -1 ){
        LinesSearchArray.push(new FormControl(product.name));
      }
    }
    this.contactsForm.patchValue({
      TitleSearchCriteria : myAdSearchDto.titleSearchCriteria,   
      LinesSearchCriteria  :myAdSearchDto.linesSearchCriteria
    });

  }

}
