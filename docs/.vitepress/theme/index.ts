// .vitepress/theme/index.ts
import { h } from 'vue'
import type { Theme } from 'vitepress'
import DefaultTheme from 'vitepress/theme'
import './custom.scss'
import Contributors from '../components/Contributors.vue'
import IndicatorChartPanel from '../components/IndicatorChartPanel.vue'
import { setupIndyChartsForVitePress } from '@facioquo/indy-charts/vitepress'

export default {
  extends: DefaultTheme,
  Layout: () => {
    return h(DefaultTheme.Layout, null, {
      // https://vitepress.dev/guide/extending-default-theme#layout-slots
      'nav-bar-title-after': () => h('span', { class: 'nav-title-below' }, 'for .NET')
    })
  },
  enhanceApp({ app }) {
    setupIndyChartsForVitePress(app, {
      api: { baseUrl: 'https://stock-charts-api.azurewebsites.net' },
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
