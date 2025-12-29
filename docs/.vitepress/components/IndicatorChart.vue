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
  HistogramSeries,
  BaselineSeries
} from 'lightweight-charts'

// Maximum number of bars to display (tail view)
const MAX_BARS = 100

interface ThresholdLine {
  value: number
  color: string
  style?: 'solid' | 'dash'
}

interface ChartData {
  metadata?: {
    symbol?: string
    timeframe?: string
    indicator?: string
    parameters?: Record<string, unknown>
    chartType?: 'overlay' | 'oscillator'
    thresholds?: ThresholdLine[]
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
    type?: 'line' | 'area' | 'histogram' | 'baseline'
    color?: string
    lineWidth?: number
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
const indicatorSeries: (ISeriesApi<'Line'> | ISeriesApi<'Area'> | ISeriesApi<'Histogram'> | ISeriesApi<'Baseline'>)[] = []

// Stock.Charts color scheme (Material Design M2)
const ChartColors = {
  StandardRed: '#DD2C00',           // deep orange A700
  StandardOrange: '#EF6C00',        // orange 800
  StandardGreen: '#2E7D32',         // green 800
  StandardBlue: '#1E88E5',          // blue 600
  StandardPurple: '#8E24AA',        // purple 600
  StandardGrayTransparent: '#9E9E9E50',
  DarkGray: '#616161CC',
  DarkGrayTransparent: '#61616110',
  ThresholdGrayTransparent: '#42424280',
  ThresholdRed: '#B71C1C70',
  ThresholdRedTransparent: '#B71C1C20',
  ThresholdGreen: '#1B5E2070',
  ThresholdGreenTransparent: '#1B5E2020'
}

// Color palette for multiple indicator series
const indicatorColors = [
  ChartColors.StandardBlue,
  ChartColors.StandardGreen,
  ChartColors.StandardRed,
  ChartColors.StandardPurple,
  ChartColors.StandardOrange
]

// Fallback colors for CSS variables
const colorFallbacks: Record<string, string> = {
  '--si-indicator-1': ChartColors.StandardBlue,
  '--si-indicator-2': ChartColors.StandardGreen,
  '--si-indicator-3': ChartColors.StandardRed,
  '--si-indicator-4': ChartColors.StandardPurple,
  '--si-indicator-5': ChartColors.StandardOrange,
  '--si-chart-bg': '#22272e',
  '--si-chart-text': '#adbac7',
  '--si-chart-grid': '#2d333b',
  '--si-chart-border': '#444c56',
  '--si-chart-crosshair': '#768390'
}

function getComputedColor(cssVar: string): string {
  if (typeof window === 'undefined') return ChartColors.StandardBlue
  const style = getComputedStyle(document.documentElement)
  const varName = cssVar.replace('var(', '').replace(')', '')
  const value = style.getPropertyValue(varName).trim()
  return value || colorFallbacks[varName] || ChartColors.StandardBlue
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
    const data = await response.json() as ChartData
    
    // Slice to show only the last MAX_BARS bars
    if (data.candles.length > MAX_BARS) {
      data.candles = data.candles.slice(-MAX_BARS)
    }
    
    // Slice series data to match
    if (data.series) {
      data.series = data.series.map(s => ({
        ...s,
        data: s.data.slice(-MAX_BARS)
      }))
    }
    
    return data
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

  chart = createChart(chartContainer.value, {
    autoSize: true,
    height: props.height,
    layout: {
      background: { type: 'none', color: bgColor }, // type: ColorType.Solid
      textColor: textColor,
      fontFamily: 'Inter, system-ui, -apple-system, sans-serif',
      fontSize: 10
      attributionLogo: false  // Remove TradingView logo
    },
    grid: {
      vertLines: { color: gridColor },
      horzLines: { color: gridColor }
    },
    crosshair: {
      mode: CrosshairMode.Normal,
      vertLine: {
        visible: false,
        labelVisible: false
      },
      horzLine: {
        visible: false,
        labelVisible: false
      }
    },
    rightPriceScale: {
      borderColor: borderColor,
      borderVisible: false,
      // scaleMargins: {
      //   top: 0.1,
      //   bottom: props.showVolume ? 0.2 : 0.1
      //}
    },
    leftPriceScale: {
      visible: false
    },
    timeScale: {
      visible: false,
      borderColor: borderColor,
      borderVisible: false,
      timeVisible: false,
      secondsVisible: false,
      fixLeftEdge: true,
      fixRightEdge: true,
      lockVisibleTimeRangeOnResize: true
    },
    // Disable all user interaction (scroll, pan, zoom)
    handleScroll: false,
    handleScale: false
  })

  return chart
}

function setupCandlestickSeries(data: ChartData) {
  if (!chart) return

  // Use Stock.Charts color scheme for candles
  const upColor = ChartColors.StandardGreen
  const downColor = ChartColors.StandardRed

  candleSeries = chart.addSeries(CandlestickSeries, {
    upColor: upColor,
    downColor: downColor,
    borderUpColor: upColor,
    borderDownColor: downColor,
    wickUpColor: upColor,
    wickDownColor: downColor,
    priceLineVisible: false,      // Remove price line marker
    lastValueVisible: false       // Remove last value label on axis
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

  const upVolumeColor = 'rgba(46, 125, 50, 0.3)'   // StandardGreen with alpha
  const downVolumeColor = 'rgba(221, 44, 0, 0.3)' // StandardRed with alpha

  volumeSeries = chart.addSeries(HistogramSeries, {
    priceFormat: { type: 'volume' },
    priceScaleId: 'volume',
    priceLineVisible: false,
    lastValueVisible: false
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
    // Use the series color or fall back to the color palette
    const color = seriesConfig.color || indicatorColors[index % indicatorColors.length]
    const resolvedColor = color.startsWith('var(') ? getComputedColor(color) : color
    const lineWidth = seriesConfig.lineWidth || 2

    let series: ISeriesApi<'Line'> | ISeriesApi<'Area'> | ISeriesApi<'Histogram'> | ISeriesApi<'Baseline'>

    switch (seriesConfig.type) {
      case 'area':
        series = chart!.addSeries(AreaSeries, {
          lineColor: resolvedColor,
          topColor: `${resolvedColor}40`,
          bottomColor: `${resolvedColor}05`,
          lineWidth: lineWidth,
          priceLineVisible: false,
          lastValueVisible: false,
          crosshairMarkerVisible: false
        })
        break
      case 'histogram':
        series = chart!.addSeries(HistogramSeries, {
          color: resolvedColor,
          priceScaleId: 'indicator-histogram',
          priceLineVisible: false,
          lastValueVisible: false
        })
        chart!.priceScale('indicator-histogram').applyOptions({
          scaleMargins: {
            top: 0.8,
            bottom: 0
          }
        })
        break
      case 'baseline':
        series = chart!.addSeries(BaselineSeries, {
          baseValue: { type: 'price', price: 0 },
          topLineColor: ChartColors.StandardGreen,
          topFillColor1: ChartColors.ThresholdGreenTransparent,
          topFillColor2: ChartColors.ThresholdGreenTransparent,
          bottomLineColor: ChartColors.StandardRed,
          bottomFillColor1: ChartColors.ThresholdRedTransparent,
          bottomFillColor2: ChartColors.ThresholdRedTransparent,
          lineWidth: lineWidth,
          priceLineVisible: false,
          lastValueVisible: false,
          crosshairMarkerVisible: false
        })
        break
      default:
        series = chart!.addSeries(LineSeries, {
          color: resolvedColor,
          lineWidth: lineWidth,
          priceLineVisible: false,
          lastValueVisible: false,
          crosshairMarkerVisible: false
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
  margin: 0;
}

.chart-container {
  width: 100%;
  border-radius: 8px;
  overflow: hidden;
  border: 0 solid var(--si-chart-border);
}

/* Hide the TradingView attribution link via CSS as backup */
.chart-container :deep(a[href*="tradingview"]) {
  display: none !important;
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
