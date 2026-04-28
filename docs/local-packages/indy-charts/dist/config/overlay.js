import { baseChartOptions } from "./common";
export function baseOverlayConfig(volumeAxisSize, settings) {
    // Root chart type changed from "line" to "candlestick" so Chart.js
    // calculates y-axis bounds from OHLC values; using "line" caused empty
    // overlay (scale collapsed in production build).
    const config = {
        type: "candlestick",
        data: {
            datasets: []
        },
        options: baseOverlayOptions(volumeAxisSize, settings)
    };
    return config;
}
export function baseOverlayOptions(volumeAxisSize, settings) {
    const options = baseChartOptions(settings);
    options.scales ?? (options.scales = {});
    const y = options.scales["y"];
    if (y) {
        y.ticks.callback = (value, index, values) => {
            if (index === 0 || index === values.length - 1)
                return null;
            return "$" + value;
        };
    }
    // define secondary y-axis for volume
    options.scales["volumeAxis"] = {
        display: false,
        type: "linear",
        axis: "y",
        position: "left",
        beginAtZero: true,
        padding: 0,
        border: {
            display: false
        },
        max: volumeAxisSize
    };
    return options;
}
//# sourceMappingURL=overlay.js.map