import { test, expect, type Page, type Route } from '@playwright/test'
import { readFileSync } from 'fs'
import { fileURLToPath } from 'url'
import { dirname, join } from 'path'

const __dirname = dirname(fileURLToPath(import.meta.url))
const FIXTURES = join(__dirname, 'fixtures')

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

  // SMA indicator data
  await page.route(`${API_BASE}/indicators/sma**`, (route: Route) =>
    route.fulfill({ contentType: 'application/json', body: smaJson })
  )

  // RSI indicator data
  await page.route(`${API_BASE}/indicators/rsi**`, (route: Route) =>
    route.fulfill({ contentType: 'application/json', body: rsiJson })
  )

  // Any other indicator endpoint: return empty array (chart shows empty state)
  await page.route(`${API_BASE}/indicators/**`, (route: Route) =>
    route.fulfill({ contentType: 'application/json', body: '[]' })
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
  const errorState = root.locator('.indy-demo__status--error')
  const emptyState = root.locator('[data-testid$="-empty"]')

  // Wait for loading to finish
  await page.waitForFunction(
    (id) => {
      const el = document.querySelector(`[data-testid="${id}-loading"]`)
      return el === null
    },
    testId,
    { timeout: 20_000 }
  )

  if (await readyOverlay.isVisible()) return 'ready'
  if (await emptyState.isVisible()) return 'empty'
  if (await errorState.isVisible()) return 'error'
  return 'unknown'
}

// ---------------------------------------------------------------------------
// Overlay chart — SMA on the SMA indicator page
// ---------------------------------------------------------------------------

test('SMA overlay chart renders from static fixture data', async ({ page }) => {
  await mockStockChartsApi(page)
  await page.goto('/indicators/Sma')

  // The IndicatorChartPanel uses uiid = 'SMA'
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
  await expect(root.locator('[data-testid="stock-indicator-chart-rsi-overlay-canvas"]')).toBeVisible()
  await expect(root.locator('[data-testid="stock-indicator-chart-rsi-oscillator-canvas"]')).toBeVisible()
})

// ---------------------------------------------------------------------------
// Home page charts — BB, MACD, STC
// ---------------------------------------------------------------------------

test('Home page charts reach a terminal state', async ({ page }) => {
  await mockStockChartsApi(page)
  await page.goto('/')

  // Wait for all three charts on the home page to finish loading
  const chartIds = ['bb', 'macd', 'stc']
  for (const id of chartIds) {
    const root = page.locator(`[data-testid="stock-indicator-chart-${id}-root"]`)
    await expect(root).toBeVisible({ timeout: 15_000 })
    const phase = await waitForChartPhase(page, `stock-indicator-chart-${id}`)
    // Charts may show error (API fallback) or ready (with data), but must not hang
    expect(['ready', 'empty', 'error']).toContain(phase)
  }
})

// ---------------------------------------------------------------------------
// Bulk smoke test — all indicator pages with IndicatorChartPanel must load
// ---------------------------------------------------------------------------

const INDICATOR_PAGES: Array<{ page: string; uiid: string }> = [
  { page: 'Adx', uiid: 'adx' },
  { page: 'BollingerBands', uiid: 'bb' },
  { page: 'Ema', uiid: 'ema' },
  { page: 'Macd', uiid: 'macd' },
  { page: 'Rsi', uiid: 'rsi' },
  { page: 'Sma', uiid: 'sma' },
  { page: 'Stoch', uiid: 'sto' },
]

for (const { page: pageName, uiid } of INDICATOR_PAGES) {
  test(`${pageName} indicator page chart reaches terminal state`, async ({ page }) => {
    await mockStockChartsApi(page)
    await page.goto(`/indicators/${pageName}`)

    const root = page.locator(`[data-testid="stock-indicator-chart-${uiid}-root"]`)
    await expect(root).toBeVisible({ timeout: 15_000 })

    const phase = await waitForChartPhase(page, `stock-indicator-chart-${uiid}`)
    expect(['ready', 'empty', 'error']).toContain(phase)
  })
}
