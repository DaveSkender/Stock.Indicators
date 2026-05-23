import { test, expect, type Page, type Route } from '@playwright/test'
import { readdirSync, readFileSync } from 'fs'
import { fileURLToPath } from 'url'
import { dirname, join } from 'path'

const __dirname = dirname(fileURLToPath(import.meta.url))
const FIXTURES = join(__dirname, '../.vitepress/public/data/chart-api')

const API_BASE = 'https://stock-charts-api.azurewebsites.net'

// Static fixture data loaded once
const quotesJson = readFileSync(join(FIXTURES, 'quotes.json'), 'utf8')
const indicatorsJson = readFileSync(join(FIXTURES, 'indicators.json'), 'utf8')
const smaJson = readFileSync(join(FIXTURES, 'sma.json'), 'utf8')
const rsiJson = readFileSync(join(FIXTURES, 'rsi.json'), 'utf8')

/**
 * Intercept all stock-charts API requests and respond with static fixture data.
 * Falls back gracefully when the live API is unavailable (CI or offline dev).
 */
async function mockStockChartsApi(page: Page): Promise<void> {
  await page.route(`${API_BASE}/quotes`, (route: Route) =>
    route.fulfill({ contentType: 'application/json', body: quotesJson })
  )

  await page.route(`${API_BASE}/indicators`, (route: Route) =>
    route.fulfill({ contentType: 'application/json', body: indicatorsJson })
  )

  // Routes are matched LIFO (last-registered = highest priority). The catch-all
  // below is registered before sma** and rsi**, so specific routes shadow it.
  // Any other indicator endpoint: return empty array (chart shows empty state)
  await page.route(`${API_BASE}/indicators/**`, (route: Route) =>
    route.fulfill({ contentType: 'application/json', body: '[]' })
  )

  // SMA indicator data
  await page.route(`${API_BASE}/indicators/sma**`, (route: Route) =>
    route.fulfill({ contentType: 'application/json', body: smaJson })
  )

  // RSI indicator data
  await page.route(`${API_BASE}/indicators/rsi**`, (route: Route) =>
    route.fulfill({ contentType: 'application/json', body: rsiJson })
  )
}

/**
 * Wait for a chart to reach a terminal state (ready, empty, or error).
 * Returns the phase that was reached.
 */
async function waitForChartPhase(page: Page, testId: string): Promise<string> {
  const root = page.locator(`[data-testid="${testId}-root"]`)
  await expect(root).toBeVisible({ timeout: 15_000 })

  const readyOverlay = root.locator('[data-testid$="-overlay-canvas"]')
  const emptyState = root.locator('[data-testid$="-empty"]')
  const errorState = root.locator('[data-testid$="-error"]')

  // Wait for one terminal UI marker to become visible — atomic, avoids the DOM-present
  // but not-yet-visible race that `waitForFunction` with querySelector can hit.
  const terminal = readyOverlay.or(emptyState).or(errorState)
  await terminal.first().waitFor({ state: 'visible', timeout: 20_000 })

  // Check error before ready: prevents a residual canvas from masking an error state.
  if (await errorState.isVisible()) return 'error'
  if (await emptyState.isVisible()) return 'empty'
  if (await readyOverlay.isVisible()) return 'ready'

  throw new Error(`Chart ${testId} did not reach a terminal state`)
}

// ---------------------------------------------------------------------------
// Overlay chart — SMA on the SMA indicator page
// ---------------------------------------------------------------------------

test('SMA overlay chart renders from static fixture data', async ({ page }) => {
  await mockStockChartsApi(page)
  await page.goto('/indicators/Sma')

  const root = page.locator('[data-testid="stock-indicator-chart-sma-root"]')
  await expect(root).toBeVisible({ timeout: 15_000 })

  const phase = await waitForChartPhase(page, 'stock-indicator-chart-sma')
  expect(phase, `Expected chart to be ready but got "${phase}"`).toBe('ready')

  const canvas = root.locator('[data-testid="stock-indicator-chart-sma-overlay-canvas"]')
  await expect(canvas).toBeVisible()
  await expect(canvas).toHaveAttribute('width')
})

