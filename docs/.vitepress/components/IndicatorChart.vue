<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, computed } from 'vue'
import { useData } from 'vitepress'
import {
  createChart,
  type IChartApi,
  type ISeriesApi,
  CrosshairMode,
  CandlestickSeries,
  LineSeries,
  AreaSeries,
  HistogramSeries,
  BaselineSeries,
  LineStyle,
  LineWidth
} from 'lightweight-charts'

// Maximum number of bars to display (tail view)
const MAX_BARS_WIDE = 100
const MAX_BARS_MOBILE = 80

// Container width initialization polling settings
const INIT_POLL_MAX_ATTEMPTS = 10
const INIT_POLL_INTERVAL_MS = 50

// Debounce delay for resize events (ms)
const RESIZE_DEBOUNCE_MS = 100

interface ThresholdLine {
  value: number
  color: string
  style?: 'solid' | 'dash'
  fill?: 'above' | 'below'
  fillColor?: string
}

interface SeriesStyle {
  name: string
  type?: 'line' | 'area' | 'histogram' | 'baseline' | 'dots'
  color?: string
  lineWidth?: LineWidth
  lineStyle?: 'solid' | 'dash' | 'dots'
  data: Array<{
    timestamp: string
    value: number | null
    color?: string
  }>
}

interface ChartData {
  metadata?: {
    symbol?: string
    timeframe?: string
    indicator?: string
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
  series: SeriesStyle[]
}

const props = withDefaults(defineProps<{
  src: string
}>(), {})

// Get VitePress theme state
const { isDark } = useData()

const overlayChartContainer = ref<HTMLDivElement | null>(null)
const oscillatorChartContainer = ref<HTMLDivElement | null>(null)
const isLoading = ref(true)
const hasError = ref(false)
const errorMessage = ref('')
const chartType = ref<'overlay' | 'oscillator'>('overlay')

let overlayChart: IChartApi | null = null
let oscillatorChart: IChartApi | null = null
let candleSeries: ISeriesApi<'Candlestick'> | null = null
let volumeSeries: ISeriesApi<'Histogram'> | null = null
const overlaySeries: ISeriesApi<any>[] = []
const oscillatorSeries: ISeriesApi<any>[] = []
let resizeObserver: ResizeObserver | null = null
let resizeTimeout: ReturnType<typeof setTimeout> | null = null

// Stock.Charts color scheme (Material Design M2)
const ChartColors = {
  StandardRed: '#DD2C00',
  StandardOrange: '#EF6C00',
  StandardGreen: '#2E7D32',
  StandardBlue: '#1E88E5',
  StandardPurple: '#8E24AA',
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

// Fixed price scale width for alignment between charts
const PRICE_SCALE_WIDTH = 50

// Dark theme colors (GitHub Primer dark-dimmed)
const darkTheme = {
  bgColor: 'transparent',
  textColor: '#768390',
  gridColor: '#30363d',
  borderColor: 'transparent'
}

// Light theme colors (GitHub Primer light)
const lightTheme = {
  bgColor: 'transparent',
  textColor: '#57606a',
  gridColor: '#d0d7de',
  borderColor: 'transparent'
}

// Reactive chart theme based on VitePress dark mode
const chartTheme = computed(() => isDark.value ? darkTheme : lightTheme)

// Track viewport width for responsive behavior
const viewportWidth = ref(0)
// Use standard mobile breakpoint (480px) - aligns with project breakpoints in style.scss
const isMobileViewport = computed(() => viewportWidth.value > 0 && viewportWidth.value < 480)

// Responsive bar count based on viewport width
const maxBars = computed(() => isMobileViewport.value ? MAX_BARS_MOBILE : MAX_BARS_WIDE)

function parseTimestamp(timestamp: string): string {
  try {
    const date = new Date(timestamp)
    if (isNaN(date.getTime())) {
      return '1970-01-01'
    }
    return date.toISOString().split('T')[0]
  } catch {
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

    // Slice to show only the last maxBars bars
    const barCount = maxBars.value
    if (data.candles.length > barCount) {
      data.candles = data.candles.slice(-barCount)
    }

    // Slice series data to match
    if (data.series) {
      data.series = data.series.map(s => ({
        ...s,
        data: s.data.slice(-barCount)
      }))
    }

    // Set chart type from metadata
    chartType.value = data.metadata?.chartType || 'overlay'

    return data
  } catch (error) {
    console.error('Error loading chart data:', error)
    hasError.value = true
    errorMessage.value = error instanceof Error ? error.message : 'Failed to load chart data'
    return null
  }
}

function createOverlayChart(container: HTMLDivElement): IChartApi {
  const theme = chartTheme.value
  return createChart(container, {
    // width: container.clientWidth,
    // height: container.clientHeight,
    autoSize: true,
    layout: {
      background: { color: theme.bgColor },
      textColor: theme.textColor,
      fontFamily: 'Inter, system-ui, -apple-system, sans-serif',
      fontSize: 11,
      attributionLogo: false
    },
    grid: {
      vertLines: { visible: false },
      horzLines: { color: theme.gridColor, style: LineStyle.Dotted, visible: true }
    },
    crosshair: {
      mode: CrosshairMode.Normal,
      vertLine: { visible: false, labelVisible: false },
      horzLine: { visible: false, labelVisible: false }
    },
    rightPriceScale: {
      visible: !isMobileViewport.value,
      borderVisible: false,
      scaleMargins: { top: 0.1, bottom: 0.1 },
      autoScale: true,
      minimumWidth: isMobileViewport.value ? 0 : PRICE_SCALE_WIDTH
    },
    localization: {
      priceFormatter: (price: number) => `$${Math.round(price)}`
    },
    leftPriceScale: { visible: false, borderVisible: false },
    timeScale: {
      visible: false,
      borderVisible: false,
      fixLeftEdge: true,
      fixRightEdge: true
    },
    handleScroll: false,
    handleScale: false,
    kineticScroll: { touch: false, mouse: false },
    trackingMode: { exitMode: 0 }
  })
}

function createOscillatorChart(container: HTMLDivElement): IChartApi {
  const theme = chartTheme.value
  return createChart(container, {
    // width: container.clientWidth,
    // height: container.clientHeight,
    autoSize: true,
    layout: {
      background: { color: theme.bgColor },
      textColor: theme.textColor,
      fontFamily: 'Inter, system-ui, -apple-system, sans-serif',
      fontSize: 11,
      attributionLogo: false
    },
    grid: {
      vertLines: { visible: false },
      horzLines: { color: theme.gridColor, style: LineStyle.Dotted, visible: true }
    },
    crosshair: {
      mode: CrosshairMode.Normal,
      vertLine: { visible: false, labelVisible: false },
      horzLine: { visible: false, labelVisible: false }
    },
    rightPriceScale: {
      visible: !isMobileViewport.value,
      borderVisible: false,
      scaleMargins: { top: 0.12, bottom: 0.12 },
      autoScale: true,
      minimumWidth: isMobileViewport.value ? 0 : PRICE_SCALE_WIDTH
    },
    localization: {
      priceFormatter: (price: number) => {
        if (Math.abs(price) >= 1000000) {
          return `${(price / 1000000).toFixed(1)}M`
        } else if (Math.abs(price) >= 1000) {
          return `${(price / 1000).toFixed(1)}K`
        } else if (Math.abs(price) >= 100 || Number.isInteger(price)) {
          return Math.round(price).toString()
        } else if (Math.abs(price) >= 1) {
          return price.toFixed(1)
        } else {
          return price.toFixed(2)
        }
      }
    },
    leftPriceScale: { visible: false, borderVisible: false },
    timeScale: {
      visible: false,
      borderVisible: false,
      fixLeftEdge: true,
      fixRightEdge: true
    },
    handleScroll: false,
    handleScale: false,
    kineticScroll: { touch: false, mouse: false },
    trackingMode: { exitMode: 0 }
  })
}

function setupCandlestickSeries(chart: IChartApi, data: ChartData) {
  const upColor = ChartColors.StandardGreen
  const downColor = ChartColors.StandardRed

  candleSeries = chart.addSeries(CandlestickSeries, {
    upColor: upColor,
    downColor: downColor,
    borderUpColor: upColor,
    borderDownColor: downColor,
    wickUpColor: upColor,
    wickDownColor: downColor,
    priceLineVisible: false,
    lastValueVisible: false
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

function setupVolumeSeries(chart: IChartApi, data: ChartData) {
  const upVolumeColor = 'rgba(46, 125, 50, 0.4)'
  const downVolumeColor = 'rgba(221, 44, 0, 0.4)'

  volumeSeries = chart.addSeries(HistogramSeries, {
    priceFormat: { type: 'volume' },
    priceScaleId: '',
    priceLineVisible: false,
    lastValueVisible: false
  })

  // Position volume at bottom 30% of the chart (top: 0.7 means 70% from top, leaving bottom 30%)
  volumeSeries.priceScale().applyOptions({
    scaleMargins: { top: 0.8, bottom: 0 },
  })

  const volumeData = data.candles.map(c => ({
    time: parseTimestamp(c.timestamp),
    value: c.volume || 0,
    color: c.close >= c.open ? upVolumeColor : downVolumeColor
  }))

  volumeSeries.setData(volumeData)
}

function getLineStyle(style?: string): LineStyle {
  switch (style) {
    case 'dash':
      return LineStyle.Dashed
    case 'dots':
      return LineStyle.Dotted
    default:
      return LineStyle.Solid
  }
}

function setupIndicatorSeries(chart: IChartApi, seriesData: SeriesStyle[], isOscillator: boolean) {
  const targetArray = isOscillator ? oscillatorSeries : overlaySeries

  seriesData.forEach((seriesConfig, index) => {
    const color = seriesConfig.color || indicatorColors[index % indicatorColors.length]
    const lineWidth = seriesConfig.lineWidth || 2
    const lineStyle = getLineStyle(seriesConfig.lineStyle)

    let series: ISeriesApi<any>

    switch (seriesConfig.type) {
      case 'area':
        series = chart.addSeries(AreaSeries, {
          lineColor: color,
          topColor: `${color}40`,
          bottomColor: `${color}05`,
          lineWidth: lineWidth,
          priceLineVisible: false,
          lastValueVisible: false,
          crosshairMarkerVisible: false
        })
        break
      case 'histogram':
        series = chart.addSeries(HistogramSeries, {
          color: color,
          priceLineVisible: false,
          lastValueVisible: false
        })
        break
      case 'baseline':
        series = chart.addSeries(BaselineSeries, {
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
      case 'dots':
        series = chart.addSeries(LineSeries, {
          color: color,
          lineWidth: lineWidth,
          lineStyle: LineStyle.Dotted,
          priceLineVisible: false,
          lastValueVisible: false,
          crosshairMarkerVisible: false
        })
        break
      default:
        series = chart.addSeries(LineSeries, {
          color: color,
          lineWidth: lineWidth,
          lineStyle: lineStyle,
          priceLineVisible: false,
          lastValueVisible: false,
          crosshairMarkerVisible: false
        })
    }

    const filteredData = seriesConfig.data
      .filter(d => d.value !== null && d.value !== undefined && !isNaN(d.value))
      .map(d => {
        const point = {
          time: parseTimestamp(d.timestamp),
          value: d.value as number
        }
        // Preserve per-bar color if present (for conditional coloring like Elder-Ray)
        if (d.color) {
          point.color = d.color
        }
        return point
      })

    series.setData(filteredData)
    targetArray.push(series)
  })
}

function updateViewportWidth() {
  if (typeof window !== 'undefined') {
    viewportWidth.value = window.innerWidth
  }
}

function updatePriceScaleVisibility() {
  const visible = !isMobileViewport.value
  if (overlayChart) {
    overlayChart.priceScale('right').applyOptions({ visible })
  }
  if (oscillatorChart) {
    oscillatorChart.priceScale('right').applyOptions({ visible })
  }
}

async function initChart() {
  isLoading.value = true
  hasError.value = false

  const data = await loadChartData()
  if (!data) {
    isLoading.value = false
    return
  }

  // Determine chart type from data
  const isOscillatorType = data.metadata?.chartType === 'oscillator'

  // Hide loading BEFORE creating chart so container becomes visible.
  // This is critical because v-show hides the container while loading,
  // and clientWidth is 0 when the container is hidden.
  isLoading.value = false
  chartType.value = isOscillatorType ? 'oscillator' : 'overlay'

  // Wait for Vue to update the DOM after state changes.
  // Two requestAnimationFrame calls ensure: (1) Vue processes the reactive update,
  // and (2) the browser completes layout/paint so clientWidth is accurate.
  await new Promise(resolve => requestAnimationFrame(resolve))
  await new Promise(resolve => requestAnimationFrame(resolve))

  // Wait for container to have a valid width (may take a few frames)
  // Check the appropriate container based on chart type
  const containerToCheck = isOscillatorType ? oscillatorChartContainer.value : overlayChartContainer.value
  let attempts = 0
  while (containerToCheck && containerToCheck.clientWidth === 0 && attempts < INIT_POLL_MAX_ATTEMPTS) {
    await new Promise(resolve => setTimeout(resolve, INIT_POLL_INTERVAL_MS))
    attempts++
  }

  // For overlay indicators, create overlay chart with candlesticks
  if (!isOscillatorType && overlayChartContainer.value && overlayChartContainer.value.clientWidth > 0) {
    overlayChart = createOverlayChart(overlayChartContainer.value)
    setupCandlestickSeries(overlayChart, data)
    setupVolumeSeries(overlayChart, data)

    // For overlay indicators, add series to overlay chart
    if (data.series.length > 0) {
      setupIndicatorSeries(overlayChart, data.series, false)
    }

    overlayChart.timeScale().fitContent()
  }

  // For oscillator indicators, create separate oscillator chart
  if (isOscillatorType && oscillatorChartContainer.value && oscillatorChartContainer.value.clientWidth > 0) {
    oscillatorChart = createOscillatorChart(oscillatorChartContainer.value)

    // Store thresholds info for later use with indicator data
    const thresholds = data.metadata?.thresholds || []

    // Add threshold lines first (behind indicator data)
    for (const threshold of thresholds) {
      // Add threshold line
      const series = oscillatorChart.addSeries(LineSeries, {
        color: threshold.color,
        lineWidth: 2 as LineWidth,
        lineStyle: threshold.style === 'dash' ? LineStyle.Dashed : LineStyle.Solid,
        priceLineVisible: false,
        lastValueVisible: false,
        crosshairMarkerVisible: false
      })

      // Create horizontal line across all candle timestamps
      const thresholdData = data.candles.map(c => ({
        time: parseTimestamp(c.timestamp),
        value: threshold.value
      }))
      series.setData(thresholdData)
    }

    // Add threshold zone fills using baseline series with indicator data
    // This creates colored fills when indicator exceeds threshold values
    // Supports bidirectional fills (fillAbove/fillBelow) for richer visualizations
    if (data.series.length > 0) {
      const indicatorData = data.series[0].data
        .filter(d => d.value !== null && d.value !== undefined && !isNaN(d.value))
        .map(d => ({
          time: parseTimestamp(d.timestamp),
          value: d.value as number
        }))

      // Create fills for each threshold using BaselineSeries
      for (const threshold of thresholds) {
        if (threshold.fill && threshold.fillColor) {
          const baselineSeries = oscillatorChart.addSeries(BaselineSeries, {
            baseValue: { type: 'price', price: threshold.value },
            topLineColor: 'transparent',
            topFillColor1: threshold.fill === 'above' ? threshold.fillColor : 'transparent',
            topFillColor2: threshold.fill === 'above' ? threshold.fillColor : 'transparent',
            bottomLineColor: 'transparent',
            bottomFillColor1: threshold.fill === 'below' ? threshold.fillColor : 'transparent',
            bottomFillColor2: threshold.fill === 'below' ? threshold.fillColor : 'transparent',
            priceLineVisible: false,
            lastValueVisible: false,
            crosshairMarkerVisible: false
          })
          baselineSeries.setData(indicatorData)
        }
      }
    }

    // Add oscillator series
    setupIndicatorSeries(oscillatorChart, data.series, true)
    oscillatorChart.timeScale().fitContent()
  }

  // Setup resize observer with debouncing to handle container resizing
  const observerContainer = isOscillatorType ? oscillatorChartContainer.value : overlayChartContainer.value
  if (observerContainer) {
    resizeObserver = new ResizeObserver(entries => {
      // Debounce resize events to avoid excessive updates during rapid resizing
      if (resizeTimeout) {
        clearTimeout(resizeTimeout)
      }
      resizeTimeout = setTimeout(() => {
        // Use the last entry since ResizeObserver batches multiple changes
        const entry = entries[entries.length - 1]
        const width = entry.contentRect.width
        if (width > 0) {
          if (overlayChart && overlayChartContainer.value) {
            overlayChart.resize(width, overlayChartContainer.value.clientHeight)
          }
          if (oscillatorChart && oscillatorChartContainer.value) {
            oscillatorChart.resize(width, oscillatorChartContainer.value.clientHeight)
          }
        }
      }, RESIZE_DEBOUNCE_MS)
    })
    resizeObserver.observe(observerContainer)
  }
}

function destroyChart() {
  if (resizeTimeout) {
    clearTimeout(resizeTimeout)
    resizeTimeout = null
  }
  if (resizeObserver) {
    resizeObserver.disconnect()
    resizeObserver = null
  }
  if (overlayChart) {
    overlayChart.remove()
    overlayChart = null
    candleSeries = null
    volumeSeries = null
    overlaySeries.length = 0
  }
  if (oscillatorChart) {
    oscillatorChart.remove()
    oscillatorChart = null
    oscillatorSeries.length = 0
  }
}

onMounted(() => {
  if (typeof window !== 'undefined') {
    updateViewportWidth()
    window.addEventListener('resize', updateViewportWidth)
    initChart()
  }
})

onUnmounted(() => {
  if (typeof window !== 'undefined') {
    window.removeEventListener('resize', updateViewportWidth)
  }
  destroyChart()
})

watch(() => props.src, () => {
  destroyChart()
  initChart()
})

// Watch for theme changes and reinitialize charts
watch(isDark, () => {
  destroyChart()
  initChart()
})

// Watch for viewport width changes and update price scale visibility
watch(isMobileViewport, () => {
  updatePriceScaleVisibility()
  // Reload chart data to adjust number of bars shown
  destroyChart()
  initChart()
})
</script>

<template>
  <div class="indicator-chart-wrapper">
    <div v-if="isLoading" class="chart-loading">
      <span class="loading-spinner"></span>
      <span>Loading chart...</span>
    </div>

    <div v-else-if="hasError" class="chart-error">
      <span>{{ errorMessage }}</span>
    </div>

    <div v-show="!isLoading && !hasError" class="charts-container">
      <!-- Overlay Chart (shown for overlay type indicators) -->
      <div v-if="chartType === 'overlay'" ref="overlayChartContainer" class="chart-container overlay-chart" role="img"
        aria-label="Price chart with indicator overlay"></div>

      <!-- Oscillator Chart (shown only for oscillator type) -->
      <div v-if="chartType === 'oscillator'" ref="oscillatorChartContainer" class="chart-container oscillator-chart"
        role="img" aria-label="Oscillator indicator chart"></div>
    </div>

    <noscript>
      <div class="chart-noscript">
        <p>JavaScript is required to display interactive charts.</p>
      </div>
    </noscript>
  </div>
</template>

<style scoped lang="scss">
// Breakpoints aligned with project standards
// See: docs/.vitepress/public/assets/css/style.scss
$large-breakpoint: 1024px;
$medium-breakpoint: 768px;
$small-breakpoint: 480px; // Mobile breakpoint
$landscape-height-sm: 400px;
$landscape-height-md: 600px;

.indicator-chart-wrapper {
  width: 100%;

  .charts-container {
    display: flex;
    flex-direction: column;
    gap: 0;
    text-align: center;
  }

  .chart-container {
    width: 100%;
    overflow: hidden;
    padding-left: 5px;

    /* Hide the TradingView attribution link via CSS as backup */
    :deep(a[href*="tradingview"]) {
      display: none !important;
    }
  }

  .overlay-chart {
    aspect-ratio: 2.5;

    /* Medium breakpoint (768px-1024px) */
    @media (max-width: $large-breakpoint) {
      aspect-ratio: 2.5;
    }

    /* Mobile breakpoint (<480px) - matches JavaScript isMobileViewport */
    @media (max-width: $small-breakpoint) {
      aspect-ratio: 5/4;
    }

    /* Landscape optimizations */
    @media (max-height: $landscape-height-sm) and (orientation: landscape) {
      aspect-ratio: unset;
      height: 100vh;
    }
  }

  .oscillator-chart {
    aspect-ratio: 7;

    /* Medium breakpoint (768px-1024px) */
    @media (max-width: $large-breakpoint) {
      aspect-ratio: 7;
    }

    /* Landscape optimizations */
    @media (max-width: $large-breakpoint) and (orientation: landscape) {
      aspect-ratio: unset;
      height: 25vh;
    }

    @media (max-height: $landscape-height-md) and (orientation: landscape) {
      aspect-ratio: unset;
      height: 33.33vh;
    }

    @media (max-height: $landscape-height-sm) {
      aspect-ratio: unset;
      height: 50vh;
    }
  }

  .chart-loading,
  .chart-error {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 0.5rem;
    background-color: var(--vp-c-bg);
    border: none;
    color: var(--vp-c-text-2);
  }

  .chart-error {
    color: var(--vp-c-danger-1);
  }

  .loading-spinner {
    width: 24px;
    height: 24px;
    border: 2px solid var(--vp-c-divider);
    border-top-color: var(--vp-c-brand-1);
    border-radius: 50%;
    animation: spin 1s linear infinite;
  }

  .chart-noscript {
    padding: 2rem;
    text-align: center;
    background-color: var(--vp-c-bg);
    border: none;
    color: var(--vp-c-text-2);
  }
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

/* Chart library overrides (unset VitePress table styles) */
:deep(.tv-lightweight-charts) {

  table,
  tr,
  td,
  th {
    border: none;
    margin: 0;
    overflow: unset;
  }
}
</style>
