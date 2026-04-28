import { baseChartOptions } from "./common";
export function baseOscillatorConfig(settings) {
    const config = {
        type: "line",
        data: {
            datasets: []
        },
        options: baseOscillatorOptions(settings)
    };
    return config;
}
export function baseOscillatorOptions(settings) {
    const options = baseChartOptions(settings);
    // remove x-axis
    if (options.scales?.["x"]) {
        options.scales["x"].display = false;
    }
    // format y-axis (helper context)
    if (!options.scales || !options.scales["y"])
        return options;
    const y = options.scales["y"];
    // rescale labels
    y.ticks.callback = (value, index, values) => {
        const numValue = typeof value === "string" ? parseFloat(value) : value;
        const v = Math.abs(numValue);
        // remove first and last y-axis labels
        if (index === 0 || index === values.length - 1)
            return null;
        // otherwise, condense large/small display values
        else if (v > 10000000000)
            return Math.trunc(numValue / 1000000000) + "B";
        else if (v > 10000000)
            return Math.trunc(numValue / 1000000) + "M";
        else if (v > 10000)
            return Math.trunc(numValue / 1000) + "K";
        else if (v > 10)
            return Math.trunc(numValue);
        else if (v > 0.001)
            return Math.round((numValue + Number.EPSILON) * 100000) / 100000;
        else if (v > 0)
            return numValue.toExponential(2);
        else
            return Math.round((numValue + Number.EPSILON) * 100000000) / 100000000;
    };
    return options;
}
//# sourceMappingURL=oscillator.js.map