import { expect, request as playwrightRequest, test } from '@playwright/test';
import { registerTestUser } from './helpers/api';

test('user can sign in through the UI and see authenticated navigation', async ({ page }) => {
  const api = await playwrightRequest.newContext({ ignoreHTTPSErrors: true });
  const user = await registerTestUser(api);

  await page.goto('/auth');

  await page.getByTestId('auth-email').fill(user.email);
  await page.getByTestId('auth-password').fill(user.password);
  await Promise.all([
    page.waitForResponse((response) =>
      response.url().includes('/api/auth/login') && response.ok(),
    ),
    page.getByTestId('auth-submit').click(),
  ]);

  await expect(page.getByTestId('nav-profile')).toBeVisible({ timeout: 15_000 });
  await expect(page.getByTestId('nav-cart')).toBeVisible({ timeout: 15_000 });
  await expect(page.getByTestId('nav-logout')).toBeVisible({ timeout: 15_000 });
  await expect(page.getByTestId('nav-sign-in')).toHaveCount(0);

  await api.dispose();
});
