---
title: Custom chart (bring your own data)
description: Render a candlestick + volume chart from your own quote data using OverlayChart, with a locally computed EMA(20) overlay - no API required.
---

<script setup>
import StaticChart from './StaticChart.vue'
</script>

# Custom chart (bring your own data)

Render a candlestick + volume chart **and** a technical indicator overlay directly from your own quote data - no API required. Useful for offline demos, internal dashboards, or static documentation sites.

## Live demo

<ClientOnly>
  <StaticChart />
</ClientOnly>

The chart above plots OHLC + volume from a hard-coded `Quote[]` array, with an EMA(20) line computed locally. Everything below ships in the page bundle - no network calls.

## How it works

[`OverlayChart`](https://github.com/facioquo/stock-charts/tree/main/libs/indy-charts) is a lower-level building block exported from `@facioquo/indy-charts`. Unlike `<StockIndicatorChart>`, it does **not** fetch from an API - you supply the quotes directly. Add indicators by pushing your own `ChartDataset` onto `chart.data.datasets` after `render()`.

```typescript
import { OverlayChart, loadStaticQuotes, setupIndyCharts } from '@facioquo/indy-charts'
import type { Quote } from '@facioquo/indy-charts'

const quotes: Quote[] = loadStaticQuotes([
  { timestamp: '2025-01-02', open: 180.00, high: 182.50, low: 179.20, close: 181.80, volume: 38500000 },
  // ... more bars
])

setupIndyCharts() // one-time global Chart.js registration
const canvas = document.getElementById('my-canvas') as HTMLCanvasElement
const chart = new OverlayChart(canvas, { isDarkTheme: false, showTooltips: false })
chart.render(quotes)

// Push an EMA(20) line onto the existing chart.
chart.chart?.data.datasets.push(buildEmaDataset(quotes, 20))
chart.chart?.update('none')
```

Pair the script with a simple `<canvas>` host element:

```html
<div style="position: relative; aspect-ratio: 2.5;">
  <canvas id="my-canvas"></canvas>
</div>
```

## Computing the EMA locally

Without an API to compute indicators server-side, you can do it inline. EMA is just a recurrence:

```typescript
function computeEma(closes: number[], period: number): number[] {
  if (!Number.isInteger(period) || period <= 0) {
    throw new Error(`EMA period must be a positive integer, got ${period}`)
  }
  const k = 2 / (period + 1)
  const result: number[] = new Array(closes.length).fill(NaN)
  if (closes.length < period) return result

  // Seed with SMA of the first `period` closes.
  let sum = 0
  for (let i = 0; i < period; i++) sum += closes[i]
  result[period - 1] = sum / period

  // Standard EMA recurrence after the seed.
  for (let i = period; i < closes.length; i++) {
    result[i] = closes[i] * k + result[i - 1] * (1 - k)
  }
  return result
}
```

Wrap the result in a Chart.js line dataset on the price y-axis:

```typescript
import type { ChartDataset, ScatterDataPoint } from 'chart.js'
import type { Quote } from '@facioquo/indy-charts'

function buildEmaDataset(quotes: Quote[], period: number): ChartDataset<'line', ScatterDataPoint[]> {
  const ema = computeEma(quotes.map(q => q.close), period)
  return {
    type: 'line',
    label: `EMA(${period})`,
    data: quotes.map((q, i) => ({ x: new Date(q.timestamp).valueOf(), y: ema[i] })),
    yAxisID: 'y',
    borderColor: '#FFA726',
    backgroundColor: '#FFA726',
    borderWidth: 1.5,
    pointRadius: 0,
    fill: false,
    spanGaps: false,
    order: 0
  }
}
```

## Vue component source

The full Vue 3 SFC that drives the live demo is at [`docs/examples/StaticChart.vue`](https://github.com/facioquo/stock-indicators-dotnet/blob/main/docs/examples/StaticChart.vue) — that file is the source of truth (mount, theme observer, teardown, and EMA helpers). The page embeds it via `<ClientOnly>` so the Chart.js code only runs in the browser.

## Key points

- **`Quote`** — the OHLCV bar shape from `@facioquo/indy-charts`: numeric `open`, `high`, `low`, `close`, `volume`, plus `timestamp: Date | string`. `loadStaticQuotes` normalizes any string timestamps to `Date` instances.
- **`loadStaticQuotes(raw)`** — accepts raw bars whose `timestamp` is an ISO 8601 string or `Date`, normalizes them to `Date`, and returns `Quote[]`.
- **`setupIndyCharts()`** — one-time global Chart.js + financial-chart-type registration; safe to call multiple times.
- **`OverlayChart`** — renders candlestick + volume directly onto a `<canvas>` element.
- **Custom indicators** — push your own `ChartDataset` onto `chart.data.datasets` after `render()`, then call `chart.update('none')`. Any Chart.js dataset shape works (line, bar, scatter, etc.).
- **Theme sync** — re-render the chart on `document.documentElement` class changes to follow the page's dark/light mode. Guard against re-rendering for unrelated `<html>` class mutations (VitePress toggles other classes too).
- **Cleanup** — call `overlayChart.destroy()` from your unmount hook; it releases the wrapper state and the underlying Chart.js instance in one call.

::: tip ✨ Tip: direct lower-level use is opt-in
For most pages you should use the higher-level `<StockIndicatorChart>` from `@facioquo/indy-charts/vue` (registered globally in `.vitepress/theme/index.ts`). It fetches quotes + indicators from the configured API, manages its own lifecycle, and respects the central indicator catalog. Drop down to `OverlayChart` only when you genuinely need to ship data inline.
:::
