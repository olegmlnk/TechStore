import { APIRequestContext, expect } from '@playwright/test';

const apiBaseUrl = 'https://localhost:7290/api';

export type RegisteredUser = {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
};

export type CreatedProduct = {
  id: string;
  title: string;
  price: number;
};

function createUniqueSuffix() {
  return `${Date.now()}-${Math.random().toString(36).slice(2, 8)}`;
}

export async function registerTestUser(api: APIRequestContext): Promise<RegisteredUser> {
  const suffix = createUniqueSuffix();
  const user: RegisteredUser = {
    firstName: 'E2E',
    lastName: 'User',
    email: `e2e-user-${suffix}@example.com`,
    password: 'Test123#',
  };

  const response = await api.post(`${apiBaseUrl}/auth/register`, {
    data: user,
  });

  expect(response.ok()).toBeTruthy();

  return user;
}

export async function loginAsAdmin(api: APIRequestContext): Promise<string> {
  const response = await api.post(`${apiBaseUrl}/auth/login`, {
    data: {
      email: 'admin@techstore.com',
      password: 'TechStore123#',
    },
  });

  expect(response.ok()).toBeTruthy();

  const body = await response.json();
  return body.token as string;
}

export async function createTestProduct(api: APIRequestContext): Promise<CreatedProduct> {
  const token = await loginAsAdmin(api);
  const suffix = createUniqueSuffix();

  const response = await api.post(`${apiBaseUrl}/products`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
    data: {
      title: `E2E Product ${suffix}`,
      description: 'Product created automatically for Playwright E2E tests.',
      price: 149.99,
      isAvailable: true,
      stockQuantity: 10,
      category: 0,
      imageUrl: 'https://placehold.co/600x400/png',
    },
  });

  expect(response.ok()).toBeTruthy();

  const body = await response.json();
  return {
    id: body.id as string,
    title: body.title as string,
    price: body.price as number,
  };
}
