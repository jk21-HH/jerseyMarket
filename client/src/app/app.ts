import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { ProductsList } from './products/products-list/products-list';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ProductsList],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {}
