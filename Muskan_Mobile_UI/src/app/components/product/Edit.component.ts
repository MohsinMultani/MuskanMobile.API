import { Component } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ProductService } from './Product.service'

@Component({
  selector: 'product-edit',
  template: `
    <div class="container">
      <div class="row">
        <div class="col">
          <form ngNativeValidate method="post" (submit)="edit()">
            <div class="row">
              <div class="mb-3 col-md-6 col-lg-4">
                <label class="form-label" for="product_id">Id</label>
                <input readonly id="product_id" name="id" class="form-control" [(ngModel)]="product.id" type="number" required />
                <span *ngIf="errors.id" class="text-danger">{{errors.id}}</span>
              </div>
              <div class="mb-3 col-md-6 col-lg-4">
                <label class="form-label" for="product_name">Name</label>
                <input id="product_name" name="name" class="form-control" [(ngModel)]="product.name" maxlength="50" />
                <span *ngIf="errors.name" class="text-danger">{{errors.name}}</span>
              </div>
              <div class="mb-3 col-md-6 col-lg-4">
                <label class="form-label" for="product_price">Price</label>
                <input id="product_price" name="price" class="form-control" [(ngModel)]="product.price" type="number" />
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
export class ProductEdit {
  
  product?: any = {}
  errors?: any = {}
  constructor(private router: Router, private route: ActivatedRoute, private ProductService: ProductService) { }
  
  ngOnInit() {
    this.get()
  }

  get() {
    return this.ProductService.edit(this.route.snapshot.params['id']).subscribe(data => {
      this.product = data
    }, e => {
      alert(e.error)
    })
  }

  edit() {
    this.ProductService.edit(this.route.snapshot.params['id'], this.product).subscribe(() => {
      this.router.navigateByUrl('/product')
    }, (e) => {
      alert(e.error)
    })
  }
}