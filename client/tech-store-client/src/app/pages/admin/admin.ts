import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService, CreateProductRequest } from '../../services/product.service';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.html',
  styleUrl: './admin.scss'
})
export class AdminPage implements OnInit {
  private productService = inject(ProductService);

  products = signal<Product[]>([]);
  loading = signal(true);
  saving = signal(false);
  showModal = signal(false);
  editingId = signal<string | null>(null);
  deleteConfirmId = signal<string | null>(null);
  error = signal('');
  success = signal('');
  searchQuery = signal('');

  categories = [
    { id: 0, name: 'Computers' },
    { id: 1, name: 'Phones' },
    { id: 2, name: 'Accessories' },
    { id: 3, name: 'Software' },
    { id: 4, name: 'Gaming' },
    { id: 5, name: 'Audio' },
    { id: 6, name: 'Wearables' },
    { id: 7, name: 'Smart Home' },
    { id: 8, name: 'Networking' },
    { id: 9, name: 'Storage' },
    { id: 10, name: 'Peripherals' },
    { id: 11, name: 'Drones' },
    { id: 12, name: 'Cameras' },
    { id: 13, name: 'Printers' },
    { id: 14, name: 'Office Equipment' },
    { id: 15, name: 'Other' },
  ];

  form: CreateProductRequest = this.emptyForm();

  ngOnInit() {
    this.loadProducts();
  }

  private emptyForm(): CreateProductRequest {
    return {
      title: '',
      description: '',
      price: 0,
      isAvailable: true,
      stockQuantity: 0,
      category: 0,
      imageUrl: ''
    };
  }

  loadProducts() {
    this.loading.set(true);
    this.productService.getAllForAdmin().subscribe({
      next: (res) => {
        this.products.set(res.products);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  filteredProducts(): Product[] {
    const q = this.searchQuery().toLowerCase();
    if (!q) return this.products();
    return this.products().filter(p =>
      p.title.toLowerCase().includes(q) ||
      p.categoryName.toLowerCase().includes(q)
    );
  }

  openCreateModal() {
    this.form = this.emptyForm();
    this.editingId.set(null);
    this.error.set('');
    this.showModal.set(true);
  }

  openEditModal(product: Product) {
    this.form = {
      title: product.title,
      description: product.description,
      price: product.price,
      isAvailable: product.isAvailable,
      stockQuantity: product.stockQuantity,
      category: product.category,
      imageUrl: product.imageUrl
    };
    this.editingId.set(product.id);
    this.error.set('');
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.editingId.set(null);
    this.error.set('');
  }

  closeModalFromOverlay(event: MouseEvent) {
    if (event.target === event.currentTarget) {
      this.closeModal();
    }
  }

  saveProduct() {
    if (!this.form.title.trim() || this.form.price <= 0) {
      this.error.set('Title and a valid price are required.');
      return;
    }

    this.saving.set(true);
    this.error.set('');

    const id = this.editingId();

    if (id) {
      this.productService.update(id, this.form).subscribe({
        next: () => {
          this.saving.set(false);
          this.showModal.set(false);
          this.showSuccess('Product updated successfully!');
          this.loadProducts();
        },
        error: (err) => {
          this.saving.set(false);
          this.error.set(err.error?.message || 'Failed to update product.');
        }
      });
    } else {
      this.productService.create(this.form).subscribe({
        next: () => {
          this.saving.set(false);
          this.showModal.set(false);
          this.showSuccess('Product created successfully!');
          this.loadProducts();
        },
        error: (err) => {
          this.saving.set(false);
          this.error.set(err.error?.message || 'Failed to create product.');
        }
      });
    }
  }

  confirmDelete(id: string) {
    this.deleteConfirmId.set(id);
  }

  cancelDelete() {
    this.deleteConfirmId.set(null);
  }

  deleteProduct(id: string) {
    this.productService.delete(id).subscribe({
      next: () => {
        this.deleteConfirmId.set(null);
        this.showSuccess('Product deleted successfully!');
        this.loadProducts();
      },
      error: () => {
        this.deleteConfirmId.set(null);
        this.showSuccess('Failed to delete product.');
      }
    });
  }

  getCategoryName(catId: number): string {
    return this.categories.find(c => c.id === catId)?.name || 'Other';
  }

  private showSuccess(msg: string) {
    this.success.set(msg);
    setTimeout(() => this.success.set(''), 3000);
  }
}
