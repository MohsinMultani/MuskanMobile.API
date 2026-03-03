import { Component } from '@angular/core';
import { ProductService } from '../../../services/product.service';
import { Product } from '../../../models/product.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-product',
  templateUrl: './create-product.component.html',
  styleUrls: ['./create-product.component.css']
})
export class CreateProductComponent {
  product: Product = {
    id: 0,
    name: '',
    description: '',
    price: 0,
    categoryId: 0
  };

  constructor(private productService: ProductService, private router: Router) {}

  create() {
    this.productService.create(this.product).subscribe(() => {
      this.router.navigate(['/products']);
    });
  }
}
