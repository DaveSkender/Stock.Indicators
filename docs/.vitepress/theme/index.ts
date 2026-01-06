// .vitepress/theme/index.ts
import { h } from 'vue'
import type { Theme } from 'vitepress'
import DefaultTheme from 'vitepress/theme'
import './custom.scss'
import IndicatorChart from '../components/IndicatorChart.vue'
import Contributors from '../components/Contributors.vue'

export default {
  extends: DefaultTheme,
  Layout: () => {
    return h(DefaultTheme.Layout, null, {
      // https://vitepress.dev/guide/extending-default-theme#layout-slots
      'nav-bar-title-after': () => h('span', { class: 'nav-title-below' }, 'for .NET')
    })
  },
  enhanceApp({ app }) {
    // Register global components
    app.component('IndicatorChart', IndicatorChart)
    app.component('Contributors', Contributors)
  }
} satisfies Theme
