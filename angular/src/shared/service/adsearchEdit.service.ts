import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ADsearchEdit {
  _saveSearchId: any;

  set saveSearchId(value: any) {
     this._saveSearchId = value;
  }

  get saveSearchId(): any {
      return this._saveSearchId;
  }

  constructor() {}

}
