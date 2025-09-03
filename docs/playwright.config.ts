import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  timeout: 60 * 1000,
  expect: { timeout: 5000 },
  fullyParallel: true,
  reporter: [['list']],
  use: {
    baseURL: process.env['E2E_BASE_URL'] || 'http://127.0.0.1:4000',
    headless: true
  },
  webServer: {
    command: 'npm run serve:static',
    url: 'http://127.0.0.1:4000',
    reuseExistingServer: !process.env['CI'],
    timeout: 120_000
  },
  projects: [
    { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
    { name: 'firefox', use: { ...devices['Desktop Firefox'] } },
    { name: 'webkit', use: { ...devices['Desktop Safari'] } }
  ]
});
