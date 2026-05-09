import type { ChartData, TimeContext } from './chart-types'

export function parseTimestamp(timestamp: string): string {
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

function usesNonLinearTime(data: ChartData): boolean {
  if (data.metadata?.timeScale === 'nonLinear') {
    return true
  }

  const seen = new Set<string>()
  let lastTimestamp = Number.NEGATIVE_INFINITY

  for (const candle of data.candles) {
    const normalized = parseTimestamp(candle.timestamp)
    if (seen.has(normalized)) {
      return true
    }
    seen.add(normalized)

    const numericTime = new Date(normalized).getTime()
    if (!Number.isNaN(numericTime)) {
      if (numericTime < lastTimestamp) {
        return true
      }
      lastTimestamp = numericTime
    }
  }

  return false
}

export function createTimeContext(data: ChartData): TimeContext {
  const nonLinear = usesNonLinearTime(data)

  if (!nonLinear) {
    return {
      candleTimes: data.candles.map(c => parseTimestamp(c.timestamp)),
      createResolveSeriesTime: () => (timestamp: string) => parseTimestamp(timestamp)
    }
  }

  const candleTimes = data.candles.map((_, index) => index)
  const timeIndexLookup = new Map<string, number[]>()

  data.candles.forEach((candle, index) => {
    const key = parseTimestamp(candle.timestamp)
    if (!timeIndexLookup.has(key)) {
      timeIndexLookup.set(key, [])
    }
    timeIndexLookup.get(key)?.push(index)
  })

  return {
    candleTimes,
    createResolveSeriesTime: () => {
      const timeIndexOffsets = new Map<string, number>()
      return (timestamp: string, index: number) => {
        const key = parseTimestamp(timestamp)
        const indices = timeIndexLookup.get(key)
        if (!indices || indices.length === 0) {
          return index
        }
        const offset = timeIndexOffsets.get(key) ?? 0
        const resolved = indices[offset]
        if (resolved === undefined) {
          return index
        }
        timeIndexOffsets.set(key, offset + 1)
        return resolved
      }
    }
  }
}
