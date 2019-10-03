import { Component, Output, Input, EventEmitter, OnInit, Injector } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, FormControl, NgForm } from '@angular/forms';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { MatRadioChange } from '@angular/material';
import { CommonComponentsHelper } from '@shared/helpers/commonComponents';
import {
  CommonServiceProxy,
  CommonDto,
  ADSearchDto,
  BDSearchDto
} from '@shared/service-proxies/service-proxies';
import { of } from 'rxjs';
import { resolve } from 'url';

@Component({
  selector: 'app-geographic',
  templateUrl: './geographic.component.html',

  styleUrls: ['./geographic.component.css']
})
export class GeographicComponent extends AppComponentBase implements OnInit {
  @Input() isAdSearch: boolean = false;

  IncludeStates: string = "";
  ExcludedStates: string = "";

  commonVar = new CommonComponentsHelper();
  countryList: any = this.commonVar.countryList;

  //Child form Logic//
  @Output() formReady = new EventEmitter<FormGroup>()

  geographicForm: FormGroup;

  ///States Variables
  states: any = this.commonVar.countryLevelChangeAction('United States');
  ExcludeStates: any = this.states;
  settings = {
    singleSelection: false,
    enableSearchFilter: true,
    text: "Select States",
    selectAllText: 'Select All',
    unSelectAllText: 'Unselect All'
  };
  selectedItems: any = [];
  selectedItemsExclude: any = [];
  /////Counties Variables
  counties: any = [];
  selectedCounties: any = [];
  countiesExclude: any = [];
  selectedCountiesExclude: any = [];
  settingsCounties = {
    singleSelection: false,
    text: "Select Counties",
    selectAllText: 'Select All',
    unSelectAllText: 'Unselect All',
    enableSearchFilter: true,
    badgeShowLimit: 40,
    noDataLabel: 'Click on Load Counties to get data'
  };
  ////Zip Codes variable
  zip3digits: any = [];
  selectedZip3digits: any = [];
  zip5digits: any = [];
  selectedZip5digits: any = [];

  zip3digitsEx: any = [];
  selectedZip3digitsEx: any = [];
  zip5digitsEx: any = [];
  selectedZip5digitsEx: any = [];

  settingsZip = {
    singleSelection: false,
    text: "Select Zip Codes",
    selectAllText: 'Select All',
    unSelectAllText: 'Unselect All',
    enableSearchFilter: true,
    badgeShowLimit: 40,
    noDataLabel: 'Click on Load Zip Codes to get data'
  };

  ////Metro Areas variable
  metroAreas: any = [];
  selectedMetroAreas: any = [];
  metroAreasEx: any = [];
  selectedMetroAreasEx: any = [];
  settingsMetroAreas = {
    singleSelection: false,
    text: "Select Metro Areas",
    selectAllText: 'Select All',
    unSelectAllText: 'Unselect All',
    enableSearchFilter: true,
    badgeShowLimit: 40,
    noDataLabel: 'Click on Load Metro Areas to get data'
  };
  ////Area Codes variable/////
  areaCodes: any = [];
  selectedAreaCodes: any = [];
  areaCodesEx: any = [];
  selectedAreaCodesEx: any = [];
  settingsAreaCodes = {
    singleSelection: false,
    text: "Select Area Codes",
    selectAllText: 'Select All',
    unSelectAllText: 'Unselect All',
    enableSearchFilter: true,
    badgeShowLimit: 40,
    noDataLabel: 'Click on Load Metro Areas to get data'
  };

