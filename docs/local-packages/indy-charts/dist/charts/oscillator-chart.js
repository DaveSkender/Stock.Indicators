import { Chart } from "chart.js";
import { baseOscillatorConfig, baseOscillatorOptions } from "../config/oscillator";
import { createThresholdDataset } from "../config/datasets";
import { commonLegendAnnotation } from "../config/annotations";
export class OscillatorChart {
    constructor(ctx, settings) {
        this.ctx = ctx;
        this.settings = settings;
        this.fullThresholdDatasets = [];
    }
    /**
     * Render an oscillator chart with the given selection and listing config.
     */
    render(selection, listing) {
        const chartConfig = baseOscillatorConfig(this.settings);
        // reset stored full threshold datasets for this render
        this.fullThresholdDatasets = [];
        // Add thresholds
        this.configureThresholds(chartConfig, selection, listing);
        // Configure y-axis bounds
        this.configureYAxis(chartConfig, listing);
        // Add selection datasets
        selection.results.forEach((r) => {
            chartConfig.data.datasets.push(r.dataset);
        });
        // Create chart
        if (this.chart)
            this.chart.destroy();
        this.chart = new Chart(this.ctx, chartConfig);
        // Add legend (after scales are drawn)
        this.chart.update("none");
        this.updateLegend(selection);
        this.chart.update("none");
    }
    updateLegend(selection) {
        if (!this.chart)
            return;
        if (!this.chart.scales["x"] || !this.chart.scales["y"])
            return;
        const xPos = this.chart.scales["x"].min;
        const yPos = this.chart.scales["y"].max;
        const annotation = commonLegendAnnotation(selection.label, xPos, yPos, 1, this.settings);
        if (this.chart.options?.plugins?.annotation) {
            this.chart.options.plugins.annotation.annotations = { annotation };
        }
    }
    updateTheme(settings) {
        this.settings = settings;
        if (!this.chart)
            return;
        // Preserve theme-specific runtime options from render() that must persist
        // across theme changes (tooltip filter for thresholds, y-axis suggested bounds)
        const newOptions = baseOscillatorOptions(settings);
        const existingOptions = this.chart.options;
        // Preserve tooltip filter (filters out threshold datasets from tooltips)
        if (existingOptions?.plugins?.tooltip?.filter && newOptions.plugins?.tooltip) {
            newOptions.plugins.tooltip.filter = existingOptions.plugins.tooltip.filter;
        }
        const existingY = existingOptions?.scales?.["y"];
        const newY = newOptions.scales?.["y"];
        if (existingY && newY && typeof existingY.suggestedMin === "number") {
            newY.suggestedMin = existingY.suggestedMin;
        }
        if (existingY && newY && typeof existingY.suggestedMax === "number") {
            newY.suggestedMax = existingY.suggestedMax;
        }
        this.chart.options = newOptions;
        this.chart.update("none");
    }
    /**
     * Apply sliced datasets from pre-computed full datasets.
     */
    applySlicedData(selection, fullDatasets, startIndex) {
        // Slice threshold datasets (inserted before selection datasets in render)
        this.fullThresholdDatasets.forEach((fullDataset, i) => {
            if (!this.chart?.data.datasets[i])
                return;
            if (fullDataset.data && Array.isArray(fullDataset.data)) {
                this.chart.data.datasets[i].data = [...fullDataset.data.slice(startIndex)];
            }
            if (fullDataset.backgroundColor && Array.isArray(fullDataset.backgroundColor)) {
                this.chart.data.datasets[i].backgroundColor = [
                    ...fullDataset.backgroundColor.slice(startIndex)
                ];
            }
        });
        selection.results.forEach((result, resultIndex) => {
            if (!result.dataset || !fullDatasets[resultIndex])
                return;
            const fullDataset = fullDatasets[resultIndex];
            if (!fullDataset.data || !Array.isArray(fullDataset.data))
                return;
            result.dataset.data = [...fullDataset.data.slice(startIndex)];
            // Slice array properties
            const ext = fullDataset;
            const resExt = result.dataset;
            if (ext.pointRotation && Array.isArray(ext.pointRotation)) {
                resExt.pointRotation = ext.pointRotation
                    .slice(startIndex)
                    .map(v => (typeof v === "number" ? v : NaN));
            }
            if (ext.pointBackgroundColor && Array.isArray(ext.pointBackgroundColor)) {
                resExt.pointBackgroundColor = [...ext.pointBackgroundColor.slice(startIndex)];
            }
            if (ext.pointBorderColor && Array.isArray(ext.pointBorderColor)) {
                resExt.pointBorderColor = [...ext.pointBorderColor.slice(startIndex)];
            }
            if (fullDataset.backgroundColor && Array.isArray(fullDataset.backgroundColor)) {
                result.dataset.backgroundColor = [
                    ...fullDataset.backgroundColor.slice(startIndex)
                ];
            }
        });
        if (this.chart) {
            this.updateLegend(selection);
            this.chart.update();
        }
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
    configureThresholds(chartConfig, selection, listing) {
        const qtyThresholds = listing.chartConfig?.thresholds?.length ?? 0;
        listing.chartConfig?.thresholds?.forEach((threshold, index) => {
            const firstResult = selection.results?.[0];
            if (!firstResult)
                return;
            const thresholdDataset = createThresholdDataset(threshold, firstResult, index);
            chartConfig.data.datasets.push(thresholdDataset);
            // store a full (unsliced) copy for later dynamic slicing
            this.fullThresholdDatasets.push(structuredClone(thresholdDataset));
        });
        // Hide thresholds from tooltips
        if ((qtyThresholds ?? 0) > 0 && chartConfig.options?.plugins?.tooltip) {
            chartConfig.options.plugins.tooltip.filter = (tooltipItem) => tooltipItem.datasetIndex > (qtyThresholds ?? 0) - 1;
        }
    }
    configureYAxis(chartConfig, listing) {
        if (chartConfig.options?.scales?.["y"]) {
            chartConfig.options.scales["y"].suggestedMin = listing.chartConfig?.minimumYAxis;
            chartConfig.options.scales["y"].suggestedMax = listing.chartConfig?.maximumYAxis;
        }
    }
}
//# sourceMappingURL=oscillator-chart.js.map