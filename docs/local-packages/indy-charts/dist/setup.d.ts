import "chartjs-adapter-date-fns";
/**
 * One-time setup: registers all Chart.js components required by indy-charts.
 *
 * Call this once before creating any chart instances.  Safe to call multiple
 * times — subsequent calls are no-ops.
 *
 * Registers:
 * - All core Chart.js components (`registerables`)
 * - The chartjs-plugin-annotation plugin
 * - The financial chart types (candlestick, OHLC, volume) from
 *   `@facioquo/chartjs-chart-financial`
 * - The `chartjs-adapter-date-fns` date adapter (side-effect import)
 */
export declare function setupIndyCharts(): void;
//# sourceMappingURL=setup.d.ts.map