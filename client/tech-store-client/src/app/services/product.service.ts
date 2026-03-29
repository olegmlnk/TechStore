import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Product, ProductList } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private http = inject(HttpClient);

  getAll() {
    return this.http.get<ProductList>(`${environment.apiUrl}/products`);
  }

  getById(id: string) {
    return this.http.get<Product>(`${environment.apiUrl}/products/${id}`);
  }

  getByCategory(category: number) {
    return this.http.get<ProductList>(`${environment.apiUrl}/products/category/${category}`);
  }

  search(query: string) {
    return this.http.get<ProductList>(`${environment.apiUrl}/products/search?q=${encodeURIComponent(query)}`);
  }
}
