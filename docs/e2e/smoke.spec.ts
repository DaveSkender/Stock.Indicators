import { test, expect } from '@playwright/test';

test('home page renders', async ({ page }) => {
    // Use root path to allow SPA routing to redirect to /docs/home when serving static files
    await page.goto('/');
    await expect(page).toHaveTitle(/Stock Indicators/i);
    await expect(page.locator('.docs-content')).toBeVisible();
});
