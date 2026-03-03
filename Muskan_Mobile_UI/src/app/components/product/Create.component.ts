import { Component } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ProductService } from './Product.service'

@Component({
  selector: 'product-create',
  template: `
    <div class="container">
      <div class="row">
        <div class="col">
          <form ngNativeValidate method="post" (submit)="create()">
            <div class="row">
              <div class="mb-3 col-md-6 col-lg-4">
                <label class="form-label" for="product_name">Name</label>
                <input id="product_name" name="name" class="form-control" [(ngModel)]="product.name" maxlength="50" />
                <span *ngIf="errors.name" class="text-danger">{{errors.name}}</span>
              </div>
              <div class="mb-3 col-md-6 col-lg-4">
                <label class="form-label" for="product_price">description</label>
                <input id="product_description" name="description" class="form-control" [(ngModel)]="product.description" />
                <span *ngIf="errors.price" class="text-danger">{{errors.price}}</span>
              </div>
              <div class="mb-3 col-md-6 col-lg-4">
                <label class="form-label" for="product_price">Price</label>
                <input id="product_price" name="price" class="form-control" [(ngModel)]="product.price" type="number" />
                <span *ngIf="errors.price" class="text-danger">{{errors.price}}</span>
              </div>
              <div class="mb-3 col-md-6 col-lg-4">
                <label class="form-label" for="product_categoryId">categoryId</label>
                <input id="product_categoryID" name="categoryId" class="form-control" [(ngModel)]="product.categoryId" type="number" />
                <span *ngIf="errors.price" class="text-danger">{{errors.price}}</span>
              </div>
              <div class="col-12">
                <a class="btn btn-secondary" routerLink="/product">Cancel</a>
                <button class="btn btn-primary">Submit</button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>`
})
export class ProductCreate {
  
  product?: any = {}
  errors?: any = {}
  constructor(private router: Router, private route: ActivatedRoute, private ProductService: ProductService) { }

//   create() {
//     this.ProductService.create(this.product).subscribe(() => {
//       this.router.navigateByUrl('/Product')
//     }, (e) => {
//       alert(e.error)
//     })
//   }
create() {
    this.ProductService.create(this.product).subscribe({
      next: () => {
        this.router.navigateByUrl('/product');
      },
      error: (e) => {
        alert(e.error?.message || 'An error occurred while creating the product.');
      }
    });
  }
}