// import { Component } from '@angular/core';
// import { RouterOutlet } from '@angular/router';

// @Component({
//   selector: 'app-root',
//   standalone: true,
//   imports: [RouterOutlet],
//   templateUrl: './app.component.html',
//   styleUrl: './app.component.css'
// })
// export class AppComponent {
//   title = 'muskan-mobile-webui';
// }
// =================
import { Component } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { ProductListComponent } from './components/product-list/product-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [HttpClientModule, ProductListComponent],
  template: `<app-product-list></app-product-list>`,
})
export class AppComponent { }

bootstrapApplication(AppComponent);

// import { HttpClientModule } from '@angular/common/http';
// import { HttpClient } from '@angular/common/http';
// import { Injectable, Component } from '@angular/core';

// @Component({
//   selector: 'app-product',
//   standalone: true,  // This makes the component standalone
//   imports: [HttpClientModule],  // Add HttpClientModule here
//   templateUrl: './product-list.component.html',
//   styleUrls: ['./product.component.css'],
// })
// export class ProductComponent {
//   constructor(private http: HttpClient) {
//     this.getProducts();
//   }

//   getProducts() {
//     this.http.get('http://localhost:7269/api/products').subscribe(data => {
//       console.log(data);
//     });
//   }
// }
