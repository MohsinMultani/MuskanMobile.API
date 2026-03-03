// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-product',
//   standalone: true,
//   imports: [],
//   templateUrl: './product.component.html',
//   styleUrl: './product.component.css'
// })
// export class ProductComponent {

// }

import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';  // Import HttpClient
import { Observable } from 'rxjs';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css']
})
export class ProductComponent implements OnInit {

  products: any[] = [];  // Array to hold products

  constructor(private http: HttpClient) { }  // Inject HttpClient service

  ngOnInit(): void {
    // Replace with your API URL
    this.http.get<any[]>('http://localhost:5000/api/products')
      .subscribe(
        (data) => {
          this.products = data;  // Assign the response to the products array
        },
        (error) => {
          console.error('Error fetching products:', error);
        }
      );
  }
}

