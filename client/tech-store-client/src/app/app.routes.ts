import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./pages/home/home').then(m => m.HomePage) },
  { path: 'auth', loadComponent: () => import('./pages/auth/auth').then(m => m.AuthPage) },
  { path: 'product/:id', loadComponent: () => import('./pages/product/product').then(m => m.ProductPage) },
  { path: 'profile', loadComponent: () => import('./pages/profile/profile').then(m => m.ProfilePage), canActivate: [authGuard] },
  { path: 'admin', loadComponent: () => import('./pages/admin/admin').then(m => m.AdminPage), canActivate: [authGuard, adminGuard] },
  { path: '**', redirectTo: '' }
];
