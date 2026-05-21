export interface Environment {
  production: boolean;
  apiUrl: string;
  appStatus: string;
  posthogKey: string;
  posthogHost: string;
  sentryDsn: string;
}
