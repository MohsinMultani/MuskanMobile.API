import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';  // Import HttpClientModule
import { AppComponent } from './app.component';  // Import AppComponent
import { ProductComponent } from './product/product.component';  // Import ProductComponent

@NgModule({
  declarations: [
    AppComponent,
    ProductComponent  // Declare ProductComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule  // Add HttpClientModule here
  ],
  providers: [],
  bootstrap: [AppComponent]  // Bootstrap the root component
})
export class AppModule { }
