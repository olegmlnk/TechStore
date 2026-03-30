import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserProfile } from '../../models/auth.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class ProfilePage implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);

  profile = signal<UserProfile | null>(null);
  loading = signal(true);
  editing = signal(false);
  saving = signal(false);
  message = signal('');

  editForm = { firstName: '', lastName: '' };

  ngOnInit() {
    this.authService.getProfile().subscribe({
      next: (p) => {
        this.profile.set(p);
        this.editForm = { firstName: p.firstName, lastName: p.lastName };
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.router.navigate(['/auth']);
      }
    });
  }

  startEdit() {
    const p = this.profile();
    if (p) {
      this.editForm = { firstName: p.firstName, lastName: p.lastName };
      this.editing.set(true);
      this.message.set('');
    }
  }

  cancelEdit() {
    this.editing.set(false);
  }

  saveProfile() {
    this.saving.set(true);
    this.authService.updateProfile(this.editForm).subscribe({
      next: (p) => {
        this.profile.set(p);
        this.editing.set(false);
        this.saving.set(false);
        this.message.set('Profile updated successfully!');
        setTimeout(() => this.message.set(''), 3000);
      },
      error: () => {
        this.saving.set(false);
        this.message.set('Failed to update profile.');
      }
    });
  }

  logout() {
    this.authService.logout();
  }
}
