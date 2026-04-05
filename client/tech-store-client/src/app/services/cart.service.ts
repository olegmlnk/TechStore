import { Injectable, inject, signal, computed, effect } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { CartResponse, AddToCartRequest } from '../models/cart.model';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class CartService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  cart = signal<CartResponse | null>(null);
  loading = signal(false);
  itemCount = computed(() => {
    const currentCart = this.cart();
    if (!currentCart) return 0;
    return currentCart.items.reduce((sum, item) => sum + item.quantity, 0);
  });

  // Sidebar visibility UI state
  isCartOpen = signal(false);

  constructor() {
    // Automatically load cart when user logs in, or clear when logs out
    effect(() => {
      const loggedIn = this.authService.isLoggedIn();
      if (loggedIn) {
        this.loadCart();
      } else {
        this.cart.set(null);
        this.isCartOpen.set(false);
      }
    });
  }

  toggleCart() {
    if (this.authService.isLoggedIn()) {
      this.isCartOpen.update(v => !v);
    }
  }

  closeCart() {
    this.isCartOpen.set(false);
  }

  loadCart() {
    this.loading.set(true);
    this.http.get<CartResponse>(`${environment.apiUrl}/carts/my-cart`).subscribe({
      next: (res) => {
        this.cart.set(res);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  addToCart(req: AddToCartRequest) {
    this.loading.set(true);
    return this.http.post<CartResponse>(`${environment.apiUrl}/carts/items`, req).subscribe({
      next: (res) => {
        this.cart.set(res);
        this.loading.set(false);
        this.isCartOpen.set(true);
      },
      error: (err) => {
        console.error('Failed to add to cart:', err);
        this.loading.set(false);
      }
    });
  }

  updateQuantity(itemId: string, quantity: number) {
    this.loading.set(true);
    return this.http.put<CartResponse>(`${environment.apiUrl}/carts/items/${itemId}`, { quantity }).subscribe({
      next: (res) => {
        this.cart.set(res);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to update quantity:', err);
        this.loading.set(false);
      }
    });
  }

  removeItem(itemId: string) {
    this.loading.set(true);
    return this.http.delete<CartResponse>(`${environment.apiUrl}/carts/items/${itemId}`).subscribe({
      next: (res) => {
        this.cart.set(res);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to remove item:', err);
        this.loading.set(false);
      }
    });
  }

  clearCart() {
    this.loading.set(true);
    return this.http.delete<CartResponse>(`${environment.apiUrl}/carts/my-cart`).subscribe({
      next: (res) => {
        this.cart.set(res);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to clear cart:', err);
        this.loading.set(false);
      }
    });
  }
}
