// .vitepress/theme/index.ts
import { h } from 'vue'
import type { Theme } from 'vitepress'
import DefaultTheme from 'vitepress/theme'
import './custom.scss'
import Contributors from '../components/Contributors.vue'
import IndicatorChartPanel from '../components/IndicatorChartPanel.vue'
import { setupIndyChartsForVitePress } from '@facioquo/indy-charts/vitepress'

const STOCK_CHARTS_API_BASE_URL = 'https://stock-charts-api.azurewebsites.net'
const STOCK_CHARTS_API_HOST = new URL(STOCK_CHARTS_API_BASE_URL).hostname
const CHART_API_FALLBACK_PATH = '/data/chart-api'

function fallbackUrl(url: URL): string | undefined {
  if (url.pathname === '/quotes') return `${CHART_API_FALLBACK_PATH}/quotes.json`
  if (url.pathname === '/indicators') return `${CHART_API_FALLBACK_PATH}/indicators.json`
  if (url.pathname.startsWith('/indicators/sma')) return `${CHART_API_FALLBACK_PATH}/sma.json`
  if (url.pathname.startsWith('/indicators/rsi')) return `${CHART_API_FALLBACK_PATH}/rsi.json`

  return undefined
}

function emptyIndicatorData(): Response {
  return new Response('[]', {
    headers: {
      'content-type': 'application/json'
    }
  })
}

// In dev, the Vite server proxy at /chart-api-proxy forwards requests to the
// stock-charts API server-side, avoiding CORS restrictions. We detect dev mode
// by checking for the proxy path having been configured (import.meta.env.DEV).
const DEV_PROXY_PATH = '/chart-api-proxy'

function installStockChartsApiFallback(): void {
  if (typeof window === 'undefined' || typeof window.fetch !== 'function') return

  const originalFetch = window.fetch.bind(window)

  window.fetch = async (input: RequestInfo | URL, init?: RequestInit): Promise<Response> => {
    const requestUrl = new URL(
      input instanceof Request ? input.url : String(input),
      window.location.href
    )
    if (requestUrl.hostname !== STOCK_CHARTS_API_HOST) {
      return originalFetch(input, init)
    }

    const fetchTarget =
      import.meta.env.DEV
        ? `${DEV_PROXY_PATH}${requestUrl.pathname}${requestUrl.search}`
        : input

    try {
      const response = await originalFetch(fetchTarget, init)
      if (response.ok) return response
    } catch {
      // fall through to static fixture fallback
    }

    const fallback = fallbackUrl(requestUrl)
    if (fallback) {
      return originalFetch(fallback, init)
    }

    if (requestUrl.pathname.startsWith('/indicators/')) {
      return emptyIndicatorData()
    }

    return originalFetch(input, init)
  }
}

export default {
  extends: DefaultTheme,
  Layout: () => {
    return h(DefaultTheme.Layout, null, {
      // https://vitepress.dev/guide/extending-default-theme#layout-slots
      'nav-bar-title-after': () => h('span', { class: 'nav-title-below' }, 'for .NET')
    })
  },
  enhanceApp({ app }) {
    installStockChartsApiFallback()

    setupIndyChartsForVitePress(app, {
      api: { baseUrl: STOCK_CHARTS_API_BASE_URL },
      defaults: {
        barCount: 250,
        quoteCount: 250,
        showTooltips: true
      }
    })

    // Register global components
    app.component('Contributors', Contributors)
    app.component('IndicatorChartPanel', IndicatorChartPanel)
  }
} satisfies Theme
