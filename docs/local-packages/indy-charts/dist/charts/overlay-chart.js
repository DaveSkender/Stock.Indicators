import { Chart } from "chart.js";
import { applyFinancialElementTheme, buildCandlestickDataset, buildFinancialChartOptions, buildVolumeDataset, getFinancialPalette } from "@facioquo/chartjs-chart-financial";
import { processQuoteData } from "../data/transformers";
import { baseOverlayConfig, baseOverlayOptions } from "../config/overlay";
import { commonLegendAnnotation } from "../config/annotations";
const CHART_TYPES = {
    OVERLAY: "overlay"
};
export class OverlayChart {
    constructor(ctx, settings) {
        this.ctx = ctx;
        this.settings = settings;
        this.volumeAxisSize = 0;
    }
    /**
     * Initialize the overlay chart with quote data.
     * Returns the full-resolution datasets for use in dynamic slicing.
     */
    render(quotes, extraBars = 7) {
        const palette = getFinancialPalette(this.settings.isDarkTheme ? "dark" : "light");
        applyFinancialElementTheme(palette);
        const { priceData, volumeAxisSize } = processQuoteData(quotes);
        this.volumeAxisSize = volumeAxisSize;
        const chartData = {
            datasets: [
                buildCandlestickDataset(priceData, palette.candleBorder),
                buildVolumeDataset(quotes, extraBars, palette)
            ]
        };
        const chartConfig = baseOverlayConfig(volumeAxisSize, this.settings);
        chartConfig.options = buildFinancialChartOptions(chartConfig.options ?? {});
        chartConfig.data = chartData;
        if (this.chart)
            this.chart.destroy();
        this.chart = new Chart(this.ctx, chartConfig);
        // Return deep copy of datasets for dynamic slicing (structuredClone preserves NaN)
        return structuredClone(chartData.datasets);
    }
    addIndicatorDatasets(results) {
        if (!this.chart)
            return;
        const chart = this.chart;
        results.forEach((r) => {
            chart.data.datasets.push(r.dataset);
        });
        this.chart.update("none");
    }
    removeIndicatorDatasets(results) {
        if (!this.chart)
            return;
        const chart = this.chart;
        results.forEach((result) => {
            const dx = chart.data.datasets.indexOf(result.dataset, 0);
            if (dx !== -1) {
                chart.data.datasets.splice(dx, 1);
            }
        });
        this.chart.update("none");
    }
    updateLegends(overlaySelections) {
        if (!this.chart)
            return;
        if (!this.chart.scales["x"] || !this.chart.scales["y"])
            return;
        const xPos = this.chart.scales["x"].min;
        const yPos = this.chart.scales["y"].max;
        let adjY = 10;
        const sorted = [...overlaySelections]
            .filter(x => x.chartType === CHART_TYPES.OVERLAY)
            .sort((a, b) => a.label.localeCompare(b.label));
        if (this.chart.options?.plugins?.annotation) {
            this.chart.options.plugins.annotation.annotations = sorted
                .filter(selection => selection.results.length > 0 && selection.results[0] !== undefined)
                .map((selection, index) => {
                const annotation = commonLegendAnnotation(selection.label, xPos, yPos, adjY, this.settings);
                annotation.id = "legend" + (index + 1).toString();
                annotation.color = selection.results[0].color;
                adjY += 15; // LEGEND_Y_OFFSET
                return annotation;
            });
            this.chart.update("none");
        }
    }
    updateTheme(settings) {
        this.settings = settings;
        if (!this.chart)
            return;
        applyFinancialElementTheme(getFinancialPalette(settings.isDarkTheme ? "dark" : "light"));
        this.chart.options = buildFinancialChartOptions(baseOverlayOptions(this.volumeAxisSize, settings));
        this.chart.update("none");
    }
    /**
     * Apply sliced datasets from pre-computed full datasets.
     */
    applySlicedData(fullMainDatasets, startIndex) {
        if (!this.chart)
            return;
        // Price dataset (candlestick - index 0)
        const fullPrice = fullMainDatasets[0];
        const currentPrice = this.chart.data.datasets[0];
        if (fullPrice && currentPrice && fullPrice.type === "candlestick") {
            currentPrice.data = [...fullPrice.data.slice(startIndex)];
        }
        // Volume dataset (bar - index 1)
        const fullVolume = fullMainDatasets[1];
        const currentVolume = this.chart.data.datasets[1];
        if (fullVolume && currentVolume && fullVolume.type === "bar") {
            currentVolume.data = [...fullVolume.data.slice(startIndex)];
            if (fullVolume.backgroundColor && Array.isArray(fullVolume.backgroundColor)) {
                currentVolume.backgroundColor = [
                    ...fullVolume.backgroundColor.slice(startIndex)
                ];
            }
        }
        this.chart.update();
    }
    resize() {
        if (!this.chart)
            return;
        this.chart.resize();
        this.chart.update("resize");
    }
    destroy() {
        if (this.chart) {
            this.chart.destroy();
            this.chart = undefined;
        }
    }
}
//# sourceMappingURL=overlay-chart.js.map