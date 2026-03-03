// // import { bootstrapApplication } from '@angular/platform-browser';
// // import { appConfig } from './app/app.config';
// // import { AppComponent } from './app/app.component';

// // bootstrapApplication(AppComponent, appConfig)
// //   .catch((err) => console.error(err));

// import { AppComponent } from './app/app.component';
// import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

// platformBrowserDynamic().bootstrapStandalone(AppComponent)
//   .catch(err => console.error(err));
// import { AppComponent } from './app/app.component';
// import { ProductComponent } from './app/product/product.component';
// import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

// platformBrowserDynamic().bootstrapStandalone(AppComponent, {
//   providers: []  // You can add any providers here if needed
// })
//   .catch(err => console.error(err));
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
