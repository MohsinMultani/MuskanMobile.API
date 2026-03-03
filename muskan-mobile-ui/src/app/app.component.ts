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
//   title = 'muskan-mobile-ui';
// }


//========
// import { Component } from '@angular/core';
// import { CommonModule } from '@angular/common';  // Import CommonModule for standalone components

// @Component({
//   selector: 'app-root',
//   standalone: true,  // Mark this component as standalone
//   templateUrl: './app.component.html',
//   styleUrls: ['./app.component.css'],
//   imports: [CommonModule]  // Import CommonModule for common Angular directives like *ngIf, *ngFor
// })
// export class AppComponent {
//   title = 'muskan-mobile-ui';  // You can change the title as per your project
// }

import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PaymentDetailsComponent } from "./product/product.component";
// import { PaymentDetailFormComponent } from "./payment-details/payment-detail-form/payment-detail-form.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, PaymentDetailsComponent],
  templateUrl: './app.component.html',
  styles: [],
})
export class AppComponent {
  title = 'ShethAutoCunsult';
}
