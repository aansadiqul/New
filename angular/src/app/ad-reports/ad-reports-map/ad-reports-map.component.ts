import { Component, OnInit,ElementRef,ViewChild,Renderer2 } from '@angular/core';
import {IMarkerOptions,MarkerTypeId, IMapOptions, IBox, IMarkerIconInfo} from 'angular-maps';
import { ActivatedRoute, Router} from '@angular/router';
import {
  ADOrderServiceProxy
} from '@shared/service-proxies/service-proxies';
import { OrderStatus } from '@shared/AppEnums';
@Component({
  selector: 'app-ad-reports-map',
  templateUrl: './ad-reports-map.component.html',
  styleUrls: ['./ad-reports-map.component.css']
})
export class AdReportsMapComponent implements OnInit {
  id: number;
  private sub: any;
  mapMarkers: any = [];
  mapMarkersCopy: any = [];
  saving: boolean = false;
  infobox : any;
  //@ViewChild('xmap') BingMap: ElementRef;

  @ViewChild('myMap') myMap; 
  _count : number = 0;
  _currentpage : number = 0;
  _states : any = [];
  _counties : any = [];
  _types : any = [];

  selectedState: string = "";
  selectedCounty: string = "";
  selectedType: string = "";
  
  map:any;
  layer : any;
  isFilter : boolean = false;

  totalRecords : number = 0;

  constructor(private renderer: Renderer2,private route: ActivatedRoute,
    private router: Router,private _adOrders: ADOrderServiceProxy) {  }

  ngOnInit() {
    this.saving = true;
  }

  ngAfterViewInit(){  // after the view completes initializaion, create the map
    this.sub = this.route.params.subscribe(params => {
            this.id = +params['id']; // (+) converts string 'id' to a number            
            this._adOrders.getMapData(this.id).subscribe(result => {
            
            if(result.status == OrderStatus.Valid)
            {
              this.mapMarkers = JSON.parse(result.data); 
              this.mapMarkersCopy = this.mapMarkers;
              //console.log(this.mapMarkers);              
              this._count = this.mapMarkers.length;
              this.getMap();
              this.setDropDownValues();
            }
            else if(result.status == OrderStatus.Expired)
            {
              abp.notify.success('Your report has expired.');
              this.saving = false;
            }
            else if(result.status == OrderStatus.PurchaseRequired)
            {
              abp.notify.success('A report is not available for this order.');
              this.saving = false;
            }  
            else
            {
              abp.notify.success('Invalid Order Id');
              this.saving = false;
            }
      
      });  
      
    });
  }
  
