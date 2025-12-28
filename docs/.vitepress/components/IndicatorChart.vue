<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'
import {
  createChart,
  type IChartApi,
  type ISeriesApi,
  ColorType,
  CrosshairMode,
  CandlestickSeries,
  LineSeries,
  AreaSeries,
  HistogramSeries
} from 'lightweight-charts'

interface ChartData {
  metadata?: {
    symbol?: string
    timeframe?: string
    indicator?: string
    parameters?: Record<string, unknown>
  }
  candles: Array<{
    timestamp: string
    open: number
    high: number
    low: number
    close: number
    volume?: number
  }>
  series: Array<{
    name: string
    type?: 'line' | 'area' | 'histogram'
    color?: string
    data: Array<{
      timestamp: string
      value: number | null
    }>
  }>
}

const props = withDefaults(defineProps<{
  src: string
  height?: number
  showVolume?: boolean
}>(), {
  height: 360,
  showVolume: false
})

const chartContainer = ref<HTMLDivElement | null>(null)
const isLoading = ref(true)
const hasError = ref(false)
const errorMessage = ref('')

let chart: IChartApi | null = null
let candleSeries: ISeriesApi<'Candlestick'> | null = null
let volumeSeries: ISeriesApi<'Histogram'> | null = null
const indicatorSeries: (ISeriesApi<'Line'> | ISeriesApi<'Area'> | ISeriesApi<'Histogram'>)[] = []

// Color palette for multiple indicator series
const indicatorColors = [
  'var(--si-indicator-1)',
  'var(--si-indicator-2)',
  'var(--si-indicator-3)',
  'var(--si-indicator-4)',
  'var(--si-indicator-5)'
]

// Fallback colors for CSS variables
const colorFallbacks: Record<string, string> = {
  '--si-indicator-1': '#539bf5',
  '--si-indicator-2': '#e6b450',
  '--si-indicator-3': '#c678dd',
  '--si-indicator-4': '#56b6c2',
  '--si-indicator-5': '#e06c75',
  '--si-chart-bg': '#22272e',
  '--si-chart-text': '#adbac7',
  '--si-chart-grid': '#2d333b',
  '--si-chart-border': '#444c56',
  '--si-chart-crosshair': '#768390'
}

function getComputedColor(cssVar: string): string {
  if (typeof window === 'undefined') return '#539bf5'
  const style = getComputedStyle(document.documentElement)
  const varName = cssVar.replace('var(', '').replace(')', '')
  const value = style.getPropertyValue(varName).trim()
  return value || colorFallbacks[varName] || '#539bf5'
}

function parseTimestamp(timestamp: string): string {
  // Convert ISO timestamp to lightweight-charts format (YYYY-MM-DD)
  try {
    const date = new Date(timestamp)
    if (isNaN(date.getTime())) {
      console.warn('Invalid timestamp:', timestamp)
      return '1970-01-01'
    }
    return date.toISOString().split('T')[0]
  } catch {
    console.warn('Error parsing timestamp:', timestamp)
    return '1970-01-01'
  }
}

async function loadChartData(): Promise<ChartData | null> {
  try {
    const response = await fetch(props.src)
    if (!response.ok) {
      throw new Error(`Failed to load chart data: ${response.status}`)
    }
    return await response.json()
  } catch (error) {
    console.error('Error loading chart data:', error)
    hasError.value = true
    errorMessage.value = error instanceof Error ? error.message : 'Failed to load chart data'
    return null
  }
}

function createChartInstance() {
  if (!chartContainer.value || typeof window === 'undefined') return

  // Get computed CSS values for theming
  const bgColor = getComputedColor('var(--si-chart-bg)') || '#22272e'
  const textColor = getComputedColor('var(--si-chart-text)') || '#adbac7'
  const gridColor = getComputedColor('var(--si-chart-grid)') || '#2d333b'
  const borderColor = getComputedColor('var(--si-chart-border)') || '#444c56'
  const crosshairColor = getComputedColor('var(--si-chart-crosshair)') || '#768390'

  chart = createChart(chartContainer.value, {
    autoSize: true,
    height: props.height,
    layout: {
      background: { type: ColorType.Solid, color: bgColor },
      textColor: textColor,
      fontFamily: 'Inter, system-ui, -apple-system, sans-serif'
    },
    grid: {
      vertLines: { color: gridColor },
      horzLines: { color: gridColor }
    },
    crosshair: {
      mode: CrosshairMode.Normal,
      vertLine: {
        color: crosshairColor,
        width: 1,
        style: 2,
        labelBackgroundColor: borderColor
      },
      horzLine: {
        color: crosshairColor,
        width: 1,
        style: 2,
        labelBackgroundColor: borderColor
      }
    },
    rightPriceScale: {
      borderColor: borderColor,
      scaleMargins: {
        top: 0.1,
        bottom: props.showVolume ? 0.2 : 0.1
      }
    },
    timeScale: {
      borderColor: borderColor,
      timeVisible: false,
      secondsVisible: false
    },
    handleScroll: {
      mouseWheel: true,
      pressedMouseMove: true,
      horzTouchDrag: true,
      vertTouchDrag: false
    },
    handleScale: {
      axisPressedMouseMove: true,
      mouseWheel: true,
      pinch: true
    }
  })

  return chart
}

