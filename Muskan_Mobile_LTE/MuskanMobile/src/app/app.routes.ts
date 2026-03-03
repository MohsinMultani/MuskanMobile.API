import { Component } from '@angular/core';
import { Routes } from '@angular/router';
import path from 'node:path';
import { LoginComponent } from './Components/login/login.component';
import { Z_FULL_FLUSH } from 'node:zlib';
import { LayoutComponent } from './Components/layout/layout.component';
import { DashboardComponent } from './Components/dashboard/dashboard.component';
import { CustomersComponent } from './Components/customers/customers.component';

export const routes: Routes = [
    {
        path: '',
        component: LoginComponent,
        pathMatch: 'full'
    },
    {
        path: 'Login',
        component: LoginComponent
    },
    {
        // path: '',
        // component: AdminComponent,
        // children: [
        //   { path: 'dashboard', component: DashboardComponent },
        //   { path: 'products', component: ProductsComponent },
        //   { path: '', redirectTo: 'dashboard', pathMatch: 'full' },



        path: '',
        component: LayoutComponent,
        children: [
            { path: 'Dashboard', component: DashboardComponent, title: 'Dashboard' },
            { path: 'Customers', component: CustomersComponent, title: 'Customers' },
            { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
        ]
    }

];
