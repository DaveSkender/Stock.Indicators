/**
 * Capture indicator thumbnails from https://charts.stockindicators.dev
 * Usage: cd docs && node capture-thumbs.mjs
 */

import { chromium } from '@playwright/test';
import { writeFileSync, mkdirSync } from 'fs';
import { join, dirname } from 'path';
import { fileURLToPath } from 'url';

const __dirname = dirname(fileURLToPath(import.meta.url));
const OUT_DIR = join(__dirname, '.vitepress/public/assets/thumbs/indicators');

mkdirSync(OUT_DIR, { recursive: true });

// Maps demo indicator label → output filename (no .png)
// RETRY: only the 8 indicators that failed in the first run
const INDICATORS = [
  // 4 that timed out (dialog not closed bug — now fixed):
  ['Rate of Change', 'roc'],
  ['Standard Deviation (absolute)', 'std-dev'],
  ['STARC Bands', 'starc-bands'],
  ['Ulcer Index (UI)', 'ulcer-index'],
  // 4 "not added" — try alternative names:
  ['Price Momentum Oscillator', 'pmo'],
  ['Wilder Smoothed Moving Average', 'smma'],
  ['Standard Deviation Channel', 'std-dev-channels'],
  ['True Strength Index', 'tsi'],
];

/** Open settings dialog */
async function openSettings(page) {
  await page.click('main button');
  await page.waitForSelector('[role="dialog"]', { timeout: 5000 });
  await page.waitForTimeout(400);
}

/** Close settings dialog via the mat-icon-button X button */
async function closeSettings(page) {
  // Use Playwright locator click to fire Angular Zone events (page.evaluate won't)
  const closeBtn = page.locator('[role="dialog"] button.mdc-icon-button').first();
  if (await closeBtn.count() > 0) {
    await closeBtn.click();
  } else {
    // Fallback: press Escape
    await page.keyboard.press('Escape');
  }
  await page.waitForSelector('[role="dialog"]', { state: 'hidden', timeout: 5000 }).catch(() => {});
  await page.waitForTimeout(400);
}

/** Remove all displayed indicators via select-all checkbox + REMOVE SELECTED */
async function clearAllIndicators(page) {
  const count = await page.evaluate(() =>
    document.querySelectorAll('mat-selection-list mat-list-option').length
  );
  if (count === 0) return;

  // Click the select-all mat-checkbox input
  await page.evaluate(() => {
    const input = document.querySelector('mat-checkbox input[type="checkbox"]');
    if (input && !input.checked) input.click();
  });
  await page.waitForTimeout(200);

  // Click REMOVE SELECTED
  await page.evaluate(() => {
    const btns = document.querySelectorAll('[role="dialog"] button');
    for (const btn of btns) {
      if (btn.textContent.trim() === 'REMOVE SELECTED' && !btn.disabled) {
        btn.click();
        return;
      }
    }
  });
  await page.waitForTimeout(500);
}

/** Add an indicator: click nav item → params dialog opens → click ADD */
async function addIndicator(page, label) {
  // Click the indicator in the Available list
  const loc = page.locator('mat-nav-list a').filter({
    has: page.locator('.mdc-list-item__primary-text', { hasText: label })
  });
  if (await loc.count() === 0) return false;

  await loc.first().click();
  await page.waitForTimeout(400);

  // A params dialog should have appeared with an ADD button
  const addBtn = page.locator('[role="dialog"] button').filter({ hasText: 'ADD' });
  if (await addBtn.count() > 0) {
    await addBtn.first().click();
    await page.waitForTimeout(500);
    return true;
  }

  // If no ADD button, maybe it was added directly — check count
  const count = await page.evaluate(() =>
    document.querySelectorAll('mat-selection-list mat-list-option').length
  );
  return count > 0;
}

(async () => {
  const browser = await chromium.launch({ headless: false, slowMo: 20 });
  const context = await browser.newContext({
    viewport: { width: 1280, height: 800 },
  });
  const page = await context.newPage();

  console.log('Loading demo site...');
  await page.goto('https://charts.stockindicators.dev/', { waitUntil: 'networkidle' });
  await page.waitForTimeout(3000);

  let successCount = 0;
  let failCount = 0;
  const failed = [];

  for (const [label, filename] of INDICATORS) {
    const idx = successCount + failCount + 1;
    console.log(`[${idx}/${INDICATORS.length}] ${label}`);

    try {
      await openSettings(page);
      await clearAllIndicators(page);

      const added = await addIndicator(page, label);
      if (!added) {
        console.warn(`  ⚠ Not found in demo: ${label}`);
        failCount++;
        failed.push({ label, reason: 'not found' });
        await closeSettings(page);
        continue;
      }

      // Verify it was added to the displayed list
      const displayedCount = await page.evaluate(() => {
        return document.querySelectorAll('[role="listbox"] [role="option"]').length;
      });

      if (displayedCount === 0) {
        console.warn(`  ⚠ Indicator not added to chart: ${label}`);
        failCount++;
        failed.push({ label, reason: 'not added' });
        await closeSettings(page);
        continue;
      }

      await closeSettings(page);
      await page.waitForTimeout(2800); // wait for API + chart render

      // Capture the main element - tight crop based on actual canvas content
      const box = await page.locator('main').boundingBox();
      if (!box) {
        console.warn(`  ⚠ No bounding box`);
        failCount++;
        failed.push({ label, reason: 'no bbox' });
        continue;
      }

      // Get the chart overlay canvas bounding box for precise crop
      const canvasBox = await page.locator('#chartOverlay').boundingBox().catch(() => null);
      const capX = canvasBox ? canvasBox.x : box.x;
      const capY = canvasBox ? canvasBox.y : box.y;
      const capW = canvasBox ? canvasBox.width : box.width;
      // For height: use canvas height + a bit for sub-panes (oscillators etc)
      const subCanvases = await page.locator('canvas:not(#chartOverlay)').count();
      const subHeight = subCanvases > 0 ? Math.min(subCanvases * 150, 300) : 0;
      const capH = Math.min((canvasBox?.height ?? box.height) + subHeight, box.height);

      const buf = await page.screenshot({
        clip: { x: capX, y: capY, width: capW, height: capH }
      });

      writeFileSync(join(OUT_DIR, `${filename}.png`), buf);
      console.log(`  ✓ ${filename}.png (${Math.round(capW)}×${Math.round(capH)})`);
      successCount++;
    } catch (err) {
      console.error(`  ✗ Error: ${err.message}`);
      failCount++;
      failed.push({ label, reason: err.message });
      // Try to recover
      try {
        await page.keyboard.press('Escape');
        await page.waitForTimeout(600);
      } catch {}
    }
  }

  console.log(`\n=== Done: ${successCount} captured, ${failCount} failed ===`);
  if (failed.length) {
    console.log('Failed:');
    failed.forEach(f => console.log(`  - ${f.label}: ${f.reason}`));
  }

  await browser.close();
})();
