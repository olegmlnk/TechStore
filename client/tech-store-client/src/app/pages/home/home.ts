import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class HomePage implements OnInit {
  private productService = inject(ProductService);

  products = signal<Product[]>([]);
  filteredProducts = signal<Product[]>([]);
  loading = signal(true);
  searchQuery = signal('');
  selectedCategory = signal<number | null>(null);

  categories = [
    { id: 0, name: 'Computers', icon: '💻' },
    { id: 1, name: 'Phones', icon: '📱' },
    { id: 2, name: 'Accessories', icon: '🎧' },
    { id: 4, name: 'Gaming', icon: '🎮' },
    { id: 5, name: 'Audio', icon: '🔊' },
    { id: 6, name: 'Wearables', icon: '⌚' },
    { id: 7, name: 'Smart Home', icon: '🏠' },
    { id: 10, name: 'Peripherals', icon: '🖱️' },
    { id: 11, name: 'Drones', icon: '🚁' },
    { id: 12, name: 'Cameras', icon: '📷' },
    { id: 9, name: 'Storage', icon: '💾' },
  ];

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.loading.set(true);
    this.productService.getAll().subscribe({
      next: (res) => {
        this.products.set(res.products);
        this.filteredProducts.set(res.products);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  filterByCategory(categoryId: number | null) {
    this.selectedCategory.set(categoryId);
    this.searchQuery.set('');

    if (categoryId === null) {
      this.filteredProducts.set(this.products());
      return;
    }

    this.loading.set(true);
    this.productService.getByCategory(categoryId).subscribe({
      next: (res) => {
        this.filteredProducts.set(res.products);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onSearch() {
    const q = this.searchQuery();
    this.selectedCategory.set(null);

    if (!q.trim()) {
      this.filteredProducts.set(this.products());
      return;
    }

    this.loading.set(true);
    this.productService.search(q).subscribe({
      next: (res) => {
        this.filteredProducts.set(res.products);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
}
