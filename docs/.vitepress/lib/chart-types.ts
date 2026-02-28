export interface ThresholdLine {
  value: number
  color: string
  style?: 'solid' | 'dash'
  fill?: 'above' | 'below'
  fillColor?: string
}

export interface SeriesStyle {
  name: string
  type?: 'line' | 'area' | 'histogram' | 'baseline' | 'dots' | 'pointer'
  color?: string
  lineWidth?: number
  lineStyle?: 'solid' | 'dash' | 'dots'
  data: Array<{
    timestamp: string
    value: number | null
    color?: string
  }>
}

export interface ChartData {
  metadata?: {
    symbol?: string
    timeframe?: string
    indicator?: string
    chartType?: 'overlay' | 'oscillator' | 'candles'
    timeScale?: 'linear' | 'nonLinear'
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

export interface TimeContext {
  candleTimes: Array<string | number>
  createResolveSeriesTime: () => (timestamp: string, index: number) => string | number
}
