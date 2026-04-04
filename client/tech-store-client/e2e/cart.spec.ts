import { expect, request as playwrightRequest, test } from '@playwright/test';
import { createTestProduct, registerTestUser } from './helpers/api';

test('logged in user can add a product to the cart and see it in the sidebar', async ({ page }) => {
  const api = await playwrightRequest.newContext({ ignoreHTTPSErrors: true });
  const user = await registerTestUser(api);
  const product = await createTestProduct(api);

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

  await page.goto(`/product/${product.id}`);

  await expect(page.getByTestId('product-title')).toHaveText(product.title);
  await expect(page.getByTestId('product-quantity')).toHaveText('1');

  await page.getByTestId('product-add-to-cart').click();

  await expect(page.getByTestId('cart-sidebar')).toHaveClass(/open/);
  await expect(page.getByTestId('cart-item-title')).toHaveText(product.title);
  await expect(page.getByTestId('cart-item-count')).toContainText('1');
  await expect(page.getByTestId('nav-cart-badge')).toHaveText('1');
  await expect(page.getByTestId('cart-subtotal')).toHaveText(`$${product.price.toFixed(2)}`);

  await api.dispose();
});
