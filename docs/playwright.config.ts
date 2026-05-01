import { defineConfig, devices } from '@playwright/test'

/**
 * Playwright configuration for visual chart inspection.
 *
 * Run after building and serving the docs site:
 *   pnpm run docs:build && pnpm run docs:preview &
 *   pnpm exec playwright test
 *
 * The tests mock all stock-charts API requests with static fixture data so
 * visual results are deterministic and do not require a live API service.
 */
export default defineConfig({
  testDir: './tests',
  fullyParallel: false,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  workers: 1,
  reporter: process.env.CI ? [['github'], ['html', { open: 'never' }]] : 'list',

  use: {
    baseURL: 'http://localhost:4173',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },

  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],

  webServer: {
    command: 'pnpm run docs:build && pnpm run docs:preview',
    url: 'http://localhost:4173',
    reuseExistingServer: !process.env.CI,
    timeout: 180_000,
  },
})
