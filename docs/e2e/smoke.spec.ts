import { test, expect } from '@playwright/test';

test.describe('Stock Indicators Documentation Site', () => {
  test('home page loads successfully', async ({ page }) => {
    await page.goto('/');
    
    // Check if the page title is correct
    await expect(page).toHaveTitle(/Stock Indicators for \.NET/);
    
    // Check for main content
    await expect(page.locator('body')).toBeVisible();
    
    // Check for navigation elements (adjust selectors based on actual site structure)
    const navigation = page.locator('nav, .navbar, [role="navigation"]');
    if (await navigation.count() > 0) {
      await expect(navigation.first()).toBeVisible();
    }
  });

  test('documentation pages are accessible', async ({ page }) => {
    await page.goto('/');
    
    // Look for documentation links
    const docLinks = page.locator('a[href*="/docs/"], a[href*="/indicators/"]');
    const linkCount = await docLinks.count();
    
    if (linkCount > 0) {
      // Test the first documentation link
      const firstLink = docLinks.first();
      const href = await firstLink.getAttribute('href');
      
      if (href) {
        await page.goto(href);
        await expect(page.locator('body')).toBeVisible();
        
        // Check that we're not on an error page
        await expect(page.locator('h1')).not.toContainText('404');
        await expect(page.locator('h1')).not.toContainText('Error');
      }
    }
  });

  test('search functionality works if present', async ({ page }) => {
    await page.goto('/');
    
    // Look for search input
    const searchInput = page.locator('input[type="search"], input[placeholder*="search"], #search');
    
    if (await searchInput.count() > 0) {
      await searchInput.fill('SMA');
      await expect(searchInput).toHaveValue('SMA');
    }
  });

  test('responsive design works on mobile', async ({ page }) => {
    // Set mobile viewport
    await page.setViewportSize({ width: 375, height: 667 });
    await page.goto('/');
    
    // Check that the page is still functional on mobile
    await expect(page.locator('body')).toBeVisible();
    
    // Check for mobile navigation if present
    const mobileNav = page.locator('.mobile-nav, .navbar-toggle, .hamburger, [aria-label*="menu"]');
    if (await mobileNav.count() > 0) {
      await expect(mobileNav.first()).toBeVisible();
    }
  });

  test('accessibility landmarks are present', async ({ page }) => {
    await page.goto('/');
    
    // Check for basic accessibility landmarks
    const main = page.locator('main, [role="main"]');
    if (await main.count() > 0) {
      await expect(main.first()).toBeVisible();
    }
    
    // Check for proper heading structure
    const h1 = page.locator('h1');
    if (await h1.count() > 0) {
      await expect(h1.first()).toBeVisible();
    }
  });
});