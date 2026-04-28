# @facioquo/indy-charts

Framework-agnostic financial charting library with technical indicators and stock market data visualization built on Chart.js.

## Installation

```bash
npm install @facioquo/indy-charts chart.js chartjs-adapter-date-fns chartjs-plugin-annotation date-fns
```

## Quick Start

```typescript
import { createApiClient, OverlayChart, setupIndyCharts } from "@facioquo/indy-charts";

setupIndyCharts();

const client = createApiClient({
  baseUrl: "https://api.example.com"
});

const quotes = await client.getQuotes();

const canvas = document.getElementById("main-chart");
if (!(canvas instanceof HTMLCanvasElement)) {
  throw new Error("Chart canvas not found");
}

const chart = new OverlayChart(canvas, {
  isDarkTheme: false,
  showTooltips: true
});

chart.render(quotes.slice(-250));
```

## Usage with VitePress

Register the optional VitePress adapter once in `.vitepress/theme/index.ts`:

```typescript
import { setupIndyChartsForVitePress } from "@facioquo/indy-charts/vitepress";

export default {
  enhanceApp({ app }) {
    setupIndyChartsForVitePress(app, {
      api: { baseUrl: "https://api.example.com" },
      defaults: { barCount: 250, quoteCount: 250, showTooltips: true },
      indicators: {
        rsi: { uiid: "RSI", params: { lookbackPeriods: 14 }, results: ["rsi"] }
      }
    });
  }
};
```

Then use the global component from Markdown:

```vue
<ClientOnly>
  <StockIndicatorChart indicator="rsi" />
</ClientOnly>
```

## License

Apache-2.0 License - see the repository LICENSE file for details.

## Related Projects

- [@facioquo/chartjs-chart-financial](../chartjs-financial) - Chart.js financial chart types
- [stock-charts](https://github.com/facioquo/stock-charts) - Full-featured Angular application
