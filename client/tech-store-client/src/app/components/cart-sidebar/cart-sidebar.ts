import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AnalyticsService } from '../../services/analytics.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-cart-sidebar',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './cart-sidebar.html',
  styleUrl: './cart-sidebar.scss'
})
export class CartSidebarComponent implements OnInit {
  cartService = inject(CartService);
  private analytics = inject(AnalyticsService);
  private router = inject(Router);

  showNewCta = signal(false);

  ngOnInit(): void {
    this.analytics.onFeatureFlagsLoaded(() => {
      this.showNewCta.set(this.analytics.isFeatureEnabled('new-checkout-cta'));
    });
  }

  increaseQty(itemId: string, currentQty: number) {
    this.cartService.updateQuantity(itemId, currentQty + 1);
  }

  decreaseQty(itemId: string, currentQty: number) {
    if (currentQty > 1) {
      this.cartService.updateQuantity(itemId, currentQty - 1);
    } else {
      this.removeItem(itemId);
    }
  }

  removeItem(itemId: string) {
    const item = this.cartService.cart()?.items.find((i) => i.id === itemId);
    if (item) {
      this.analytics.capture('removed_from_cart', {
        product_id: item.productId,
        product_name: item.productTitle,
      });
    }
    this.cartService.removeItem(itemId);
  }

  clearCart() {
    this.cartService.clearCart();
  }

  proceedToCheckout(): void {
    const cart = this.cartService.cart();
    this.analytics.capture('checkout_started', {
      cart_total_value: cart?.totalPrice ?? 0,
      items_count: cart?.items.length ?? 0,
      cta_variant: this.showNewCta() ? 'new' : 'old',
    });
    this.cartService.closeCart();
    this.router.navigate(['/checkout']);
  }
}
