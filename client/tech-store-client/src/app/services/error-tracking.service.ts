import { Injectable, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import * as Sentry from '@sentry/angular';
import { environment } from '../../environments/environment';

export interface SentryUser {
  id: string;
  email?: string;
  username?: string;
}

@Injectable({ providedIn: 'root' })
export class ErrorTrackingService {
  private platformId = inject(PLATFORM_ID);
  private initialized = false;

  init(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (this.initialized) return;

    if (!environment.sentryDsn) {
      console.warn('[ErrorTracking] Sentry DSN is empty — error tracking is disabled.');
      return;
    }

    Sentry.init({
      dsn: environment.sentryDsn,
      environment: environment.production ? 'production' : 'development',
      integrations: [
        Sentry.browserTracingIntegration(),
        Sentry.replayIntegration({
          maskAllText: false,
          maskAllInputs: true, // не записувати введення паролів/карт
          blockAllMedia: false,
        }),
      ],
      tracesSampleRate: environment.production ? 0.2 : 1.0,
      replaysSessionSampleRate: 0.1,
      replaysOnErrorSampleRate: 1.0,
      release: 'techstore@' + (environment.production ? 'production' : 'dev'),
      beforeSend(event) {
        // не відправляти події з localhost у production
        if (environment.production && event.request?.url?.includes('localhost')) {
          return null;
        }
        return event;
      },
    });

    this.initialized = true;
  }

  setUser(user: SentryUser): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (!this.initialized) return;
    Sentry.setUser(user);
  }

  clearUser(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (!this.initialized) return;
    Sentry.setUser(null);
  }

  captureException(error: Error, context?: Record<string, unknown>): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (!this.initialized) return;
    Sentry.captureException(error, context ? { extra: context } : undefined);
  }

  captureMessage(message: string, level: Sentry.SeverityLevel = 'info'): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (!this.initialized) return;
    Sentry.captureMessage(message, level);
  }

  addBreadcrumb(breadcrumb: Sentry.Breadcrumb): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (!this.initialized) return;
    Sentry.addBreadcrumb(breadcrumb);
  }

  setTag(key: string, value: string): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (!this.initialized) return;
    Sentry.setTag(key, value);
  }
}
