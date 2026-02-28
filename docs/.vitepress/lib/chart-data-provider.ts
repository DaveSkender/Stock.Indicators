import type { ChartData } from './chart-types'

const CACHE_PREFIX = 'si-docs-chart:'
const DEFAULT_TTL_MS = 60 * 60 * 1000
const memoryCache = new Map<string, { expiresAt: number, data: ChartData }>()

interface ChartProviderOptions {
  src?: string
  indicatorKey?: string
  maxBars?: number
}

interface CachePayload {
  expiresAt: number
  data: ChartData
}

function resolveSource(options: ChartProviderOptions): { localUrl: string, liveUrl?: string } {
  const localUrl = options.src || (options.indicatorKey ? `/data/${options.indicatorKey}.json` : '')
  const liveBase = import.meta.env.VITE_CHART_DATA_BASE_URL as string | undefined

  if (!liveBase || !options.indicatorKey) {
    return { localUrl }
  }

  const normalizedBase = liveBase.replace(/\/$/, '')
  return {
    localUrl,
    liveUrl: `${normalizedBase}/${options.indicatorKey}.json`
  }
}

function getCacheTtlMs(): number {
  const raw = Number(import.meta.env.VITE_CHART_CACHE_TTL_MS)
  if (!Number.isFinite(raw) || raw <= 0) {
    return DEFAULT_TTL_MS
  }
  return raw
}

function getCacheKey(indicatorKey?: string): string | null {
  return indicatorKey ? `${CACHE_PREFIX}${indicatorKey}` : null
}

function readStorageCache(cacheKey: string): CachePayload | null {
  if (typeof window === 'undefined') {
    return null
  }

  try {
    const raw = window.localStorage.getItem(cacheKey)
    if (!raw) {
      return null
    }
    const parsed = JSON.parse(raw) as CachePayload
    if (!parsed.expiresAt || !parsed.data || Date.now() > parsed.expiresAt) {
      window.localStorage.removeItem(cacheKey)
      return null
    }
    return parsed
  } catch {
    // SecurityError (private/restrictive mode), QuotaExceededError, or JSON parse failure
    try { window.localStorage.removeItem(cacheKey) } catch { /* ignore */ }
    return null
  }
}

function writeStorageCache(cacheKey: string, payload: CachePayload) {
  if (typeof window === 'undefined') {
    return
  }

  try {
    window.localStorage.setItem(cacheKey, JSON.stringify(payload))
  } catch {
    // Ignore storage errors (quota/private mode)
  }
}


function sliceData(data: ChartData, maxBars: number): ChartData {
  const result = { ...data, candles: data.candles.slice(-maxBars) }
  if (data.series) {
    result.series = data.series.map(s => ({
      ...s,
      data: s.data.slice(-maxBars)
    }))
  }
  return result
}

async function fetchJson(url: string): Promise<ChartData> {
  const controller = new AbortController()
  const timeoutId = setTimeout(() => controller.abort(), 8000)
  try {
    const response = await fetch(url, { signal: controller.signal })
    if (!response.ok) {
      throw new Error(`Failed to load chart data: ${response.status}`)
    }
    return await response.json() as ChartData
  } finally {
    clearTimeout(timeoutId)
  }
}

export async function getChartData(options: ChartProviderOptions): Promise<ChartData> {
  const normalizedMaxBars =
    Number.isFinite(options.maxBars) && options.maxBars! > 0
      ? Math.floor(options.maxBars!)
      : 100
  const cacheKey = getCacheKey(options.indicatorKey)

  if (cacheKey) {
    const memory = memoryCache.get(cacheKey)
    if (memory && Date.now() <= memory.expiresAt) {
      return sliceData(structuredClone(memory.data), normalizedMaxBars)
    }

    const storage = readStorageCache(cacheKey)
    if (storage) {
      memoryCache.set(cacheKey, storage)
      return sliceData(structuredClone(storage.data), normalizedMaxBars)
    }
  }

  const { localUrl, liveUrl } = resolveSource(options)
  if (!localUrl && !liveUrl) {
    throw new Error('No chart data source configured')
  }

  let data: ChartData | null = null
  if (liveUrl) {
    try {
      data = await fetchJson(liveUrl)
    } catch {
      // Fallback to local static data on live API failure.
    }
  }

  if (!data && localUrl) {
    data = await fetchJson(localUrl)
  }

  if (!data) {
    throw new Error('Failed to load chart data from live or local source')
  }

  if (cacheKey) {
    const payload = {
      data,
      expiresAt: Date.now() + getCacheTtlMs()
    }
    memoryCache.set(cacheKey, payload)
    writeStorageCache(cacheKey, payload)
  }

  return sliceData(data, normalizedMaxBars)

}