function setupCandlestickSeries(data: ChartData) {
  if (!chart) return

  const upColor = getComputedColor('var(--si-candle-up)') || '#57ab5a'
  const downColor = getComputedColor('var(--si-candle-down)') || '#e5534b'
  const upWickColor = getComputedColor('var(--si-candle-wick-up)') || '#57ab5a'
  const downWickColor = getComputedColor('var(--si-candle-wick-down)') || '#e5534b'

  candleSeries = chart.addSeries(CandlestickSeries, {
    upColor: upColor,
    downColor: downColor,
    borderUpColor: upColor,
    borderDownColor: downColor,
    wickUpColor: upWickColor,
    wickDownColor: downWickColor
  })

  const candleData = data.candles.map(c => ({
    time: parseTimestamp(c.timestamp),
    open: c.open,
    high: c.high,
    low: c.low,
    close: c.close
  }))

  candleSeries.setData(candleData)
}

function setupVolumeSeries(data: ChartData) {
  if (!chart || !props.showVolume) return

  const upVolumeColor = getComputedColor('var(--si-volume-up)') || 'rgba(87, 171, 90, 0.3)'
  const downVolumeColor = getComputedColor('var(--si-volume-down)') || 'rgba(229, 83, 75, 0.3)'

  volumeSeries = chart.addSeries(HistogramSeries, {
    priceFormat: { type: 'volume' },
    priceScaleId: 'volume'
  })

  chart.priceScale('volume').applyOptions({
    scaleMargins: {
      top: 0.85,
      bottom: 0
    }
  })

  const volumeData = data.candles.map(c => ({
    time: parseTimestamp(c.timestamp),
    value: c.volume || 0,
    color: c.close >= c.open ? upVolumeColor : downVolumeColor
  }))

  volumeSeries.setData(volumeData)
}

function setupIndicatorSeries(data: ChartData) {
  if (!chart || !data.series || data.series.length === 0) return

  data.series.forEach((seriesConfig, index) => {
    const color = seriesConfig.color || indicatorColors[index % indicatorColors.length]
    const resolvedColor = color.startsWith('var(') ? getComputedColor(color) : color

    let series: ISeriesApi<'Line'> | ISeriesApi<'Area'> | ISeriesApi<'Histogram'>

    switch (seriesConfig.type) {
      case 'area':
        series = chart!.addSeries(AreaSeries, {
          lineColor: resolvedColor,
          topColor: `${resolvedColor}40`,
          bottomColor: `${resolvedColor}05`,
          lineWidth: 2
        })
        break
      case 'histogram':
        series = chart!.addSeries(HistogramSeries, {
          color: resolvedColor,
          priceScaleId: 'indicator-histogram'
        })
        chart!.priceScale('indicator-histogram').applyOptions({
          scaleMargins: {
            top: 0.8,
            bottom: 0
          }
        })
        break
      default:
        series = chart!.addSeries(LineSeries, {
          color: resolvedColor,
          lineWidth: 2,
          crosshairMarkerVisible: true,
          crosshairMarkerRadius: 4
        })
    }

    const seriesData = seriesConfig.data
      .filter(d => d.value !== null && d.value !== undefined && !isNaN(d.value))
      .map(d => ({
        time: parseTimestamp(d.timestamp),
        value: d.value as number
      }))

    series.setData(seriesData)
    indicatorSeries.push(series)
  })
}

async function initChart() {
  isLoading.value = true
  hasError.value = false

  const data = await loadChartData()
  if (!data) {
    isLoading.value = false
    return
  }

  // Wait for next tick to ensure container is rendered
  await new Promise(resolve => requestAnimationFrame(resolve))

  createChartInstance()
  setupCandlestickSeries(data)
  setupVolumeSeries(data)
  setupIndicatorSeries(data)

  // Fit content to view
  if (chart) {
    chart.timeScale().fitContent()
  }

  isLoading.value = false
}

function destroyChart() {
  if (chart) {
    chart.remove()
    chart = null
    candleSeries = null
    volumeSeries = null
    indicatorSeries.length = 0
  }
}

onMounted(() => {
  if (typeof window !== 'undefined') {
    initChart()
  }
})

onUnmounted(() => {
  destroyChart()
})

watch(() => props.src, () => {
  destroyChart()
  initChart()
})
</script>

<template>
  <div class="indicator-chart-wrapper">
    <div v-if="isLoading" class="chart-loading" :style="{ height: `${height}px` }">
      <span class="loading-spinner"></span>
      <span>Loading chart...</span>
    </div>

    <div v-else-if="hasError" class="chart-error" :style="{ height: `${height}px` }">
      <span>{{ errorMessage }}</span>
    </div>

    <div v-show="!isLoading && !hasError" ref="chartContainer" class="chart-container"
      :style="{ height: `${height}px` }" role="img" aria-label="Interactive indicator chart"></div>

    <noscript>
      <div class="chart-noscript">
        <p>JavaScript is required to display interactive charts.</p>
      </div>
    </noscript>
  </div>
</template>

<style scoped>
.indicator-chart-wrapper {
  width: 100%;
  margin: 1rem 0;
}

.chart-container {
  width: 100%;
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid var(--si-chart-border);
}

.chart-loading,
.chart-error {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  background-color: var(--si-chart-bg);
  border: 1px solid var(--si-chart-border);
  border-radius: 8px;
  color: var(--si-chart-text);
}

.chart-error {
  color: var(--vp-c-danger-1);
}

.loading-spinner {
  width: 24px;
  height: 24px;
  border: 2px solid var(--si-chart-border);
  border-top-color: var(--vp-c-brand-1);
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.chart-noscript {
  padding: 2rem;
  text-align: center;
  background-color: var(--si-chart-bg);
  border: 1px solid var(--si-chart-border);
  border-radius: 8px;
  color: var(--si-chart-text);
}
</style>
