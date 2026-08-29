import { Routes } from '@angular/router';

import { authGuard } from './auth/auth-guard';
import { Login } from './auth/login/login';
import { Register } from './auth/register/register';
import { ProductForm } from './products/product-form/product-form';
import { ProductsList } from './products/products-list/products-list';

export const routes: Routes = [
  { path: '', component: ProductsList },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'products/new', component: ProductForm, canActivate: [authGuard] },
  { path: 'products/:id/edit', component: ProductForm, canActivate: [authGuard] },
];
