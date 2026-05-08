import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { AnalyticsService } from '../../services/analytics.service';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './product.html',
  styleUrl: './product.scss'
})
export class ProductPage implements OnInit {
  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);
  private cartService = inject(CartService);
  private analytics = inject(AnalyticsService);
  authService = inject(AuthService);

  product = signal<Product | null>(null);
  loading = signal(true);
  quantity = signal(1);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.productService.getById(id).subscribe({
        next: (p) => {
          this.product.set(p);
          this.loading.set(false);
          this.analytics.capture('product_viewed', {
            product_id: p.id,
            product_name: p.title,
            category: p.categoryName,
            price: p.price,
            in_stock: p.stockQuantity > 0,
          });
        },
        error: () => this.loading.set(false)
      });
    }
  }

  incrementQty() {
    const p = this.product();
    if (p && this.quantity() < p.stockQuantity) {
      this.quantity.set(this.quantity() + 1);
    }
  }

  decrementQty() {
    if (this.quantity() > 1) {
      this.quantity.set(this.quantity() - 1);
    }
  }

  addToCart(productId: string) {
    if (!this.authService.isLoggedIn()) {
      // In a real app we might redirect to login here
      alert('Please sign in to add items to your cart.');
      return;
    }

    const product = this.product();
    const qty = this.quantity();
    if (product) {
      this.analytics.capture('added_to_cart', {
        product_id: product.id,
        product_name: product.title,
        price: product.price,
        quantity: qty,
        cart_total_items: this.cartService.itemCount() + qty,
      });
    }

    this.cartService.addToCart({
      productId: productId,
      quantity: qty,
    });
  }
}
