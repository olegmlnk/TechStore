import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Product, ProductList } from '../models/product.model';

export interface CreateProductRequest {
  title: string;
  description: string;
  price: number;
  isAvailable: boolean;
  stockQuantity: number;
  category: number;
  imageUrl: string;
}

export interface UpdateProductRequest extends CreateProductRequest {}

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

  // ── Admin methods ──────────────────────────────────────

  getAllForAdmin() {
    return this.http.get<ProductList>(`${environment.apiUrl}/products/admin`);
  }

  create(product: CreateProductRequest) {
    return this.http.post<Product>(`${environment.apiUrl}/products`, product);
  }

  update(id: string, product: UpdateProductRequest) {
    return this.http.put<Product>(`${environment.apiUrl}/products/${id}`, product);
  }

  delete(id: string) {
    return this.http.delete(`${environment.apiUrl}/products/${id}`);
  }
}
