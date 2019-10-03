import { BDSearchDto } from './../../../../shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { CommonServiceProxy, IndustryDto, ADSearchDto } from '@shared/service-proxies/service-proxies';
import { SelectionModel } from '@angular/cdk/collections';
import { FlatTreeControl } from '@angular/cdk/tree';
import { Component, Injectable, Injector, OnInit, Output, Input, EventEmitter } from '@angular/core';
import { MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material/tree';
import { BehaviorSubject } from 'rxjs';
import { FormGroup, FormBuilder, FormArray, FormControl } from '@angular/forms';
import * as XLSX from 'xlsx';

//import { Transform } from 'stream';

/** Flat to-do item node with expandable and level information */
export class TreeItemFlatNode {
  text: string;
  level: number;
  expandable: boolean;
  sic:string;
  sicid : string;
  code: string;
}


@Injectable()
export class ChecklistDatabase {
  dataChange = new BehaviorSubject<IndustryDto[]>([]);
  treeData: any[];
  get data(): IndustryDto[] { return this.dataChange.value; }


  constructor(private _commonService: CommonServiceProxy) {
    this.initialize();
  }

  async initialize() {
    await this._commonService.getIndustries()
      .toPromise().then(result => {
        const data = result;
        this.treeData = result;
        this.dataChange.next(data);

      });
  }
  resetFilter(){
    this._commonService.getIndustries()
      .subscribe(result => {
        const data = result;
        this.treeData=result;
        this.dataChange.next(data);
      });
  }

  async LoadForAdsearch(){
    await this._commonService.getIndustries()
    .toPromise().then(result => {
        const data = result;
        this.treeData=result;
        this.dataChange.next(data);
     });
  }

}
@Component({
  selector: 'app-industry',
  templateUrl: './industry.component.html',
  styleUrls: ['./industry.component.css'],
  animations: [appModuleAnimation()],
  providers: [ChecklistDatabase]
})
export class IndustryComponent extends AppComponentBase implements OnInit {

  /** Map from flat node to nested node. This helps us finding the nested node to be modified */
  flatNodeMap = new Map<TreeItemFlatNode, IndustryDto>();
  /** Map from nested node to flattened node. This helps us to keep the same object for selection */
  nestedNodeMap = new Map<IndustryDto, TreeItemFlatNode>();
  /** A selected parent node to be inserted */
  selectedParent: TreeItemFlatNode | null = null;
  /** The new item's name */
  newItemName = '';
  treeControl: FlatTreeControl<TreeItemFlatNode>;
  treeFlattener: MatTreeFlattener<IndustryDto, TreeItemFlatNode>;
  dataSource: MatTreeFlatDataSource<IndustryDto, TreeItemFlatNode>;
  /** The selection for checklist */
  checklistSelection = new SelectionModel<TreeItemFlatNode>(true /* multiple */);

  ////Form///
  //Child form Logic//
  @Output() formReady = new EventEmitter<FormGroup>()
  industryForm: FormGroup;
  searchString:string ="";
  selectedNodes: any =[];
  searchText : boolean =false;
  arrayBuffer:any;
  file:File;
  sicCodesArr: any = [];


  show:boolean = false;///Loader
  constructor(private fb: FormBuilder, private database: ChecklistDatabase, private _commonService: CommonServiceProxy, injector: Injector) {
    super(injector);
    this.treeFlattener = new MatTreeFlattener(this.transformer, this.getLevel,
      this.isExpandable, this.getChildren);
    this.treeControl = new FlatTreeControl<TreeItemFlatNode>(this.getLevel, this.isExpandable);
    this.dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);
  }
  ngOnInit() {
    this.getIndustries();
    this.industryForm = this.fb.group({
      SICCodes:new FormArray([]),
      SICIDS:new FormArray([]),
      sicNames:new FormArray([]),
    });
    this.formReady.emit(this.industryForm);
  }

 getIndustries(){
    this.database.dataChange.subscribe(data => {
      this.dataSource.data = data;
      this.show = !this.show;
  });
  }

  getLevel = (node: TreeItemFlatNode) => node.level;
  isExpandable = (node: TreeItemFlatNode) => node.expandable;
  getChildren = (node: IndustryDto): IndustryDto[] => node.children;
  hasChild = (_: number, _nodeData: TreeItemFlatNode) => _nodeData.expandable;
  /**
   * Transformer to convert nested node to flat node. Record the nodes in maps for later use.
   */
  transformer = (node: IndustryDto, level: number) => {
    const existingNode = this.nestedNodeMap.get(node);
    const flatNode = existingNode && existingNode.text === node.text
      ? existingNode
      : new TreeItemFlatNode();
    flatNode.text = node.text;
    flatNode.level = level;
    flatNode.code = node.code;
    flatNode.sicid = node.sicid;
    flatNode.sic = node.sic;

    flatNode.expandable = node.children && node.children.length > 0;
    this.flatNodeMap.set(flatNode, node);
    this.nestedNodeMap.set(node, flatNode);

    return flatNode;
  }

  /** Whether all the descendants of the node are selected. */
  descendantsAllSelected(node: TreeItemFlatNode): boolean {
    const descendants = this.treeControl.getDescendants(node);
    const descAllSelected = descendants.every(child =>
      this.checklistSelection.isSelected(child)
      //this.checklistSelection.isSelected(child) ? this.populateFormNodes(node,true): this.populateFormNodes(node,false);
    );
    return descAllSelected;
  }

  /** Whether part of the descendants are selected */
  descendantsPartiallySelected(node: TreeItemFlatNode): boolean {
    const descendants = this.treeControl.getDescendants(node);
    const result = descendants.some(child => this.checklistSelection.isSelected(child));

    return result && !this.descendantsAllSelected(node);
  }

  /** Toggle the to-do item selection. Select/deselect all the descendants node */
  todoItemSelectionToggle(node: TreeItemFlatNode): void {
    this.checklistSelection.toggle(node);
    const descendants = this.treeControl.getDescendants(node);


    this.checklistSelection.isSelected(node)
      ? this.checklistSelection.select(...descendants)
      : this.checklistSelection.deselect(...descendants);

     /* this.checklistSelection.isSelected(node)
      ? this.populateFormNodes(node,true)
      : this.populateFormNodes(node,false)*/


    // Force update for the parent
    descendants.every(child =>
      this.checklistSelection.isSelected(child)
    );
    this.checkAllParentsSelection(node);
    this.chkselection();
  }

  /** Toggle a leaf to-do item selection. Check all the parents to see if they changed */
  todoLeafItemSelectionToggle(node: TreeItemFlatNode): void {
    this.checklistSelection.toggle(node);
    this.checkAllParentsSelection(node);
  }

  /* Checks all the parents when a leaf node is selected/unselected */
  checkAllParentsSelection(node: TreeItemFlatNode): void {
    let parent: TreeItemFlatNode | null = this.getParentNode(node);
    while (parent) {
      this.checkRootNodeSelection(parent);
      parent = this.getParentNode(parent);
    }
  }

  /** Check root node checked state and change it accordingly */
  checkRootNodeSelection(node: TreeItemFlatNode): void {
    const nodeSelected = this.checklistSelection.isSelected(node);
    const descendants = this.treeControl.getDescendants(node);
    const descAllSelected = descendants.every(child =>
      this.checklistSelection.isSelected(child)
    );
    if (nodeSelected && !descAllSelected) {
      this.checklistSelection.deselect(node);
    } else if (!nodeSelected && descAllSelected) {
      this.checklistSelection.select(node);
    }
  }

  /* Get the parent node of a node */
  getParentNode(node: TreeItemFlatNode): TreeItemFlatNode | null {
    const currentLevel = this.getLevel(node);

    if (currentLevel < 1) {
      return null;
    }

    const startIndex = this.treeControl.dataNodes.indexOf(node) - 1;

    for (let i = startIndex; i >= 0; i--) {
      const currentNode = this.treeControl.dataNodes[i];

      if (this.getLevel(currentNode) < currentLevel) {
        return currentNode;
      }
    }
    return null;
  }
  async changeSearchText(filterText: string){
    filterText!="" ? this.searchText=true :  this.searchText=false;
    if(filterText=="")
    {
      this.show=true;
      await this.database.LoadForAdsearch();
      this.selectPreselectedNodes();
      this.show=false;
    }
    this.searchString=filterText;
  }

  recursiveNodeEliminator(tree: Array<IndustryDto>): boolean {
    for (let index = tree.length - 1; index >= 0; index--) {
      const node = tree[index];
      if (node.children) {
        const parentCanBeEliminated = this.recursiveNodeEliminator(node.children);
        if (parentCanBeEliminated) {
          if (node.text.toLocaleLowerCase().indexOf(this.searchString.toLocaleLowerCase()) === -1) {
            tree.splice(index, 1);
          }
        }
      } else {
        // Its a leaf node. No more branches.
        if (node.text.toLocaleLowerCase().indexOf(this.searchString.toLocaleLowerCase()) === -1) {
          tree.splice(index, 1);
        }
      }
    }
    return tree.length === 0;
  }

  incomingfile(event)
  {

    this.file = event.target.files[0];
    let fileReader = new FileReader();
        fileReader.onload = (e) => {
            this.arrayBuffer = fileReader.result;
            var data = new Uint8Array(this.arrayBuffer);
            var arr = new Array();
            for(var i = 0; i != data.length; ++i) arr[i] = String.fromCharCode(data[i]);
            var bstr = arr.join("");
            var workbook = XLSX.read(bstr, {type:"binary"});
            var first_sheet_name = workbook.SheetNames[0];
            var worksheet = workbook.Sheets[first_sheet_name];
            var sheetJson=XLSX.utils.sheet_to_json(worksheet,{raw:true});
           // console.log(sheetJson);
            for(let jval of sheetJson)
            {
              this.sicCodesArr.push(jval[Object.keys(jval)[0]]);
            }
            this.selectExcelNodes();

            //console.log(XLSX.utils.sheet_to_json(worksheet,{raw:true}));
    }
    fileReader.readAsArrayBuffer(this.file);
    //console.log('last');
  }
  filterChanged() {
    this.show = true;

    //this.sicCodesArr = myAdSearchDto.sicCodes.split(',');
    const clonedTreeLocal = this.dataSource.data;
    this.recursiveNodeEliminator(clonedTreeLocal);
    this.dataSource.data = clonedTreeLocal;
    //to load preselected nodes
    this.selectPreselectedNodes();

    this.treeControl.expandAll();
    //this.show = false;
    setTimeout(()=>{    //<<<---    using ()=> syntax
      this.show = false;
     }, 3000);
  }
  clearfilter(){
    this.show = true;
    this.searchString="";
    this.database.resetFilter();
    /*setTimeout(()=>{
      this.show = false;
     }, 3000);*/
  }
  populateFormNodes(node:TreeItemFlatNode,isChecked:boolean=false)
  {

    const SICIDSArr = <FormArray>this.industryForm.controls.SICIDS;
    if(isChecked) {
      SICIDSArr.push(new FormControl(node.sicid));
    } else {
      let index = SICIDSArr.controls.findIndex(x => x.value == node.sicid);
      SICIDSArr.removeAt(index);
    }

    const sicArr = <FormArray>this.industryForm.controls.SICCodes;
    if(isChecked) {
      sicArr.push(new FormControl(node.sic));
    } else {
      let index = sicArr.controls.findIndex(x => x.value == node.sic);
      sicArr.removeAt(index);
    }

    const sicNamesArr = <FormArray>this.industryForm.controls.sicNames;
    if(isChecked) {
      sicNamesArr.push(new FormControl(node.text));
    } else {
      let index = sicNamesArr.controls.findIndex(x => x.value == node.text);
      sicNamesArr.removeAt(index);
    }
  }
  chkselection(){
    const arr = <FormArray>this.industryForm.controls.SICCodes;
    arr.controls = [];
    arr.removeAt(0)
    const arr2 = <FormArray>this.industryForm.controls.SICIDS;
    arr2.controls = [];
    arr2.removeAt(0)
    const arr3= <FormArray>this.industryForm.controls.sicNames;
    arr3.controls = [];
    arr3.removeAt(0)
   //this.treeControl.dataNodes

    for(let checkedItem of this.checklistSelection.selected){
      let parent: TreeItemFlatNode | null = this.getParentNode(checkedItem);
      /*if (node.text.toLocaleLowerCase().indexOf(this.searchString.toLocaleLowerCase()) === -1) {
        tree.splice(index, 1);
      }*/
      if(this.checklistSelection.isSelected(checkedItem) && !this.checklistSelection.isSelected(parent))
      {
        this.populateFormNodes(checkedItem,true);
      }
    }
  }

  selectExcelNodes(){
    //console.log("Sic Codes");
   // console.log(this.sicCodesArr);
    this.show = true;
    const arr = <FormArray>this.industryForm.controls.SICCodes;
    arr.controls = [];
    arr.removeAt(0)
    const arr2 = <FormArray>this.industryForm.controls.SICIDS;
    arr2.controls = [];
    arr2.removeAt(0)
    const arr3= <FormArray>this.industryForm.controls.sicNames;
    arr3.controls = [];
    arr3.removeAt(0)


    for(let checkedItem of this.treeControl.dataNodes){

      if(this.sicCodesArr.indexOf(checkedItem.sic) != -1)
      {
        //console.log(checkedItem);
        if(!this.checklistSelection.isSelected(checkedItem))
        {
          this.todoItemSelectionToggle(checkedItem);
          this.treeControl.expand(checkedItem);
        }
      }
    }
    this.show = false;
    //this.treeControl.expandAll();
  }
  selectPreselectedNodes(){


    const Sicarr=<FormArray>this.industryForm.controls.SICCodes;
    this.sicCodesArr = Sicarr.value;
    console.log(this.sicCodesArr);
    for(let checkedItem of this.treeControl.dataNodes){

      if(this.sicCodesArr.indexOf(checkedItem.sic) != -1)
      {
        if(!this.checklistSelection.isSelected(checkedItem))
        {
          this.todoItemSelectionToggle(checkedItem);
          this.treeControl.expand(checkedItem);
        }
      }
    }

    //this.treeControl.expandAll();
  }
  async editIndustryAdSearch(myAdSearchDto: ADSearchDto){
    await this.database.LoadForAdsearch();
    this.sicCodesArr = myAdSearchDto.sicCodes.split(',');
    this.selectExcelNodes();
  }

  async editIndustryBdSearch(bdSearchDto: BDSearchDto){
    await this.database.LoadForAdsearch();
    this.sicCodesArr = bdSearchDto.sic.split(',');
    this.selectExcelNodes();
  }
}