  constructor(private fb: FormBuilder, injector: Injector,
    private _commonService: CommonServiceProxy
  ) {
    super(injector);
  }
  ngOnInit() {
    this.geographicForm = this.fb.group({
      Country: "USA",
      State: new FormArray([]),
      ExcludeState: new FormArray([]),

      County: new FormArray([]),
      CountyIDs: new FormArray([]),
      ExcludeCountyIds: new FormArray([]),
      ExcludeCountiesList: new FormArray([]),

      Zip: new FormArray([]),
      ExcludeZip: new FormArray([]),
      MSA: new FormArray([]),
      MSAString: new FormArray([]),
      ExcludeMSA: new FormArray([]),
      MSAStringEx: new FormArray([]),
      AreaCode: new FormArray([]),
      ExcludeAreaCode: new FormArray([])
    });
    this.formReady.emit(this.geographicForm);
    /*var mystring="Arlington:VA";
    var newString = mystring.split(":")[0];
    console.log(newString);*/


    this.states = this.states.map(item => {
      return {
        id: item.value,
        itemName: item.text
      };
    });
    this.ExcludeStates = this.states;
  }
  countryChange(event: MatRadioChange): void {
    this.states = event.value == 'USA' ? this.commonVar.countryLevelChangeAction('United States') : this.commonVar.countryLevelChangeAction('Canada');
    this.states = this.states.map(item => {
      return {
        id: item.value,
        itemName: item.text
      };
    });

  }
  ////State Include List////
  onItemSelect(item: any, type: string) {
    this.populateFormIDs(item.id, true, type);
    if (type == "county") {
      this.populateFormIDs(item.itemName.split(":")[0], true, "countyText");
    }
    if (type == "countyEx") {
      this.populateFormIDs(item.itemName.split(":")[0], true, "countyExText");
    }

    if (type == "meto") {
      this.populateFormIDs(item.itemName.split(":")[0], true, "metroText");
    }
    if (type == "metoEx") {
      this.populateFormIDs(item.itemName.split(":")[0], true, "metroExText");
    }
  }
  OnItemDeSelect(item: any, type: string) {
    this.populateFormIDs(item.id, false, type);
    if (type == "county") {
      this.populateFormIDs(item.itemName.split(":")[0], false, "countyText");
    }
    if (type == "countyEx") {
      this.populateFormIDs(item.itemName.split(":")[0], false, "countyExText");
    }
    if (type == "meto") {
      this.populateFormIDs(item.itemName.split(":")[0], false, "metroText");
    }
    if (type == "metoEx") {
      this.populateFormIDs(item.itemName.split(":")[0], false, "metroExText");
    }
  }
  onSelectAll(items: any, type: string) {
    console.log(2);
    for (let item of items) {
      if (type == "county") {
        this.populateFormIDs(item.itemName.split(":")[0], true, "countyText");
      }
      if (type == "countyEx") {
        this.populateFormIDs(item.itemName.split(":")[0], true, "countyExText");
      }
      if (type == "meto") {
        this.populateFormIDs(item.itemName.split(":")[0], true, "metroText");
      }
      if (type == "metoEx") {
        this.populateFormIDs(item.itemName.split(":")[0], true, "metroExText");
      }
      this.populateFormIDs(item.id, true, type);
    }

  }
  onDeSelectAll(items: any, type: string) {
    // console.log(items.itemName);
    console.log(items);
    console.log(type);
    for (let item of items) {
      if (type == "county") {
        this.populateFormIDs(item.itemName.split(":")[0], false, "countyText");
        // console.log(item);
        //console.log(item.itemName);
      }
      if (type == "countyEx") {
        this.populateFormIDs(item.itemName.split(":")[0], false, "countyExText");
      }
      if (type == "meto") {
        this.populateFormIDs(item.itemName.split(":")[0], false, "metroText");
      }
      if (type == "metoEx") {
        this.populateFormIDs(item.itemName.split(":")[0], false, "metroExText");
      }
      this.populateFormIDs(item.id, false, type);
    }

  }
  populateFormIDs(ID, isPush, type) {
    // console.log(ID);
    var FieldArray;
    switch (type) {
      case "state":
        FieldArray = <FormArray>this.geographicForm.controls.State;
        break;
      case "stateEx":
        FieldArray = <FormArray>this.geographicForm.controls.ExcludeState;
        break;
      case "metroText":
        FieldArray = <FormArray>this.geographicForm.controls.MSAString;
        break;
      case "metroExText":
        FieldArray = <FormArray>this.geographicForm.controls.MSAStringEx;
        break;
      case "county":
        FieldArray = <FormArray>this.geographicForm.controls.CountyIDs;
        break;
      case "countyText":
        FieldArray = <FormArray>this.geographicForm.controls.County;
        break;
      case "countyEx":
        FieldArray = <FormArray>this.geographicForm.controls.ExcludeCountyIds;
        break;
      case "countyExText":
        FieldArray = <FormArray>this.geographicForm.controls.ExcludeCountiesList;
        break;
      case "3digit":
        FieldArray = <FormArray>this.geographicForm.controls.Zip;
        break;
      case "5digit":
        FieldArray = <FormArray>this.geographicForm.controls.Zip;
        break;
      case "3digitEx":
        FieldArray = <FormArray>this.geographicForm.controls.ExcludeZip;
        break;
      case "5digitEx":
        FieldArray = <FormArray>this.geographicForm.controls.ExcludeZip;
        break;
      case "meto":
        FieldArray = <FormArray>this.geographicForm.controls.MSA;
        break;
      case "metoEx":
        FieldArray = <FormArray>this.geographicForm.controls.ExcludeMSA;
        break;
      case "meto":
        FieldArray = <FormArray>this.geographicForm.controls.MSA;
        break;
      case "metoEx":
        FieldArray = <FormArray>this.geographicForm.controls.ExcludeMSA;
        break;
      case "area":
        FieldArray = <FormArray>this.geographicForm.controls.AreaCode;
        break;
      case "areaEx":
        FieldArray = <FormArray>this.geographicForm.controls.ExcludeAreaCode;
    }
    if (isPush) {
      FieldArray.push(new FormControl(ID));
    } else {
      let index = FieldArray.controls.findIndex(x => x.value == ID);
      FieldArray.removeAt(index);
    }
  }
  ////State Include Events End List////
  getStates() {

    for (let state of this.selectedItems) {
      //console.log(state.id);
      this.IncludeStates = this.IncludeStates.concat(state.id + ","); //state.id + ',';
    }
    this.IncludeStates = this.IncludeStates.replace(/,\s*$/, "");

    for (let state of this.selectedItemsExclude) {
      //console.log(state.id);
      this.ExcludedStates = this.ExcludedStates.concat(state.id + ","); //state.id + ',';
    }
    this.ExcludedStates = this.ExcludedStates.replace(/,\s*$/, "");
  }
  ////State Exclude Events////
  loadCounties() {
    this.getStates();

    if (this.IncludeStates == "" && this.ExcludedStates == "") {
      abp.message.info('Please select States from states section');

    }
    else {
      this._commonService
        .getCounties(this.IncludeStates, this.ExcludedStates, this.isAdSearch)
        .subscribe(result => {
          this.counties = result;
          this.counties = this.counties.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
          this.countiesExclude = this.counties;
        });
    }

    //console.log(IncludeStates);
  }
  loadZipCodes() {
    this.getStates();
    if (this.IncludeStates == "" && this.ExcludedStates == "") {
      abp.message.info('Please select States from states section');
    }
    else {
      this._commonService
        .getZipCodes(this.IncludeStates, this.ExcludedStates)
        .subscribe(result => {

          this.zip3digits = result.zip3Digits.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
          this.zip3digitsEx = result.zip3Digits.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
          this.zip5digits = result.zip5Digits.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
          this.zip5digitsEx = result.zip5Digits.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
        });
    }
  }
  loadMetroAreas() {
    this.getStates();
    this._commonService
      .getMetroAreas(this.IncludeStates, this.ExcludedStates)
      .subscribe(result => {

        this.metroAreas = result.map(item => {
          return {
            id: item.id,
            itemName: item.name
          };
        });
        this.metroAreasEx = result.map(item => {
          return {
            id: item.id,
            itemName: item.name
          };
        });
      });
  }

