import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  reporter: 'html',
  use: {
    baseURL: 'http://localhost:4200',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    ignoreHTTPSErrors: true,
  },
  webServer: [
    {
      command: 'dotnet run --project server/TechStore/TechStore.API/TechStore.API.csproj --launch-profile https',
      cwd: '../..',
      url: 'https://localhost:7290/api/products',
      ignoreHTTPSErrors: true,
      reuseExistingServer: !process.env.CI,
      timeout: 120_000,
    },
    {
      command: 'npm.cmd start',
      cwd: '.',
      url: 'http://localhost:4200',
      reuseExistingServer: !process.env.CI,
      timeout: 120_000,
    },
  ],
  projects: [
    {
      name: 'chromium',
      use: {
        ...devices['Desktop Chrome'],
      },
    },
  ],
});
