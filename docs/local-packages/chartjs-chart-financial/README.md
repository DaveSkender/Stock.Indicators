# @facioquo/chartjs-chart-financial

Financial charting extension for Chart.js. Provides candlestick, OHLC, and volume chart types.

This is a modernized Chart.js financial plugin based on the original [chartjs-chart-financial](https://github.com/chartjs/chartjs-chart-financial) project.

## Features

- **Candlestick charts**: Classic Japanese candlestick visualization
- **OHLC charts**: Open-High-Low-Close bar charts
- **Volume charts**: Trading volume bars with color coding
- **TypeScript support**: Full type definitions included
- **Theme support**: Light and dark theme color palettes
- **Chart.js v4.5+**: Built for the latest Chart.js version

## Installation

```bash
npm install @facioquo/chartjs-chart-financial chart.js
```

## Quick Start

```typescript
import { Chart, registerables } from "chart.js";
import { registerFinancialCharts } from "@facioquo/chartjs-chart-financial";

// Register Chart.js and financial chart types
Chart.register(...registerables);
registerFinancialCharts();

// Create a candlestick chart
const chart = new Chart(ctx, {
  type: "candlestick",
  data: {
    datasets: [
      {
        label: "Stock Price",
        data: [
          { x: "2024-01-01", o: 100, h: 110, l: 95, c: 105 },
          { x: "2024-01-02", o: 105, h: 115, l: 103, c: 112 }
          // ... more data points
        ]
      }
    ]
  }
});
```

## Chart Types

### Candlestick

```typescript
const chart = new Chart(ctx, {
  type: "candlestick",
  data: {
    datasets: [
      /* ... */
    ]
  }
});
```

### OHLC

```typescript
const chart = new Chart(ctx, {
  type: "ohlc",
  data: {
    datasets: [
      /* ... */
    ]
  }
});
```

### Volume

Use the `buildVolumeDataset()` helper to create properly configured volume datasets.

## API Reference

### Registration

- `registerFinancialCharts()`: Register all financial chart types with Chart.js
- `financialRegisterables`: Array of registerables for manual registration

### Factory Functions

- `buildCandlestickDataset(priceData, borderColor)`: Create a candlestick dataset from normalized OHLC points
- `buildVolumeDataset(quotes, extraBars, palette)`: Create a volume dataset with up/down/unchanged candle coloring
- `buildFinancialChartOptions(base)`: Wrap base chart options with financial-safe defaults (animations off, non-intersecting tooltip)
- `applyFinancialElementTheme(palette)`: Apply a `FinancialPalette` to Chart.js global element defaults
- `toFinancialDataPoint(quote)`: Convert a quote-like object to a `FinancialDataPoint`

### Theme

- `COLORS`: Color constants for financial charts
- `getCandleColor(open, close, palette)`: Determine candle color based on price movement
- `getVolumeColor(open, close, palette)`: Determine volume bar color based on price movement
- `getFinancialPalette(mode)`: Get color palette for `'light'` or `'dark'` theme

## Types

Full TypeScript definitions are included:

- `FinancialDataPoint`: OHLC data structure
- `FinancialDatasetOptions`: Dataset configuration options
- `FinancialPalette`: Color palette definition
- `FinancialThemeMode`: 'light' | 'dark'

## License

Apache-2.0 License - see LICENSE file for details.

## Credits

Based on the original [chartjs-chart-financial](https://github.com/chartjs/chartjs-chart-financial) project by the Chart.js team.
