import { Component } from '@angular/core'
import { Router, NavigationEnd } from '@angular/router'
import { ProductService } from './Product.service'

@Component({
  selector: 'product-index',
  template: `
    <div class="container">
      <div class="row">
        <div class="col">
          <table class="table table-striped table-hover">
            <thead>
              <tr>
                <th>Id</th>
                <th>Name</th>
                <th>Description</th>
                <th>Price</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let product of products">
                <td class="text-center">{{product.id}}</td>
                <td>{{product.name}}</td>
                <td>{{product.Description}}</td>
                <td class="text-center">{{product.price}}</td>
                <td class="text-center">
                  <a class="btn btn-secondary" routerLink="/product/{{product.id}}" title="View"><i class="fa fa-eye"></i></a>
                  <a class="btn btn-primary" routerLink="/product/edit/{{product.id}}" title="Edit"><i class="fa fa-pencil"></i></a>
                  <a class="btn btn-danger" routerLink="/product/delete/{{product.id}}" title="Delete"><i class="fa fa-times"></i></a>
                </td>
              </tr>
            </tbody>
          </table>
          <a class="btn btn-primary" routerLink="/product/create">Create</a>
        </div>
      </div>
    </div>`
})

export class ProductIndex {

  products?: any[]
  constructor(public router: Router, private ProductService: ProductService) { }

  ngOnInit() {
    this.get()
  }
  
  get() {

    this.ProductService.get().subscribe({
        next: data => {
          this.products = data;
          console.log(data);
        },
        error: e => {
          alert("Errors: " + (e.error?.message || JSON.stringify(e)));
        },
        complete: () => {
          console.info("Product retrieval completed.");
        }
      });

     console.log(this.products)
    // this.ProductService.get().subscribe({complete:console.info })
    // this.ProductService.get().subscribe(data => {
    //   this.products = data
    // }, e => {
    //   alert("eroors "+e.error)
    // })
  }
}