// import { NgModule } from '@angular/core';
// import { RouterModule, Routes } from '@angular/router';

// const routes: Routes = [];

// @NgModule({
//   imports: [RouterModule.forRoot(routes)],
//   exports: [RouterModule]
// })
// export class AppRoutingModule { }


import { NgModule } from '@angular/core'
import { RouterModule, Routes } from '@angular/router'
import { ProductIndex } from './components/product/Index.component'
import { ProductCreate } from './components/product/Create.component'
import { ProductDetail } from './components/product/Detail.component'
import { ProductEdit } from './components/product/Edit.component'
import { ProductDelete } from './components/product/Delete.component'
import { AdminComponent } from './layout/admin/admin.component'
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { ProductsComponent } from './pages/products/products.component';
import { CreateProductComponent } from './pages/products/create-product/create-product.component';
import { EditProductComponent } from './pages/products/edit-product/edit-product.component'


const routes: Routes = [
  {
    path: '',
    component: AdminComponent,
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'products', component: ProductsComponent },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }

      // { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      // { path: 'dashboard', loadChildren: () => import('./pages/dashboard/dashboard.module').then(m => m.DashboardModule) },
      // { path: 'products', loadChildren: () => import('./pages/products/products.module').then(m => m.ProductsModule) },
    ]
  },
  { path: '', redirectTo: 'product', pathMatch: 'full' },
  { path: 'product', component: ProductIndex },
  { path: 'product/create', component: ProductCreate },
  { path: 'product/:id', component: ProductDetail },
  { path: 'product/edit/:id', component: ProductEdit },
  { path: 'product/delete/:id', component: ProductDelete },
  { path: 'dashboard', loadChildren: () => import('./pages/dashboard/dashboard.module').then(m => m.DashboardModule) },
  { path: 'products', loadChildren: () => import('./pages/products/products.module').then(m => m.ProductsModule) },
  { path: 'products/create', component: CreateProductComponent },
  { path: 'products/edit/:id', component: EditProductComponent },
]

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }