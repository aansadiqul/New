import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CommonService {
  _id: any;

  set setId(value: any) {
     this._id = value;
  }

  get getId(): any {
      return this._id;
  }

  constructor() {}

}
