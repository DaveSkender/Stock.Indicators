import { Chart } from "chart.js";
import { setFinancialDefaults } from "./helpers/defaults";
export function applyFinancialElementTheme(palette) {
    setFinancialDefaults(palette);
    const candleDefaults = Chart.defaults.elements;
    candleDefaults.candlestick.color = { ...palette.candle };
    candleDefaults.candlestick.borderColor = { ...palette.candleBorder };
}
/** Applies baseline performance-safe financial chart options. */
export function buildFinancialChartOptions(base) {
    const options = {
        ...base,
        plugins: {
            ...base.plugins,
            tooltip: base.plugins?.tooltip ? { ...base.plugins.tooltip } : undefined
        }
    };
    options.animation = false;
    if (options.plugins?.tooltip) {
        options.plugins.tooltip.intersect = false;
    }
    return options;
}
//# sourceMappingURL=options.js.map