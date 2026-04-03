  import { PLATFORM_ID } from '@angular/core';
  import { provideHttpClient } from '@angular/common/http';
  import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
  import { TestBed } from '@angular/core/testing';
  import { Router } from '@angular/router';
  import { environment } from '../../environments/environment';
  import { AuthResponse, LoginRequest } from '../models/auth.model';
  import { AuthService } from './auth.service';

  describe('AuthService', () => {
    let httpTesting: HttpTestingController;
    let routerMock: { navigate: ReturnType<typeof vi.fn> };

    beforeEach(() => {
      localStorage.clear();
      routerMock = {
        navigate: vi.fn(() => Promise.resolve(true)),
      };

      TestBed.configureTestingModule({
        providers: [
          provideHttpClient(),
          provideHttpClientTesting(),
          { provide: Router, useValue: routerMock },
          { provide: PLATFORM_ID, useValue: 'browser' },
        ],
      });

      httpTesting = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
      httpTesting.verify();
      localStorage.clear();
    });

    it('loads the current user from localStorage on initialization', () => {
      const savedUser: AuthResponse = {
        token: 'saved-token',
        userId: 'user-1',
        email: 'saved@example.com',
        firstName: 'Saved',
        lastName: 'User',
        role: 'User',
      };

      localStorage.setItem('techstore_user', JSON.stringify(savedUser));

      const service = TestBed.inject(AuthService);

      expect(service.currentUser()).toEqual(savedUser);
      expect(service.isLoggedIn()).toBe(true);
      expect(service.isAdmin()).toBe(false);
    });

    it('stores auth data and updates state after a successful login', () => {
      const service = TestBed.inject(AuthService);
      const loginRequest: LoginRequest = {
        email: 'john@example.com',
        password: 'secret123',
      };
      const response: AuthResponse = {
        token: 'jwt-token',
        userId: 'user-42',
        email: 'john@example.com',
        firstName: 'John',
        lastName: 'Doe',
        role: 'Admin',
      };

      let emittedResponse: AuthResponse | undefined;
      service.login(loginRequest).subscribe((value) => {
        emittedResponse = value;
      });

      const request = httpTesting.expectOne(`${environment.apiUrl}/auth/login`);
      expect(request.request.method).toBe('POST');
      expect(request.request.body).toEqual(loginRequest);

      request.flush(response);

      expect(emittedResponse).toEqual(response);
      expect(service.currentUser()).toEqual(response);
      expect(service.isLoggedIn()).toBe(true);
      expect(service.isAdmin()).toBe(true);
      expect(localStorage.getItem('techstore_token')).toBe(response.token);
      expect(localStorage.getItem('techstore_user')).toBe(JSON.stringify(response));
    });

    it('clears auth state and navigates home on logout', () => {
      const savedUser: AuthResponse = {
        token: 'saved-token',
        userId: 'user-7',
        email: 'saved@example.com',
        firstName: 'Saved',
        lastName: 'User',
        role: 'User',
      };

      localStorage.setItem('techstore_token', savedUser.token);
      localStorage.setItem('techstore_user', JSON.stringify(savedUser));

      const service = TestBed.inject(AuthService);
      service.logout();

      expect(service.currentUser()).toBeNull();
      expect(service.isLoggedIn()).toBe(false);
      expect(localStorage.getItem('techstore_token')).toBeNull();
      expect(localStorage.getItem('techstore_user')).toBeNull();
      expect(routerMock.navigate).toHaveBeenCalledWith(['/']);
    });
  });
