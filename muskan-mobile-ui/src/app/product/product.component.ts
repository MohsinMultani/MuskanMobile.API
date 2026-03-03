// // import { Component } from '@angular/core';

// // @Component({
// //   selector: 'app-product',
// //   standalone: true,
// //   imports: [],
// //   templateUrl: './product.component.html',
// //   styleUrl: './product.component.css'
// // })
// // export class ProductComponent {

// // }

// import { Component, OnInit } from '@angular/core';
// import { HttpClient } from '@angular/common/http';

// @Component({
//   selector: 'app-product',
//   templateUrl: './product.component.html',
//   styleUrls: ['./product.component.css']
// })
// export class ProductComponent implements OnInit {

//   products: any[] = [];  // Array to hold the products

//   constructor(private http: HttpClient) {}

//   ngOnInit(): void {
//     // Replace with your API URL
//     this.http.get<any[]>('http://localhost:5000/api/products')
//       .subscribe(
//         (data) => {
//           this.products = data;  // Assign fetched data to products
//         },
//         (error) => {
//           console.error('Error fetching products:', error);  // Handle error
//         }
//       );
//   }
// }

// import { Component } from '@angular/core';
// import { CommonModule } from '@angular/common';
// import { HttpClient } from '@angular/common/http';

// @Component({
//   selector: 'app-product',
//   standalone: true,  // Mark this component as standalone
//   templateUrl: './product.component.html',
//   styleUrls: ['./product.component.css'],
//   imports: [CommonModule]  // Import CommonModule
// })
// export class ProductComponent {
//   products: any[] = [];

//   constructor(private http: HttpClient) {}

//   ngOnInit(): void {
//     this.http.get<any[]>('http://localhost:5000/api/products')
//       .subscribe(
//         data => this.products = data,
//         error => console.error('Error fetching products:', error)
//       );
//   }
// }


import { Component,OnInit } from '@angular/core';
import { PaymentDetailFormprComponent } from "./product.component";
import { PaymentDetailService } from '../shared/payment-detail.service';

@Component({
  selector: 'app-payment-details',
  standalone: true,
  imports: [],
  templateUrl: './payment-details.component.html',
  styles: ``
})
export class PaymentDetailsComponent implements OnInit {

  constructor(public service: PaymentDetailService){

  }
  ngOnInit(): void {
    this.service.refreshList();
  }
}