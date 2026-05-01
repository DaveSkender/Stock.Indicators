import { Chart, ChartDataset } from "chart.js";
import { ChartSettings, IndicatorResult, IndicatorSelection, Quote } from "../config/types";
export declare class OverlayChart {
    private readonly ctx;
    private settings;
    chart: Chart | undefined;
    private volumeAxisSize;
    constructor(ctx: CanvasRenderingContext2D | HTMLCanvasElement, settings: ChartSettings);
    /**
     * Initialize the overlay chart with quote data.
     * Returns the full-resolution datasets for use in dynamic slicing.
     */
    render(quotes: Quote[], extraBars?: number): ChartDataset[];
    addIndicatorDatasets(results: IndicatorResult[]): void;
    removeIndicatorDatasets(results: IndicatorResult[]): void;
    updateLegends(overlaySelections: IndicatorSelection[]): void;
    updateTheme(settings: ChartSettings): void;
    /**
     * Apply sliced datasets from pre-computed full datasets.
     */
    applySlicedData(fullMainDatasets: ChartDataset[], startIndex: number): void;
    resize(): void;
    destroy(): void;
}
//# sourceMappingURL=overlay-chart.d.ts.map