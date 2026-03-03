// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-products',
//   template: `
//     <p>
//       products works!
//     </p>
//   `,
//   styles: ``
// })
// export class ProductsComponent {

// }
// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-products',
//   templateUrl: './products.component.html',
//   styleUrls: ['./products.component.css']
// })
// export class ProductsComponent {}

import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {
  products: Product[] = [];

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.productService.getAll().subscribe(data => this.products = data);
  }
}
