import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-cart-sidebar',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './cart-sidebar.html',
  styleUrl: './cart-sidebar.scss'
})
export class CartSidebarComponent {
  cartService = inject(CartService);

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
    this.cartService.removeItem(itemId);
  }

  clearCart() {
    this.cartService.clearCart();
  }
}
