import { Injectable, signal, computed, PLATFORM_ID, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest, UserProfile, UpdateProfileRequest } from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private platformId = inject(PLATFORM_ID);

  private readonly TOKEN_KEY = 'techstore_token';
  private readonly USER_KEY = 'techstore_user';

  currentUser = signal<AuthResponse | null>(this.loadUser());
  isLoggedIn = computed(() => !!this.currentUser());
  isAdmin = computed(() => this.currentUser()?.role === 'Admin');

  register(req: RegisterRequest) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/register`, req)
      .pipe(tap(res => this.handleAuth(res)));
  }

  login(req: LoginRequest) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, req)
      .pipe(tap(res => this.handleAuth(res)));
  }

  logout() {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(this.TOKEN_KEY);
      localStorage.removeItem(this.USER_KEY);
    }
    this.currentUser.set(null);
    this.router.navigate(['/']);
  }

  getToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem(this.TOKEN_KEY);
    }
    return null;
  }

  getProfile() {
    return this.http.get<UserProfile>(`${environment.apiUrl}/user/profile`);
  }

  updateProfile(req: UpdateProfileRequest) {
    return this.http.put<UserProfile>(`${environment.apiUrl}/user/profile`, req);
  }

  private handleAuth(res: AuthResponse) {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(this.TOKEN_KEY, res.token);
      localStorage.setItem(this.USER_KEY, JSON.stringify(res));
    }
    this.currentUser.set(res);
  }

  private loadUser(): AuthResponse | null {
    if (isPlatformBrowser(this.platformId)) {
      const data = localStorage.getItem(this.USER_KEY);
      return data ? JSON.parse(data) : null;
    }
    return null;
  }
}
