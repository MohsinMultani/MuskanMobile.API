// main.ts
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { ProductComponent } from './app/product.component';  // Import the ProductComponent

platformBrowserDynamic().bootstrapStandalone(ProductComponent)  // Bootstrap the standalone component
  .catch(err => console.error(err));  // Catch and log any errors
