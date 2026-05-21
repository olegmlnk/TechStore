import type { Environment } from './environment.model';

export const environment: Environment = {
  production: false,
  apiUrl: 'https://localhost:7290/api',
  appStatus: 'Development',
  posthogKey: 'phc_qPJgDGdykMHPKMcMxFWHwbTHB2DfCvMdLa7dvXjD6hUU',
  posthogHost: 'https://us.i.posthog.com',
  sentryDsn: 'https://53e091c044280a5dc2bd859c3f6715d5@o4511429958500352.ingest.de.sentry.io/4511429960663120',
};
