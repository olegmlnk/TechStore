import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './auth.html',
  styleUrl: './auth.scss'
})
export class AuthPage {
  private authService = inject(AuthService);
  private router = inject(Router);

  isLogin = signal(true);
  error = signal('');
  loading = signal(false);

  form = {
    firstName: '',
    lastName: '',
    email: '',
    password: ''
  };

  toggleMode() {
    this.isLogin.set(!this.isLogin());
    this.error.set('');
  }

  submit() {
    this.error.set('');
    this.loading.set(true);

    if (this.isLogin()) {
      this.authService.login({ email: this.form.email, password: this.form.password })
        .subscribe({
          next: () => {
            this.loading.set(false);
            this.router.navigate(['/']);
          },
          error: (err) => {
            this.loading.set(false);
            this.error.set(err.error?.message || 'Login failed. Please try again.');
          }
        });
    } else {
      this.authService.register(this.form)
        .subscribe({
          next: () => {
            this.loading.set(false);
            this.router.navigate(['/']);
          },
          error: (err) => {
            this.loading.set(false);
            this.error.set(err.error?.message || 'Registration failed. Please try again.');
          }
        });
    }
  }
}
