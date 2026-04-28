import { Chart, registerables } from "chart.js";
import "chartjs-adapter-date-fns";
import annotationPlugin from "chartjs-plugin-annotation";
import { registerFinancialCharts } from "@facioquo/chartjs-chart-financial";
let initialized = false;
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
export function setupIndyCharts() {
    if (initialized)
        return;
    Chart.register(...registerables, annotationPlugin);
    registerFinancialCharts();
    initialized = true;
}
//# sourceMappingURL=setup.js.map