  loadAreaCodes() {
    this.getStates();
    this._commonService
      .getAreas(this.IncludeStates, this.ExcludedStates)
      .subscribe(result => {
        this.areaCodes = result.map(item => {
          return {
            id: item.id,
            itemName: item.name
          };
        });
        this.areaCodesEx = result.map(item => {
          return {
            id: item.id,
            itemName: item.name
          };
        });
      });
  }

  async editGeographic(myAdSearchDto: ADSearchDto) {

    if (myAdSearchDto.state != "" || myAdSearchDto.excludeState != "") {
      const StateArray = <FormArray>this.geographicForm.controls.State;
      for (let state of this.states) {
        if (myAdSearchDto.state.split(',').indexOf(state.id) > -1) {
          this.selectedItems.push(state);

          StateArray.push(new FormControl(state.id));
        }
      }
      const ExcludeStateArray = <FormArray>this.geographicForm.controls.ExcludeState;
      for (let state of this.states) {
        if (myAdSearchDto.excludeState.split(',').indexOf(state.id) > -1) {
          this.selectedItemsExclude.push(state);

          ExcludeStateArray.push(new FormControl(state.id));
        }
      }
      this.getStates();

    }
    ///For County///
    if (myAdSearchDto.county != "" || myAdSearchDto.countyIDs != "" || myAdSearchDto.excludeCountyIds != "" || myAdSearchDto.excludeCountiesList != "") {
      await this._commonService
        .getCounties(this.IncludeStates, this.ExcludedStates, this.isAdSearch)
        .toPromise().then(counties => {
          // this.counties = counties;
          this.counties = counties.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
          this.countiesExclude = this.counties;
        });


      if (myAdSearchDto.county != "") {
        const CountyArray = <FormArray>this.geographicForm.controls.County;
        for (let county of this.counties) {
          if (myAdSearchDto.county.split(',').indexOf(county.itemName.split(':')[0]) > -1) {
            CountyArray.push(new FormControl(county.itemName.split(':')[0]));
          }
        }
        const CountyIDsArray = <FormArray>this.geographicForm.controls.CountyIDs;
        for (let county of this.counties) {
          if (myAdSearchDto.countyIDs.split(',').indexOf(county.id) > -1) {
            this.selectedCounties.push(county);
            CountyIDsArray.push(new FormControl(county.id));
          }
        }
      }
      if (myAdSearchDto.excludeCountyIds != "") {
        const excludeCountyIdsArray = <FormArray>this.geographicForm.controls.ExcludeCountyIds;
        for (let county of this.countiesExclude) {
          if (myAdSearchDto.excludeCountyIds.split(',').indexOf(county.id) > -1) {
            this.selectedCountiesExclude.push(county);
            excludeCountyIdsArray.push(new FormControl(county.id));
          }
        }

        const ExcludeCountiesListArray = <FormArray>this.geographicForm.controls.ExcludeCountiesList;
        for (let county of this.countiesExclude) {
          if (myAdSearchDto.excludeCountiesList.split(',').indexOf(county.itemName.split(':')[0]) > -1) {
            ExcludeCountiesListArray.push(new FormControl(county.itemName.split(':')[0]));
          }
        }
      }
    }
    ///For Zip///

    if (myAdSearchDto.zip != "" || myAdSearchDto.excludeZip != "") {

      await this._commonService
        .getZipCodes(this.IncludeStates, this.ExcludedStates)
        .toPromise().then(result => {
          this.zip3digits = result.zip3Digits.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
          this.zip3digitsEx = result.zip3Digits.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
          this.zip5digits = result.zip5Digits.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
          this.zip5digitsEx = result.zip5Digits.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
        });

      if (myAdSearchDto.zip != "") {
        const ZipArray = <FormArray>this.geographicForm.controls.Zip;
        for (let zip of this.zip3digits) {
          if (myAdSearchDto.zip.split(',').indexOf(zip.id) > -1) {
            this.selectedZip3digits.push(zip.id);
            ZipArray.push(new FormControl(zip.id));
          }
        }
        for (let zip of this.zip5digits) {
          if (myAdSearchDto.zip.split(',').indexOf(zip.id) > -1) {
            this.selectedZip5digits.push(zip.id);
            ZipArray.push(new FormControl(zip.id));
          }
        }
      }
      if (myAdSearchDto.excludeZip != "") {
        const ExcludeZipArray = <FormArray>this.geographicForm.controls.ExcludeZip;
        for (let zip of this.zip3digitsEx) {
          if (myAdSearchDto.excludeZip.split(',').indexOf(zip.id) > -1) {
            this.selectedZip3digitsEx.push(zip.id);
            ExcludeZipArray.push(new FormControl(zip.id));
          }
        }
        for (let zip of this.zip5digitsEx) {
          if (myAdSearchDto.excludeZip.split(',').indexOf(zip.id) > -1) {
            this.selectedZip5digitsEx.push(zip.id);
            ExcludeZipArray.push(new FormControl(zip.id));
          }
        }
      }
    }
    if (myAdSearchDto.msa != "" || myAdSearchDto.excludeMSA != "") {
      await this._commonService
        .getMetroAreas(this.IncludeStates, this.ExcludedStates)
        .toPromise().then(result => {

          this.metroAreas = result.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
          this.metroAreasEx = result.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
        });
      if (myAdSearchDto.msa != "") {
        const MSAArray = <FormArray>this.geographicForm.controls.MSA;
        for (let metroArr of this.metroAreas) {
          if (myAdSearchDto.msa.split(',').indexOf(metroArr.id) > -1) {
            this.selectedMetroAreas.push(metroArr.id);
            MSAArray.push(new FormControl(metroArr.id));
          }
        }
        const MSAStringArray = <FormArray>this.geographicForm.controls.MSAString;
        for (let metroArr of this.metroAreas) {
          if (myAdSearchDto.msa.split(',').indexOf(metroArr.id) > -1) {
            MSAStringArray.push(new FormControl(metroArr.itemName));
          }
        }
      }
      if (myAdSearchDto.excludeMSA != "") {
        const ExcludeMSAArray = <FormArray>this.geographicForm.controls.ExcludeMSA;
        for (let metroArp of this.metroAreasEx) {
          if (myAdSearchDto.excludeMSA.split(',').indexOf(metroArp.id) > -1) {
            this.selectedMetroAreasEx.push(metroArp.id);
            ExcludeMSAArray.push(new FormControl(metroArp.id));
          }
        }
        const ExcludeMSAStringArray = <FormArray>this.geographicForm.controls.MSAStringEx;
        for (let metroArp of this.metroAreasEx) {
          if (myAdSearchDto.excludeMSA.split(',').indexOf(metroArp.id) > -1) {
            ExcludeMSAStringArray.push(new FormControl(metroArp.itemName));
          }
        }
      }
    }

    this.geographicForm.patchValue({
      Country: myAdSearchDto.country
    });
  }

