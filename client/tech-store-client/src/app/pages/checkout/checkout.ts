import { Component, OnInit, inject, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { PLATFORM_ID } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AnalyticsService } from '../../services/analytics.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './checkout.html',
  styleUrl: './checkout.scss',
})
export class CheckoutPage implements OnInit {
  private router = inject(Router);
  private cartService = inject(CartService);
  private analytics = inject(AnalyticsService);
  private platformId = inject(PLATFORM_ID);

  fullName = signal('');
  email = signal('');
  address = signal('');
  paymentMethod = signal<'card' | 'cash'>('card');
  submitting = signal(false);

  ngOnInit(): void {
    const cart = this.cartService.cart();
    if (!cart || cart.items.length === 0) {
      this.router.navigate(['/']);
    }
  }

  cartTotal(): number {
    return this.cartService.cart()?.totalPrice ?? 0;
  }

  itemsCount(): number {
    return this.cartService.cart()?.items.length ?? 0;
  }

  submit(form: NgForm): void {
    if (!form.valid || this.submitting()) return;
    this.submitting.set(true);

    const cart = this.cartService.cart();
    const items = cart?.items ?? [];
    const total = cart?.totalPrice ?? 0;
    const orderId = this.generateOrderId();

    this.analytics.capture('purchase_completed', {
      order_id: orderId,
      total_value: total,
      items_count: items.length,
      payment_method: this.paymentMethod(),
    });

    this.cartService.clearCart();
    this.router.navigate(['/order-confirmation', orderId]);
  }

  private generateOrderId(): string {
    if (isPlatformBrowser(this.platformId) && typeof crypto !== 'undefined' && crypto.randomUUID) {
      return crypto.randomUUID();
    }
    return `order-${Date.now()}-${Math.random().toString(36).slice(2, 10)}`;
  }
}
