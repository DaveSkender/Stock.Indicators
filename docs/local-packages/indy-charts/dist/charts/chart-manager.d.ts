import { ChartSettings, IndicatorDataRow, IndicatorListing, IndicatorSelection, Quote } from "../config/types";
import { OverlayChart } from "./overlay-chart";
import { OscillatorChart } from "./oscillator-chart";
export interface ChartManagerConfig {
    settings: ChartSettings;
    extraBars?: number;
}
export declare class ChartManager {
    private _overlayChart;
    private _oscillators;
    private _selections;
    private _allQuotes;
    private _allProcessedDatasets;
    private _currentBarCount;
    private _settings;
    private extraBars;
    /** Current chart settings. Update via updateTheme() to propagate to all charts. */
    get settings(): ChartSettings;
    /** Read-only access to the overlay chart instance. */
    get overlayChart(): OverlayChart | undefined;
    /** Read-only view of registered oscillator charts keyed by ucid. */
    get oscillators(): ReadonlyMap<string, OscillatorChart>;
    /** Read-only view of registered indicator selections. */
    get selections(): ReadonlyArray<IndicatorSelection>;
    /** Read-only view of the full quote dataset. */
    get allQuotes(): ReadonlyArray<Quote>;
    /** Current number of bars being displayed. */
    get currentBarCount(): number;
    constructor(config: ChartManagerConfig);
    /**
     * Initialize the overlay chart with quote data.
     * @param ctx - Canvas rendering context or canvas element
     * @param allQuotes - Full quote dataset
     * @param barCount - Number of bars to display initially
     */
    initializeOverlay(ctx: CanvasRenderingContext2D | HTMLCanvasElement, allQuotes: Quote[], barCount: number): void;
    /**
     * Process indicator data and prepare selection datasets.
     * This is separated from display so consumers can control DOM creation.
     */
    processSelectionData(selection: IndicatorSelection, listing: IndicatorListing, data: IndicatorDataRow[]): void;
    /**
     * Display a processed selection on the appropriate chart.
     */
    displaySelection(selection: IndicatorSelection, listing: IndicatorListing): void;
    /**
     * Display indicator on the overlay chart.
     * Slices datasets to currentBarCount before adding so overlay indicators
     * are aligned with the windowed x-axis from the start.
     */
    private displayOnOverlay;
    /**
     * Create an oscillator chart for a selection.
     * Consumer must provide the canvas context and call this after processSelectionData()
     * AND after displaySelection() so the selection is registered in this.selections.
     *
     * @throws {Error} if displaySelection() has not been called for this selection,
     *   because setBarCount() iterates this.selections and will silently skip any
     *   oscillator whose ucid is not present there.
     */
    createOscillator(ctx: CanvasRenderingContext2D | HTMLCanvasElement, selection: IndicatorSelection, listing: IndicatorListing): OscillatorChart;
    /**
     * Remove an indicator selection and its chart.
     */
    removeSelection(ucid: string): void;
    /**
     * Update theme across all charts.
     */
    updateTheme(settings: ChartSettings): void;
    /**
     * Update all charts with a new bar count (for window resize).
     */
    setBarCount(barCount: number): void;
    /**
     * Force all charts to resize.
     * Defers one animation frame so CSS layout has settled before Chart.js reads
     * container dimensions, avoiding race conditions on immediate resize calls.
     */
    resize(): void;
    /**
     * Destroy all charts and clean up.
     */
    destroy(): void;
    private normalizeBarCount;
}
//# sourceMappingURL=chart-manager.d.ts.map