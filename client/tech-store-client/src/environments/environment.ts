import type { Environment } from './environment.model';

export const environment: Environment = {
  production: false,
  apiUrl: 'https://localhost:7290/api',
  appStatus: 'Development',
  posthogKey: 'phc_qPJgDGdykMHPKMcMxFWHwbTHB2DfCvMdLa7dvXjD6hUU',
  posthogHost: 'https://us.i.posthog.com',
  sentryDsn: '',
};