  getMap(){
    this.map = new Microsoft.Maps.Map(this.myMap.nativeElement, {
      credentials: 'AjIwjztEZUGVsVHJ1FXu_Jxg3wsXmErEwQEikWO4-ekKvjr0lCEsWtO-pN7dE3b9',  
      center: new Microsoft.Maps.Location(39.774112, -101.510655),       
      zoom: 2, //breaks
    });        
     //Create an infobox at the center of the map but don't show it.
     this.infobox = new Microsoft.Maps.Infobox(this.map.getCenter(), {
      visible: false,
      maxHeight: 500
     });
    //Assign the infobox to a map instance.
    this.infobox.setMap(this.map);
    this.loadNewMap();
  }
  pushpinClicked(e) {
    //Make sure the infobox has metadata to display.
    if (e.target.metadata) {
        //Set the infobox options with the metadata of the pushpin.
        this.infobox.setOptions({
            location: e.target.getLocation(),
            title: e.target.metadata.title,
            description: e.target.metadata.description,
            actions: [{
                label: 'Detail',
                eventHandler: function () {
                  //location.href = '/app/ad-reports-accountDetail/'+ e.target.metadata.accountId;
                  window.open('/app/ad-reports-accountDetail/'+ e.target.metadata.accountId, '_blank');  
                }
            }, {
                label: 'Direction',
                eventHandler: function () {
                  //location.href = 'http://bing.com/maps/default.aspx?rtp=~adr.'+ e.target.metadata.Address1 +','+ e.target.metadata.City +','+ e.target.metadata.State +',' + e.target.metadata.PostCode;
                  window.open('http://bing.com/maps/default.aspx?rtp=~adr.'+ e.target.metadata.Address1 +','+ e.target.metadata.City +','+ e.target.metadata.State +',' + e.target.metadata.PostCode, '_blank');  
                }
            }],
            visible: true
        });
    }
 }
 setDropDownValues()
 {  
   this._states = Array.from(new Set(this.mapMarkers.map((item: any) => item.State)));
   this._counties = Array.from(new Set(this.mapMarkers.map((item: any) => item.County)));
   this._types = Array.from(new Set(this.mapMarkers.map((item: any) => item.Type)));
   
 }
 changeCounties(){
  this.mapMarkersCopy = this.mapMarkers.filter(object => {
		return object['State'] == this.selectedState;
  });
  this._counties = Array.from(new Set(this.mapMarkersCopy.map((item: any) => item.County)));
 }
 onChangeState(selectedState){
   this.selectedState = selectedState;
   this.changeCounties();
   this.selectedState != "" || this.selectedCounty != "" || this.selectedType!="" ? this.isFilter = true : this.isFilter = false;
 }
 onChangeCounty(selectedCounty){
  this.selectedCounty = selectedCounty;
  this.selectedState != "" || this.selectedCounty != "" || this.selectedType!="" ? this.isFilter = true : this.isFilter = false;
}
onChangeType(selectedType){
  this.selectedType = selectedType;
  this.selectedState != "" || this.selectedCounty != "" || this.selectedType!="" ? this.isFilter = true : this.isFilter = false;
}
filterData() {
  this.saving = true;
  
	this.mapMarkersCopy = this.mapMarkers.filter(object => {
    if(this.selectedState != "" && this.selectedCounty != "" && this.selectedType != "")
    {
        return object['State'] == this.selectedState && object['County'] == this.selectedCounty && object['Type'] == this.selectedType;
    }
    else if(this.selectedState != "" && this.selectedCounty)
    {
      return object['State'] == this.selectedState && object['County'] == this.selectedCounty;
    }
    else if(this.selectedState != "" && this.selectedType != "")
    {
      return object['State'] == this.selectedState && object['Type'] == this.selectedType;
    }
    else if(this.selectedCounty != "" && this.selectedType != "")
    {
      return object['County'] == this.selectedCounty && object['Type'] == this.selectedType;
    }
    else if(this.selectedState != "")
    {
      return object['State'] == this.selectedState;
    }
    else if(this.selectedCounty != "")
    {
      return object['County'] == this.selectedCounty;
    }
    else
    {
      return object['Type'] == this.selectedType;
    }
  });
  this.map.layers.remove(this.layer);
  this.loadNewMap();
}
clearFilter() {
  this.selectedState = "";
  this.selectedCounty = "";
  this.selectedType = "";
  this.saving = true;
  this.mapMarkersCopy = this.mapMarkers;
  this.map.layers.remove(this.layer);
  this.loadNewMap();
}
loadNewMap(){  
  var uniqueitems = Array.from(new Set(this.mapMarkersCopy.map((item: any) => item.Latitude && item.Longitude)));
  this.totalRecords = uniqueitems.length;

  this.layer = new Microsoft.Maps.Layer();        
    for (let i = this._currentpage; i < uniqueitems.length; i++) { 
      
      var latLon = new Microsoft.Maps.Location(this.mapMarkersCopy[i].Latitude, this.mapMarkersCopy[i].Longitude);
     // console.log(latLon);
      var pushpin = new Microsoft.Maps.Pushpin(latLon, {icon: '/assets/images/location.png'});     
      //Store some metadata with the pushpin.
      pushpin.metadata = {
        title: this.mapMarkersCopy[i].Account,
        description: this.mapMarkersCopy[i].Address1 + '<br>' + this.mapMarkersCopy[i].City + ',' + this.mapMarkersCopy[i].State + ' ' + this.mapMarkersCopy[i].PostCode + '<br>' + this.mapMarkersCopy[i].MainPhone + '<br>' + '<a  href=' + this.mapMarkersCopy[i].WebAddress + ' style="word-break: break-word;">' + this.mapMarkersCopy[i].WebAddress +'</a>' + '<br>' + this.mapMarkersCopy[i].FirstName + ' ' + this.mapMarkersCopy[i].LastName + '<br>' + 'Lat:' + this.mapMarkersCopy[i].Latitude + '<br>' + 'Long:' + this.mapMarkersCopy[i].Longitude,
        accountId:this.mapMarkersCopy[i].AccountId,
        Address1:this.mapMarkersCopy[i].Address1,
        City:this.mapMarkersCopy[i].City,
        State:this.mapMarkersCopy[i].State,
        PostCode:this.mapMarkersCopy[i].PostCode
      };     
      //Add a click event handler to the pushpin.
      Microsoft.Maps.Events.addHandler(pushpin, 'click', this.pushpinClicked.bind(this));
      this.layer.add(pushpin);      
         
    }
    
    this.map.layers.insert(this.layer);
    this.saving = false;
 }

}
