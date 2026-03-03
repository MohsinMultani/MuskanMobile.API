// product.component.ts
import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';  // Import HttpClient

@Component({
  selector: 'app-product',  // The selector for the component
  standalone: true,  // Mark this as a standalone component
  template: `
    <h2>Product List</h2>
    <ul>
      <li *ngFor="let product of products">
        <strong>{{ product.name }}</strong><br />
        Price: {{ product.price }}<br />
        Category: {{ product.category }}<br />
        Description: {{ product.description }}
      </li>
    </ul>
  `,  // Inline template to display products
  styleUrls: ['./product.component.css'],  // Optional styling file
})
export class ProductComponent implements OnInit {
  products: any[] = [];  // Array to store products

  constructor(private http: HttpClient) {}  // Inject HttpClient to make HTTP calls

  ngOnInit(): void {
    // API call to get the product data
    this.http.get<any[]>('http://localhost:5000/api/products')  // Replace with your API endpoint
      .subscribe((data) => {
        this.products = data;  // Store the response in the products array
      });
  }
}
