import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { AuthService } from '../../services/auth.service';
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
}
