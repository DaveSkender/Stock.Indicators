import { baseDataset } from "../config/datasets";
import { buildDataPoints, addExtraBars } from "../data/transformers";
import { OverlayChart } from "./overlay-chart";
import { OscillatorChart } from "./oscillator-chart";
const EXTRA_BARS = 7;
const CHART_TYPES = {
    OVERLAY: "overlay",
    OSCILLATOR: "oscillator"
};
const CATEGORIES = {
    CANDLESTICK_PATTERN: "candlestick-pattern"
};
export class ChartManager {
    /** Current chart settings. Update via updateTheme() to propagate to all charts. */
    get settings() {
        return { ...this._settings };
    }
    /** Read-only access to the overlay chart instance. */
    get overlayChart() {
        return this._overlayChart;
    }
    /** Read-only view of registered oscillator charts keyed by ucid. */
    get oscillators() {
        return this._oscillators;
    }
    /** Read-only view of registered indicator selections. */
    get selections() {
        return this._selections;
    }
    /** Read-only view of the full quote dataset. */
    get allQuotes() {
        return this._allQuotes;
    }
    /** Current number of bars being displayed. */
    get currentBarCount() {
        return this._currentBarCount;
    }
    constructor(config) {
        this._oscillators = new Map();
        this._selections = [];
        this._allQuotes = [];
        this._allProcessedDatasets = new Map();
        this._currentBarCount = 250;
        this._settings = { ...config.settings };
        this.extraBars = config.extraBars ?? EXTRA_BARS;
    }
    /**
     * Initialize the overlay chart with quote data.
     * @param ctx - Canvas rendering context or canvas element
     * @param allQuotes - Full quote dataset
     * @param barCount - Number of bars to display initially
     */
    initializeOverlay(ctx, allQuotes, barCount) {
        this._allQuotes = allQuotes;
        this._currentBarCount = this.normalizeBarCount(barCount);
        this._overlayChart?.destroy();
        this._overlayChart = new OverlayChart(ctx, this._settings);
        // Render with full allQuotes so stored datasets cover complete history,
        // enabling correct slicing when setBarCount() is called later.
        const fullDatasets = this._overlayChart.render(allQuotes, this.extraBars);
        this._allProcessedDatasets.set("overlay-main", fullDatasets);
        // Apply initial barCount slice for display.
        const startIndex = Math.max(0, allQuotes.length - this._currentBarCount);
        this._overlayChart.applySlicedData(fullDatasets, startIndex);
        // Re-attach previously registered overlay selections so they survive
        // overlay re-initialization (e.g., theme/canvas reset). Without this,
        // displaySelection() would silently skip these (duplicate ucid guard),
        // leaving the chart without their datasets.
        const overlaySelections = this._selections.filter(s => s.chartType === CHART_TYPES.OVERLAY);
        overlaySelections.forEach(selection => {
            this.displayOnOverlay(selection);
        });
    }
    /**
     * Process indicator data and prepare selection datasets.
     * This is separated from display so consumers can control DOM creation.
     */
    processSelectionData(selection, listing, data) {
        selection.results.forEach((result) => {
            const resultConfig = listing.results.find((x) => x.dataName === result.dataName);
            if (!resultConfig)
                return;
            const dataset = baseDataset(result, resultConfig);
            const { dataPoints, pointColor, pointRotation } = buildDataPoints(data, result, listing);
            addExtraBars(dataPoints, this.extraBars);
            if (listing.category === CATEGORIES.CANDLESTICK_PATTERN && dataset.type !== "bar") {
                const ext = dataset;
                ext.pointRotation = pointRotation;
                ext.pointBackgroundColor = pointColor;
                ext.pointBorderColor = pointColor;
            }
            dataset.data = dataPoints;
            result.dataset = dataset;
        });
        // Store deep copy for efficient resizing.
        // structuredClone is used instead of JSON.parse/stringify to safely preserve
        // NaN values and avoid crashes on undefined fields.
        this._allProcessedDatasets.set(selection.ucid, selection.results.map(result => structuredClone(result.dataset)));
    }
    /**
     * Display a processed selection on the appropriate chart.
     */
    displaySelection(selection, listing) {
        if (this._selections.some(s => s.ucid === selection.ucid))
            return;
        this._selections.push(selection);
        if (listing.chartType === CHART_TYPES.OVERLAY) {
            this.displayOnOverlay(selection);
        }
        // Oscillator charts require consumer to provide the canvas context
        // via createOscillator() after calling processSelectionData()
    }
    /**
     * Display indicator on the overlay chart.
     * Slices datasets to currentBarCount before adding so overlay indicators
     * are aligned with the windowed x-axis from the start.
     */
    displayOnOverlay(selection) {
        if (!this._overlayChart)
            return;
        // Slice indicator datasets to currentBarCount before adding to the windowed chart.
        // Also slice style arrays (pointBackgroundColor, pointBorderColor, pointRotation)
        // to keep them synchronized with the data length.
        const fullDatasets = this._allProcessedDatasets.get(selection.ucid);
        if (fullDatasets) {
            const startIndex = Math.max(0, this._allQuotes.length - this._currentBarCount);
            selection.results.forEach((result, i) => {
                const full = fullDatasets[i];
                if (full && result.dataset) {
                    result.dataset.data = [...full.data.slice(startIndex)];
                    // Slice style arrays if present with type casts
                    const ext = result.dataset;
                    if (Array.isArray(full.pointBackgroundColor)) {
                        ext.pointBackgroundColor = [
                            ...full.pointBackgroundColor.slice(startIndex)
                        ];
                    }
                    if (Array.isArray(full.pointBorderColor)) {
                        ext.pointBorderColor = [...full.pointBorderColor.slice(startIndex)];
                    }
                    if (Array.isArray(full.pointRotation)) {
                        ext.pointRotation = [...full.pointRotation.slice(startIndex)];
                    }
                }
            });
        }
        this._overlayChart.addIndicatorDatasets(selection.results);
        this._overlayChart.updateLegends(this._selections);
    }
    /**
     * Create an oscillator chart for a selection.
     * Consumer must provide the canvas context and call this after processSelectionData()
     * AND after displaySelection() so the selection is registered in this.selections.
     *
     * @throws {Error} if displaySelection() has not been called for this selection,
     *   because setBarCount() iterates this.selections and will silently skip any
     *   oscillator whose ucid is not present there.
     */
    createOscillator(ctx, selection, listing) {
        // Guard: selection must be registered via displaySelection() first, otherwise
        // setBarCount() (which iterates this._selections) will never update this oscillator.
        if (!this._selections.some(s => s.ucid === selection.ucid)) {
            throw new Error(`createOscillator: selection "${selection.ucid}" has not been registered. ` +
                `Call displaySelection() before createOscillator() to ensure setBarCount() ` +
                `can update this oscillator when the window changes.`);
        }
        this._oscillators.get(selection.ucid)?.destroy();
        const oscillator = new OscillatorChart(ctx, this._settings);
        oscillator.render(selection, listing);
        this._oscillators.set(selection.ucid, oscillator);
        return oscillator;
    }
    /**
     * Remove an indicator selection and its chart.
     */
    removeSelection(ucid) {
        // Use findIndex for a single O(n) pass instead of find + indexOf.
        const sx = this._selections.findIndex(x => x.ucid === ucid);
        if (sx === -1)
            return;
        const [selection] = this._selections.splice(sx, 1);
        this._allProcessedDatasets.delete(ucid);
        if (selection.chartType === CHART_TYPES.OVERLAY) {
            if (this._overlayChart) {
                this._overlayChart.removeIndicatorDatasets(selection.results);
                this._overlayChart.updateLegends(this._selections);
                this._overlayChart.chart?.update();
            }
        }
        else {
            const oscillator = this._oscillators.get(ucid);
            if (oscillator) {
                oscillator.destroy();
                this._oscillators.delete(ucid);
            }
        }
    }
    /**
     * Update theme across all charts.
     */
    updateTheme(settings) {
        this._settings = { ...settings };
        if (this._overlayChart) {
            this._overlayChart.updateTheme(settings);
            this._overlayChart.updateLegends(this._selections);
            this._overlayChart.chart?.update("none");
        }
        this._oscillators.forEach((oscillator, ucid) => {
            oscillator.updateTheme(settings);
            const selection = this._selections.find(s => s.ucid === ucid);
            if (selection) {
                oscillator.updateLegend(selection);
            }
            oscillator.chart?.update("none");
        });
    }
    /**
     * Update all charts with a new bar count (for window resize).
     */
    setBarCount(barCount) {
        // No quotes -> nothing to do.
        if (this._allQuotes.length === 0)
            return;
        const totalQuotes = this._allQuotes.length;
        const normalizedBarCount = this.normalizeBarCount(barCount);
        if (normalizedBarCount === this._currentBarCount)
            return;
        this._currentBarCount = normalizedBarCount;
        const startIndex = Math.max(0, totalQuotes - normalizedBarCount);
        // Update overlay main datasets (candlestick + volume)
        const fullMainDatasets = this._allProcessedDatasets.get("overlay-main");
        if (this._overlayChart && fullMainDatasets) {
            this._overlayChart.applySlicedData(fullMainDatasets, startIndex);
            this._overlayChart.updateLegends(this._selections);
        }
        // Update overlay indicator datasets to stay aligned with the windowed x-axis.
        // Mirror the oscillator loop below but for OVERLAY selections: retrieve each
        // indicator's full dataset from allProcessedDatasets and slice to startIndex.
        // Also slice style arrays (pointBackgroundColor, pointBorderColor, pointRotation)
        // to keep them synchronized with the data length.
        let overlayIndicatorsUpdated = false;
        this._selections.forEach(selection => {
            if (selection.chartType !== CHART_TYPES.OVERLAY)
                return;
            const fullDatasets = this._allProcessedDatasets.get(selection.ucid);
            if (!fullDatasets)
                return;
            selection.results.forEach((result, i) => {
                const full = fullDatasets[i];
                if (full && result.dataset) {
                    result.dataset.data = [...full.data.slice(startIndex)];
                    // Slice style arrays if present with type casts
                    const ext = result.dataset;
                    if (Array.isArray(full.pointBackgroundColor)) {
                        ext.pointBackgroundColor = [
                            ...full.pointBackgroundColor.slice(startIndex)
                        ];
                    }
                    if (Array.isArray(full.pointBorderColor)) {
                        ext.pointBorderColor = [...full.pointBorderColor.slice(startIndex)];
                    }
                    if (Array.isArray(full.pointRotation)) {
                        ext.pointRotation = [...full.pointRotation.slice(startIndex)];
                    }
                }
            });
            overlayIndicatorsUpdated = true;
        });
        // Trigger a single chart refresh after all overlay indicator data has been sliced.
        if (overlayIndicatorsUpdated && this._overlayChart) {
            this._overlayChart.chart?.update("none");
        }
        // Update oscillators
        this._selections.forEach(selection => {
            if (selection.chartType !== CHART_TYPES.OSCILLATOR)
                return;
            const fullDatasets = this._allProcessedDatasets.get(selection.ucid);
            const oscillator = this._oscillators.get(selection.ucid);
            if (!fullDatasets || !oscillator)
                return;
            oscillator.applySlicedData(selection, fullDatasets, startIndex);
        });
    }
    /**
     * Force all charts to resize.
     * Defers one animation frame so CSS layout has settled before Chart.js reads
     * container dimensions, avoiding race conditions on immediate resize calls.
     */
    resize() {
        requestAnimationFrame(() => {
            this._overlayChart?.resize();
            this._oscillators.forEach(oscillator => oscillator.resize());
        });
    }
    /**
     * Destroy all charts and clean up.
     */
    destroy() {
        this._overlayChart?.destroy();
        this._overlayChart = undefined;
        this._oscillators.forEach(oscillator => oscillator.destroy());
        this._oscillators.clear();
        this._selections = [];
        this._allProcessedDatasets.clear();
        this._allQuotes = [];
    }
    normalizeBarCount(barCount) {
        if (this._allQuotes.length === 0)
            return 1;
        const normalized = Number.isFinite(barCount) ? Math.floor(barCount) : 1;
        return Math.max(1, Math.min(normalized, this._allQuotes.length));
    }
}
//# sourceMappingURL=chart-manager.js.map