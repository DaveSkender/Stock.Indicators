/*
 * Derived from chartjs-chart-financial (https://github.com/chartjs/chartjs-chart-financial)
 * Version reference: upstream plugin v0.2.x API surface.
 * License: MIT.
 */
import { Chart } from "chart.js";
import { merge } from "chart.js/helpers";
const defaultPalette = {
    candle: {
        up: "rgba(80, 160, 115, 1)",
        down: "rgba(215, 85, 65, 1)",
        unchanged: "rgba(90, 90, 90, 1)"
    },
    candleBorder: {
        up: "rgba(80, 160, 115, 1)",
        down: "rgba(215, 85, 65, 1)",
        unchanged: "rgba(90, 90, 90, 1)"
    },
    volume: {
        up: "rgba(80, 160, 115, 0.35)",
        down: "rgba(215, 85, 65, 0.35)",
        unchanged: "rgba(90, 90, 90, 0.35)"
    }
};
export function setFinancialDefaults(palette = defaultPalette) {
    const chartDefaults = Chart.defaults;
    const elementDefaults = Chart.defaults.elements;
    chartDefaults.financial = {
        color: { ...palette.candle }
    };
    elementDefaults.financial = {
        color: { ...palette.candle }
    };
    elementDefaults.ohlc = merge({}, [
        elementDefaults.financial,
        { lineWidth: 2, armLength: null, armLengthRatio: 0.8 }
    ]);
    elementDefaults.candlestick = merge({}, [
        elementDefaults.financial,
        {
            borderColor: { ...palette.candleBorder },
            borderWidth: 1
        }
    ]);
}
//# sourceMappingURL=defaults.js.map