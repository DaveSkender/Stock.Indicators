import { test, expect } from '@playwright/test';

const ROUTES_TO_CHECK = ['/', '/guide', '/indicators', '/utilities', '/performance'];
const INDICATOR_SAMPLES = ['sma', 'rsi', 'macd'];
const HTML_ACCEPT_HEADERS = {
  accept: 'text/html,application/xhtml+xml;q=0.9,*/*;q=0.8'
} as const;

test.describe('Stock Indicators documentation endpoints', () => {
  test('home page responds with expected branding', async ({ request }) => {
    const response = await request.get('/', { headers: HTML_ACCEPT_HEADERS });
    expect(response.status()).toBe(200);

    const html = await response.text();
    expect(html).toContain('<app-root');
    expect(html).not.toMatch(/404|error/gi);
  });

  for (const route of ROUTES_TO_CHECK) {
    test(`serves ${route} without error text`, async ({ request }) => {
      const response = await request.get(route, { headers: HTML_ACCEPT_HEADERS });
      expect(response.status()).toBeLessThan(500);

      const html = await response.text();
      expect(html).not.toMatch(/404|error/gi);
    });
  }

  for (const indicator of INDICATOR_SAMPLES) {
    test(`indicator page for ${indicator} renders`, async ({ request }) => {
      const response = await request.get(`/indicators/${indicator}`, { headers: HTML_ACCEPT_HEADERS });
      expect(response.status()).toBeLessThan(500);

      const html = await response.text();
      expect(html).not.toMatch(/404|error/gi);
    });
  }

  test('sitemap (if present) is well formed', async ({ request }) => {
    const response = await request.get('/sitemap.xml');

    if (response.status() === 404) {
      test.info().annotations.push({ type: 'note', description: 'sitemap.xml not generated for this build' });
      return;
    }

    expect(response.status()).toBe(200);
    const xml = await response.text();
    expect(xml).toContain('<urlset');
  });
});
