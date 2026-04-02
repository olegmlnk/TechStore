import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { AuthService } from './services/auth.service';
import { CartService } from './services/cart.service';

describe('App', () => {
  const authServiceMock = {
    isLoggedIn: vi.fn(() => false),
    isAdmin: vi.fn(() => false),
  };

  const cartServiceMock = {
    cart: signal(null),
    loading: signal(false),
    itemCount: signal(0),
    isCartOpen: signal(false),
    toggleCart: vi.fn(),
    closeCart: vi.fn(),
    updateQuantity: vi.fn(),
    removeItem: vi.fn(),
    clearCart: vi.fn(),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: authServiceMock },
        { provide: CartService, useValue: cartServiceMock },
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render the application shell', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-navbar')).toBeTruthy();
    expect(compiled.querySelector('app-cart-sidebar')).toBeTruthy();
    expect(compiled.querySelector('main')).toBeTruthy();
  });
});