  async editBdGeographic(bdSearchDto: BDSearchDto) {

    if (bdSearchDto.state != "" || bdSearchDto.exclude_State != "") {
      const StateArray = <FormArray>this.geographicForm.controls.State;
      for (let state of this.states) {
        if (bdSearchDto.state.split(',').indexOf(state.id) > -1) {
          this.selectedItems.push(state);

          StateArray.push(new FormControl(state.id));
        }
      }
      const ExcludeStateArray = <FormArray>this.geographicForm.controls.ExcludeState;
      for (let state of this.states) {
        if (bdSearchDto.exclude_State.split(',').indexOf(state.id) > -1) {
          this.selectedItemsExclude.push(state);

          ExcludeStateArray.push(new FormControl(state.id));
        }
      }
      this.getStates();

    }
    ///For County///
    if (bdSearchDto.county != "" || bdSearchDto.exclude_County != "") {
      await this._commonService
        .getCounties(this.IncludeStates, this.ExcludedStates, this.isAdSearch)
        .toPromise().then(counties => {
          // this.counties = counties;
          this.counties = counties.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
          this.countiesExclude = this.counties;
        });


      if (bdSearchDto.county != "") {
        const CountyArray = <FormArray>this.geographicForm.controls.County;
        for (let county of this.counties) {
          if (bdSearchDto.county.split(',').indexOf(county.itemName.split(':')[0]) > -1) {
            CountyArray.push(new FormControl(county.itemName.split(':')[0]));
          }
        }
      }
      if (bdSearchDto.exclude_County != "") {
        const excludeCountyIdsArray = <FormArray>this.geographicForm.controls.ExcludeCountyIds;
        for (let county of this.countiesExclude) {
          if (bdSearchDto.exclude_County.split(',').indexOf(county.id) > -1) {
            this.selectedCountiesExclude.push(county);
            excludeCountyIdsArray.push(new FormControl(county.id));
          }
        }
      }
    }
    ///For Zip///

    if (bdSearchDto.zip != "" || bdSearchDto.exclude_Zip != "") {

      await this._commonService
        .getZipCodes(this.IncludeStates, this.ExcludedStates)
        .toPromise().then(result => {
          this.zip3digits = result.zip3Digits.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
          this.zip3digitsEx = result.zip3Digits.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
          this.zip5digits = result.zip5Digits.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
          this.zip5digitsEx = result.zip5Digits.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
        });

      if (bdSearchDto.zip != "") {
        const ZipArray = <FormArray>this.geographicForm.controls.Zip;
        for (let zip of this.zip3digits) {
          if (bdSearchDto.zip.split(',').indexOf(zip.id) > -1) {
            this.selectedZip3digits.push(zip.id);
            ZipArray.push(new FormControl(zip.id));
          }
        }
        for (let zip of this.zip5digits) {
          if (bdSearchDto.zip.split(',').indexOf(zip.id) > -1) {
            this.selectedZip5digits.push(zip.id);
            ZipArray.push(new FormControl(zip.id));
          }
        }
      }
      if (bdSearchDto.exclude_Zip != "") {
        const ExcludeZipArray = <FormArray>this.geographicForm.controls.ExcludeZip;
        for (let zip of this.zip3digitsEx) {
          if (bdSearchDto.exclude_Zip.split(',').indexOf(zip.id) > -1) {
            this.selectedZip3digitsEx.push(zip.id);
            ExcludeZipArray.push(new FormControl(zip.id));
          }
        }
        for (let zip of this.zip5digitsEx) {
          if (bdSearchDto.exclude_Zip.split(',').indexOf(zip.id) > -1) {
            this.selectedZip5digitsEx.push(zip.id);
            ExcludeZipArray.push(new FormControl(zip.id));
          }
        }
      }
    }
    if (bdSearchDto.msa != "" || bdSearchDto.exclude_MSA != "") {
      await this._commonService
        .getMetroAreas(this.IncludeStates, this.ExcludedStates)
        .toPromise().then(result => {

          this.metroAreas = result.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
          this.metroAreasEx = result.map(item => {
            return {
              id: item.id,
              itemName: item.name
            };
          });
        });
      if (bdSearchDto.msa != "") {
        const MSAArray = <FormArray>this.geographicForm.controls.MSA;
        for (let metroArr of this.metroAreas) {
          if (bdSearchDto.msa.split(',').indexOf(metroArr.id) > -1) {
            this.selectedMetroAreas.push(metroArr.id);
            MSAArray.push(new FormControl(metroArr.id));
          }
        }
        const MSAStringArray = <FormArray>this.geographicForm.controls.MSAString;
        for (let metroArr of this.metroAreas) {
          if (bdSearchDto.msa.split(',').indexOf(metroArr.id) > -1) {
            MSAStringArray.push(new FormControl(metroArr.itemName));
          }
        }
      }
      if (bdSearchDto.exclude_MSA != "") {
        const ExcludeMSAArray = <FormArray>this.geographicForm.controls.ExcludeMSA;
        for (let metroArp of this.metroAreasEx) {
          if (bdSearchDto.exclude_MSA.split(',').indexOf(metroArp.id) > -1) {
            this.selectedMetroAreasEx.push(metroArp.id);
            ExcludeMSAArray.push(new FormControl(metroArp.id));
          }
        }
        const ExcludeMSAStringArray = <FormArray>this.geographicForm.controls.MSAStringEx;
        for (let metroArp of this.metroAreasEx) {
          if (bdSearchDto.exclude_MSA.split(',').indexOf(metroArp.id) > -1) {
            ExcludeMSAStringArray.push(new FormControl(metroArp.itemName));
          }
        }
      }
    }
  }

  reset(form: NgForm) {
    // this.populateFormIDs(item.itemName.split(":")[0], false, "countyText");
    console.log(this.counties);
    var item = this.counties;
    this.populateFormIDs(item.itemName.split(":")[0], false, "countyText");

  }
}
