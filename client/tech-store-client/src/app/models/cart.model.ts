export interface CartResponse {
  id: string;
  items: CartItemResponse[];
  totalPrice: number;
}

export interface CartItemResponse {
  id: string;
  productId: string;
  productTitle: string;
  productImageUrl: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
}

export interface AddToCartRequest {
  productId: string;
  quantity: number;
}

export interface UpdateCartItemRequest {
  quantity: number;
}
