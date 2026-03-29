export interface Product {
  id: string;
  title: string;
  description: string;
  price: number;
  isAvailable: boolean;
  stockQuantity: number;
  category: number;
  categoryName: string;
  imageUrl: string;
}

export interface ProductList {
  products: Product[];
  totalCount: number;
}
