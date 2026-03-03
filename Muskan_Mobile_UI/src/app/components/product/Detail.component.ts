import { Component } from '@angular/core'
import { ActivatedRoute } from '@angular/router'
import { ProductService } from './Product.service'

@Component({
  selector: 'product-detail',
  template: `
    <div class="container">
      <div class="row">
        <div class="col">
          <form ngNativeValidate method="post">
            <div class="row">
              <div class="mb-3 col-md-6 col-lg-4">
                <label class="form-label" for="product_id">Id</label>
                <input readonly id="product_id" name="id" class="form-control" value="{{product.id}}" type="number" required />
              </div>
              <div class="mb-3 col-md-6 col-lg-4">
                <label class="form-label" for="product_name">Name</label>
                <input readonly id="product_name" name="name" class="form-control" value="{{product.name}}" maxlength="50" />
              </div>
              <div class="mb-3 col-md-6 col-lg-4">
                <label class="form-label" for="product_price">Price</label>
                <input readonly id="product_price" name="price" class="form-control" value="{{product.price}}" type="number" />
              </div>
              <div class="col-12">
                <a class="btn btn-secondary" routerLink="/product">Back</a>
                <a class="btn btn-primary" routerLink="/product/edit/{{product.id}}">Edit</a>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>`
})
export class ProductDetail {
  
  product?: any = {}
  constructor(private route: ActivatedRoute, private ProductService: ProductService) { }
  
  ngOnInit() {
    this.get()
  }

  get() {
    return this.ProductService.get(this.route.snapshot.params['id']).subscribe(data => {
      this.product = data
    }, e => {
      alert(e.error)
    })
  }
}