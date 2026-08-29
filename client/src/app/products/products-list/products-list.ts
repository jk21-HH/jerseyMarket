import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { Product } from '../product.model';
import { Products } from '../products';

@Component({
  selector: 'app-products-list',
  imports: [FormsModule],
  templateUrl: './products-list.html',
  styleUrl: './products-list.css',
})
export class ProductsList {
  private readonly productsService = inject(Products);

  protected readonly products = signal<Product[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  protected productNameFilter = '';
  protected categoryNameFilter = '';

  constructor() {
    this.search();
  }

  protected search(): void {
    this.loading.set(true);
    this.error.set(null);

    this.productsService.getAll(this.productNameFilter, this.categoryNameFilter).subscribe({
      next: (products) => {
        this.products.set(products);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.message);
        this.loading.set(false);
      },
    });
  }
}
