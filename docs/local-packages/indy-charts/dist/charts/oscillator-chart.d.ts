import { Chart, ChartDataset } from "chart.js";
import { ChartSettings, IndicatorListing, IndicatorSelection } from "../config/types";
export declare class OscillatorChart {
    private readonly ctx;
    private settings;
    chart: Chart | undefined;
    private fullThresholdDatasets;
    constructor(ctx: CanvasRenderingContext2D | HTMLCanvasElement, settings: ChartSettings);
    /**
     * Render an oscillator chart with the given selection and listing config.
     */
    render(selection: IndicatorSelection, listing: IndicatorListing): void;
    updateLegend(selection: IndicatorSelection): void;
    updateTheme(settings: ChartSettings): void;
    /**
     * Apply sliced datasets from pre-computed full datasets.
     */
    applySlicedData(selection: IndicatorSelection, fullDatasets: ChartDataset[], startIndex: number): void;
    resize(): void;
    destroy(): void;
    private configureThresholds;
    private configureYAxis;
}
//# sourceMappingURL=oscillator-chart.d.ts.map