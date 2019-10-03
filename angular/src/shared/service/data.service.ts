import { Injectable } from '@angular/core';

@Injectable()
export class DataService {

  private data = {};

 setData(key, value) {
    this.data[key] = value;
  }

  getData() {
    return this.data;
  }
}
