import { Component, OnInit, inject } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { NavbarComponent } from './components/navbar/navbar';
import { CartSidebarComponent } from './components/cart-sidebar/cart-sidebar';
// TODO Lab 6: remove ErrorTestComponent (import + <app-error-test /> in app.html)
// after Sentry verification screenshots are taken. The component file is kept.
import { ErrorTestComponent } from './components/error-test/error-test.component';
import { AnalyticsService } from './services/analytics.service';
import { ErrorTrackingService } from './services/error-tracking.service';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, CartSidebarComponent, ErrorTestComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  readonly appStatus = environment.appStatus;
  private analytics = inject(AnalyticsService);
  private errorTracking = inject(ErrorTrackingService);
  private router = inject(Router);

  ngOnInit(): void {
    this.analytics.init();
    // Order matters: Sentry after PostHog so error breadcrumbs can reference analytics state.
    this.errorTracking.init();

    this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe((event) => {
        this.analytics.capture('$pageview', { path: event.urlAfterRedirects });
      });
  }
}
