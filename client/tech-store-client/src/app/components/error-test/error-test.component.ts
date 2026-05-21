import { Component, inject } from '@angular/core';
import { ErrorTrackingService } from '../../services/error-tracking.service';

@Component({
  selector: 'app-error-test',
  standalone: true,
  template: `
    <div style="position: fixed; bottom: 20px; right: 20px; z-index: 9999; display: flex; flex-direction: column; gap: 8px;">
      <button (click)="throwError()" style="padding: 10px 16px; background: #dc2626; color: white; border: none; border-radius: 6px; cursor: pointer; font-weight: 600;">
        🔥 Break the world (test Sentry)
      </button>
      <button (click)="captureMessage()" style="padding: 10px 16px; background: #f59e0b; color: white; border: none; border-radius: 6px; cursor: pointer; font-weight: 600;">
        ⚠️ Send warning to Sentry
      </button>
    </div>
  `,
})
export class ErrorTestComponent {
  private errorTracking = inject(ErrorTrackingService);

  throwError(): void {
    this.errorTracking.addBreadcrumb({
      category: 'test',
      message: 'User clicked Break the world button',
      level: 'warning',
    });
    throw new Error('Sentry Test Error: TechStore intentional crash for monitoring verification');
  }

  captureMessage(): void {
    this.errorTracking.captureMessage('Test warning from TechStore', 'warning');
  }
}
