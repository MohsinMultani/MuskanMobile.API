import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { environment } from '../../environments/environment.development';


@Injectable({
  providedIn: 'root'
})
export class ProductService {

  url:string = environment.apiBaseUrl + '/SaleMaster/GetUserList'
  constructor(private http:HttpClient) { }

  refreshList()
  {
    this.http.get(this.url)
    .subscribe({
      next: res =>{
        console.log(res);
      },
      error: err =>{
        console.log(err);
      }
    })
  }
}
