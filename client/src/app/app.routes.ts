import { Routes } from '@angular/router';

import { Login } from './auth/login/login';
import { Register } from './auth/register/register';
import { ProductsList } from './products/products-list/products-list';

export const routes: Routes = [
  { path: '', component: ProductsList },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
];
