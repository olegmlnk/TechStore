import { signal } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../environments/environment';
import { AddToCartRequest, CartResponse } from '../app/models/cart.model';
import { AuthService } from '../app/services/auth.service';
import { CartService } from '../app/services/cart.service';

describe('CartService broken demo (intentional failure)', () => {
  let service: CartService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: AuthService,
          useValue: {
            isLoggedIn: signal(false),
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

  it('fails on purpose: after a successful addToCart the sidebar should stay closed', () => {
    const requestBody: AddToCartRequest = {
      productId: 'product-1',
      quantity: 1,
    };

    service.addToCart(requestBody);

    const addRequest = httpTesting.expectOne(`${environment.apiUrl}/carts/items`);
    expect(addRequest.request.method).toBe('POST');

    addRequest.flush({
      id: 'cart-1',
      totalPrice: 149.99,
      items: [
        {
          id: 'item-1',
          productId: 'product-1',
          productTitle: 'Broken Demo Product',
          productImageUrl: '/demo.png',
          unitPrice: 149.99,
          quantity: 1,
          totalPrice: 149.99,
        },
      ],
    } satisfies CartResponse);

    // Intentional bad expectation:
    // real code opens the sidebar with isCartOpen = true.
    // This assertion expects the opposite, so the test must fail.
    expect(service.isCartOpen()).toBe(false);
  });
});
