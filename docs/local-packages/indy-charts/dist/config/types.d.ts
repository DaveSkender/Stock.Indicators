import { ChartDataset } from "chart.js";
/**
 * Extended dataset interface for candlestick pattern datasets that carry
 * per-point visual properties not present on the base ChartDataset type.
 * Used internally by ChartManager and OscillatorChart.
 */
export type ExtendedChartDataset = ChartDataset & {
    pointRotation?: number[];
    pointBackgroundColor?: string[];
    pointBorderColor?: string[];
};
export interface Quote {
    date: Date;
    open: number;
    high: number;
    low: number;
    close: number;
    volume: number;
}
export interface RawQuote {
    date: string;
    open: number;
    high: number;
    low: number;
    close: number;
    volume: number;
}
export interface IndicatorDataRow {
    date: string;
    candle: Quote;
    [key: string]: unknown;
}
export interface ChartSettings {
    isDarkTheme: boolean;
    showTooltips: boolean;
}
export interface IndicatorListing {
    name: string;
    uiid: string;
    legendTemplate: string;
    endpoint: string;
    category: string;
    chartType: string;
    order: number;
    chartConfig: ChartConfig | null;
    parameters: IndicatorParamConfig[];
    results: IndicatorResultConfig[];
}
export interface IndicatorParamConfig {
    displayName: string;
    paramName: string;
    dataType: string;
    defaultValue: number;
    minimum: number;
    maximum: number;
}
export interface IndicatorResultConfig {
    displayName: string;
    tooltipTemplate: string;
    dataName: string;
    dataType: string;
    lineType: string;
    stack: string;
    lineWidth: number | null;
    defaultColor: string;
    fill?: ChartFill | null;
    order: number;
}
export interface ChartConfig {
    minimumYAxis: number | null;
    maximumYAxis: number | null;
    thresholds: ChartThreshold[];
}
export interface ChartThreshold {
    value: number;
    color: string;
    style: string;
    fill?: ChartFill | null;
}
export interface ChartFill {
    target: string;
    colorAbove: string;
    colorBelow: string;
}
export interface IndicatorSelection {
    ucid: string;
    uiid: string;
    label: string;
    chartType: string;
    params: IndicatorParam[];
    results: IndicatorResult[];
}
export interface IndicatorParam {
    paramName: string;
    displayName: string;
    minimum: number;
    maximum: number;
    value?: number;
}
export interface IndicatorResult {
    label: string;
    displayName: string;
    dataName: string;
    color: string;
    lineType: string;
    lineWidth: number;
    order: number;
    dataset: ChartDataset;
}
//# sourceMappingURL=types.d.ts.map