import { Injectable, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import posthog from 'posthog-js';
import { environment } from '../../environments/environment';

export type AnalyticsProperties = Record<string, unknown>;

@Injectable({ providedIn: 'root' })
export class AnalyticsService {
  private platformId = inject(PLATFORM_ID);
  private initialized = false;

  init(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (this.initialized) return;

    posthog.init(environment.posthogKey, {
      api_host: environment.posthogHost,
      person_profiles: 'identified_only',
      capture_pageview: true,
      capture_pageleave: true,
      autocapture: true,
      session_recording: {
        maskAllInputs: true,
      },
    });

    this.initialized = true;
  }

  capture(event: string, properties?: AnalyticsProperties): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (!this.initialized) return;
    posthog.capture(event, properties);
  }

  identify(userId: string, properties?: AnalyticsProperties): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (!this.initialized) return;
    posthog.identify(userId, properties);
  }

  setPersonProperties(properties: AnalyticsProperties): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (!this.initialized) return;
    posthog.setPersonProperties(properties);
  }

  isFeatureEnabled(flag: string): boolean {
    if (!isPlatformBrowser(this.platformId)) return false;
    if (!this.initialized) return false;
    return posthog.isFeatureEnabled(flag) === true;
  }

  onFeatureFlagsLoaded(callback: () => void): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (!this.initialized) return;
    posthog.onFeatureFlags(callback);
  }

  reset(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (!this.initialized) return;
    posthog.reset();
  }
}
