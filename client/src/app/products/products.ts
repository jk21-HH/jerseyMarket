import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { Product } from './product.model';

@Injectable({
  providedIn: 'root',
})
export class Products {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/Products';

  getAll(productName?: string, categoryName?: string): Observable<Product[]> {
    let params = new HttpParams();
    if (productName) {
      params = params.set('productName', productName);
    }
    if (categoryName) {
      params = params.set('categoryName', categoryName);
    }

    return this.http.get<Product[]>(this.baseUrl, { params });
  }
}
