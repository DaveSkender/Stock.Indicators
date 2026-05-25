<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref } from 'vue'

import {
  ChartManager,
  createApiClient,
  createDefaultSelection,
  loadStaticIndicatorData,
  setupIndyCharts,
  type IndicatorListing,
  type IndicatorSelection
} from '@facioquo/indy-charts'

const API_BASE = 'https://stock-charts-api.azurewebsites.net'
const BAR_COUNT = 250

const overlayCanvas = ref<HTMLCanvasElement | null>(null)
const phase = ref<'idle' | 'loading' | 'ready' | 'error'>('idle')
const errorMessage = ref('Unable to load chart preview.')

let chartManager: ChartManager | null = null
let themeObserver: MutationObserver | null = null
let resizeHandler: (() => void) | null = null
let disposed = false
let loadToken = 0

interface ChartSpec {
  uiid: string
  label: string
  params?: Record<string, number>
}

const overlaySpecs: ChartSpec[] = [
  { uiid: 'Ema', label: 'EMA(200)', params: { lookbackPeriods: 200 } },
  { uiid: 'Ema', label: 'EMA(50)', params: { lookbackPeriods: 50 } },
  { uiid: 'Slope', label: 'LINEAR(30)', params: { lookbackPeriods: 30 } },
  { uiid: 'Marubozu', label: 'MARUBOZU(90%)', params: { minBodyPercent: 90 } },
  { uiid: 'AtrStopClose', label: 'ATR-STOP(21,3,CLOSE)', params: { lookbackPeriods: 21, multiplier: 3 } }
]

function isDark(): boolean {
  return document.documentElement.classList.contains('dark')
}

function currentSettings() {
  return {
    isDarkTheme: isDark(),
    showTooltips: false
  }
}

function destroyChart(): void {
  chartManager?.destroy()
  chartManager = null
}

function updateTheme(): void {
  chartManager?.updateTheme(currentSettings())
}

function normalizeSelection(selection: IndicatorSelection, label: string): IndicatorSelection {
  selection.label = label
  selection.results.forEach((result) => {
    result.label = label
  })
  return selection
}

function findListing(listings: IndicatorListing[], uiid: string): IndicatorListing | undefined {
  return listings.find((listing) => listing.uiid.toLowerCase() === uiid.toLowerCase())
}

async function renderCharts(): Promise<void> {
  const token = ++loadToken
  phase.value = 'loading'
  errorMessage.value = 'Unable to load chart preview.'
  destroyChart()

  try {
    const canvas = overlayCanvas.value
    if (!canvas) {
      throw new Error('Overlay chart canvas is not available.')
    }

    setupIndyCharts()

    const client = createApiClient({ baseUrl: API_BASE })
    const [quotes, listings] = await Promise.all([client.getQuotes(), client.getListings()])
    if (disposed || token !== loadToken) return

    const manager = new ChartManager({ settings: currentSettings() })
    chartManager = manager
    manager.initializeOverlay(canvas, quotes, BAR_COUNT)

    for (const spec of overlaySpecs) {
      const listing = findListing(listings, spec.uiid)
      if (!listing) {
        throw new Error(`Indicator listing not found for uiid "${spec.uiid}".`)
      }

      const selection = normalizeSelection(
        createDefaultSelection(listing, spec.params, 'landing-home-'),
        spec.label
      )
      const data = loadStaticIndicatorData(await client.getSelectionData(selection, listing))
      if (disposed || token !== loadToken) return

      manager.processSelectionData(selection, listing, data)
      manager.displaySelection(selection, listing)
    }

    if (disposed || token !== loadToken) return
    phase.value = 'ready'
    await Promise.resolve()
    manager.resize()
  } catch (error) {
    if (disposed || token !== loadToken) return
    destroyChart()
    phase.value = 'error'
    errorMessage.value = error instanceof Error ? error.message : errorMessage.value
  }
}

onMounted(() => {
  void renderCharts()

  themeObserver = new MutationObserver(() => {
    updateTheme()
  })
  themeObserver.observe(document.documentElement, {
    attributes: true,
    attributeFilter: ['class']
  })

  resizeHandler = () => {
    chartManager?.resize()
  }
  window.addEventListener('resize', resizeHandler)
})

onBeforeUnmount(() => {
  disposed = true
  loadToken += 1
  themeObserver?.disconnect()
  themeObserver = null
  if (resizeHandler) {
    window.removeEventListener('resize', resizeHandler)
    resizeHandler = null
  }
  destroyChart()
})
</script>

<template>
  <section class="home-charts-stack" data-testid="landing-charts-root">
    <div class="home-charts-stack__panel home-charts-stack__panel--overlay">
      <div class="home-charts-stack__canvas-wrap home-charts-stack__canvas-wrap--overlay">
        <canvas
          ref="overlayCanvas"
          class="home-charts-stack__canvas"
          data-testid="landing-charts-overlay-canvas"
        />
        <div
          v-if="phase === 'loading'"
          class="indy-demo__status indy-demo__status--loading home-charts-stack__status"
        >
          Loading chart preview...
        </div>
        <div
          v-else-if="phase === 'error'"
          class="indy-demo__status indy-demo__status--error home-charts-stack__status"
        >
          {{ errorMessage }}
        </div>
      </div>
    </div>

    <StockIndicatorChart indicator="Macd" id="landing-macd" :bar-count="BAR_COUNT" />
    <StockIndicatorChart indicator="Stc" id="landing-stc" :bar-count="BAR_COUNT" />
  </section>
</template>

<style scoped>
.home-charts-stack {
  display: flex;
  flex-direction: column;
  gap: 0.35rem;
}

.home-charts-stack__panel {
  overflow: hidden;
}

.home-charts-stack__canvas-wrap {
  position: relative;
  width: 100%;
  background: var(--vp-c-bg);
}

.home-charts-stack__canvas-wrap--overlay {
  aspect-ratio: 2.15;
}

.home-charts-stack__canvas {
  display: block;
  width: 100%;
  height: 100%;
}

.home-charts-stack__status {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  background: var(--vp-c-bg);
}

.indy-demo__status {
  padding: 0.25rem 0 0.35rem;
  font-size: 13px;
  color: var(--vp-c-text-2);
}

.indy-demo__status--error {
  color: var(--vp-c-warning-1);
}

:deep(.indy-demo) {
  margin: 0 !important;
}
</style>
