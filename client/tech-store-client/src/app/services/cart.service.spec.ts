import { signal } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../environments/environment';
import { AddToCartRequest, CartResponse } from '../models/cart.model';
import { AuthService } from './auth.service';
import { CartService } from './cart.service';

describe('CartService', () => {
  let service: CartService;
  let httpTesting: HttpTestingController;
  let authLoggedIn: ReturnType<typeof signal<boolean>>;

  const createCartResponse = (
    overrides: Partial<CartResponse> = {},
  ): CartResponse => ({
    id: 'cart-1',
    totalPrice: 200,
    items: [
      {
        id: 'item-1',
        productId: 'product-1',
        productTitle: 'Laptop',
        productImageUrl: '/laptop.png',
        unitPrice: 100,
        quantity: 2,
        totalPrice: 200,
      },
    ],
    ...overrides,
  });

  beforeEach(() => {
    authLoggedIn = signal(false);

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: AuthService,
          useValue: {
            isLoggedIn: authLoggedIn,
          },
        },
      ],
    });

    service = TestBed.inject(CartService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('calculates itemCount from all cart items', () => {
    service.cart.set({
      id: 'cart-1',
      totalPrice: 140,
      items: [
        {
          id: 'item-1',
          productId: 'product-1',
          productTitle: 'Laptop',
          productImageUrl: '/laptop.png',
          unitPrice: 100,
          quantity: 1,
          totalPrice: 100,
        },
        {
          id: 'item-2',
          productId: 'product-2',
          productTitle: 'Mouse',
          productImageUrl: '/mouse.png',
          unitPrice: 20,
          quantity: 2,
          totalPrice: 40,
        },
      ],
    });

    expect(service.itemCount()).toBe(3);
  });

  it('updates cart state and opens the sidebar after addToCart succeeds', () => {
    const requestBody: AddToCartRequest = {
      productId: 'product-1',
      quantity: 2,
    };
    const response = createCartResponse();

    service.addToCart(requestBody);

    expect(service.loading()).toBe(true);

    const request = httpTesting.expectOne(`${environment.apiUrl}/carts/items`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(requestBody);

    request.flush(response);

    expect(service.cart()).toEqual(response);
    expect(service.loading()).toBe(false);
    expect(service.isCartOpen()).toBe(true);
  });

  it('loads the cart and updates state after a successful response', () => {
    const response = createCartResponse();

    service.loadCart();

    expect(service.loading()).toBe(true);

    const request = httpTesting.expectOne(`${environment.apiUrl}/carts/my-cart`);
    expect(request.request.method).toBe('GET');

    request.flush(response);

    expect(service.cart()).toEqual(response);
    expect(service.loading()).toBe(false);
  });

  it('stops the loading state when loadCart fails', () => {
    service.loadCart();

    expect(service.loading()).toBe(true);

    const request = httpTesting.expectOne(`${environment.apiUrl}/carts/my-cart`);
    expect(request.request.method).toBe('GET');

    request.flush({ message: 'Server error' }, { status: 500, statusText: 'Server Error' });

    expect(service.loading()).toBe(false);
    expect(service.cart()).toBeNull();
  });

  it('does not open the cart sidebar for logged out users', () => {
    expect(service.isCartOpen()).toBe(false);

    service.toggleCart();

    expect(service.isCartOpen()).toBe(false);
  });

  it('sends updateQuantity and stores the updated cart response', () => {
    const response = createCartResponse({
      totalPrice: 300,
      items: [
        {
          id: 'item-1',
          productId: 'product-1',
          productTitle: 'Laptop',
          productImageUrl: '/laptop.png',
          unitPrice: 100,
          quantity: 3,
          totalPrice: 300,
        },
      ],
    });

    service.updateQuantity('item-1', 3);

    expect(service.loading()).toBe(true);

    const request = httpTesting.expectOne(`${environment.apiUrl}/carts/items/item-1`);
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual({ quantity: 3 });

    request.flush(response);

    expect(service.cart()).toEqual(response);
    expect(service.loading()).toBe(false);
  });

  it('sends removeItem and stores the resulting cart response', () => {
    const response = createCartResponse({
      totalPrice: 0,
      items: [],
    });

    service.removeItem('item-1');

    expect(service.loading()).toBe(true);

    const request = httpTesting.expectOne(`${environment.apiUrl}/carts/items/item-1`);
    expect(request.request.method).toBe('DELETE');

    request.flush(response);

    expect(service.cart()).toEqual(response);
    expect(service.loading()).toBe(false);
  });

  it('clears the cart and stores the empty cart response', () => {
    const response = createCartResponse({
      totalPrice: 0,
      items: [],
    });

    service.clearCart();

    expect(service.loading()).toBe(true);

    const request = httpTesting.expectOne(`${environment.apiUrl}/carts/my-cart`);
    expect(request.request.method).toBe('DELETE');

    request.flush(response);

    expect(service.cart()).toEqual(response);
    expect(service.loading()).toBe(false);
  });
});