// ---------------------------------------------------------------------------
// Oscillator chart — RSI on the RSI indicator page
// ---------------------------------------------------------------------------

test('RSI oscillator chart renders from static fixture data', async ({ page }) => {
  await mockStockChartsApi(page)
  await page.goto('/indicators/Rsi')

  const root = page.locator('[data-testid="stock-indicator-chart-rsi-root"]')
  await expect(root).toBeVisible({ timeout: 15_000 })

  const phase = await waitForChartPhase(page, 'stock-indicator-chart-rsi')
  expect(phase, `Expected chart to be ready but got "${phase}"`).toBe('ready')

  // Oscillator must show both overlay (price) and oscillator (RSI) canvases
  await expect(
    root.locator('[data-testid="stock-indicator-chart-rsi-overlay-canvas"]')
  ).toBeVisible()
  await expect(
    root.locator('[data-testid="stock-indicator-chart-rsi-oscillator-canvas"]')
  ).toBeVisible()
})

// ---------------------------------------------------------------------------
// Home page charts — BB, MACD, STC
// ---------------------------------------------------------------------------

test('Home page charts reach a terminal state', async ({ page }) => {
  await mockStockChartsApi(page)
  await page.goto('/')

  // Wait for all three charts on the home page to finish loading.
  // Slugs are derived from the registry keys in `.vitepress/theme/index.ts`.
  const chartIds = ['bollingerbands', 'macd', 'stc']
  for (const id of chartIds) {
    const root = page.locator(`[data-testid="stock-indicator-chart-${id}-root"]`)
    await expect(root).toBeVisible({ timeout: 15_000 })
    const phase = await waitForChartPhase(page, `stock-indicator-chart-${id}`)
    // Fixture-backed charts either render data or show a controlled empty state.
    expect(['ready', 'empty']).toContain(phase)
  }
})

// ---------------------------------------------------------------------------
// Bulk smoke test — all indicator pages with StockIndicatorChart must load
// ---------------------------------------------------------------------------

function toSlug(value: string): string {
  return value
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-|-$/g, '')
}

function indicatorPages(): Array<{ page: string; indicatorSlug: string }> {
  const indicatorsDir = join(__dirname, '../indicators')
  const files = readdirSync(indicatorsDir)

  return files
    .filter((file) => file.endsWith('.md'))
    .flatMap((file) => {
      const body = readFileSync(join(indicatorsDir, file), 'utf8')
      // Matches only self-closing <StockIndicatorChart indicator="..." ... />
      // forms; all current pages use this form. A future page that uses the
      // <StockIndicatorChart ...></StockIndicatorChart> long form would be
      // silently skipped - broaden the regex if/when that pattern appears.
      const matches = [...body.matchAll(/<StockIndicatorChart indicator="([^"]+)"[^/]*\/>/g)]
      if (matches.length === 0) return []

      const page = file.replace(/\.md$/, '')
      return matches.map((m) => ({
        page,
        indicatorSlug: toSlug(m[1])
      }))
    })
}

const INDICATOR_PAGES = indicatorPages()

for (const { page: pageName, indicatorSlug } of INDICATOR_PAGES) {
  test(`${pageName} indicator page chart reaches terminal state`, async ({ page }) => {
    await mockStockChartsApi(page)
    await page.goto(`/indicators/${pageName}`)

    const root = page.locator(`[data-testid="stock-indicator-chart-${indicatorSlug}-root"]`)
    await expect(root).toBeVisible({ timeout: 15_000 })

    const phase = await waitForChartPhase(page, `stock-indicator-chart-${indicatorSlug}`)
    // Accept 'error' too: the static indicators.json fixture intentionally
    // does not cover every uiid the live API exposes (e.g. DCPERIOD on the
    // HtTrendline page), so the library's listing-not-found error path is a
    // valid terminal state under fixture mocking - what matters is that the
    // chart root mounts and reaches a phase rather than hanging.
    expect(['ready', 'empty', 'error']).toContain(phase)
  })
}
