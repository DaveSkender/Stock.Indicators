import type { ApiClientConfig } from "../api";
import type { ChartSettings } from "../config";
export type IndyChartsVitePressApiOptions = ApiClientConfig;
export interface StockIndicatorChartConfig {
    id?: string;
    uiid?: string;
    title?: string;
    params?: Record<string, number>;
    results?: string[];
    barCount?: number;
    quoteCount?: number;
}
export type StockIndicatorChartRegistry = Record<string, StockIndicatorChartConfig>;
export interface IndyChartsVitePressDefaults {
    indicator?: string;
    barCount?: number;
    quoteCount?: number;
    showTooltips?: boolean;
}
export interface IndyChartsVitePressThemeOptions {
    isDarkTheme?: boolean;
    observeVitePressDarkMode?: boolean;
}
export interface IndyChartsVitePressOptions {
    api: IndyChartsVitePressApiOptions;
    defaults?: IndyChartsVitePressDefaults;
    theme?: IndyChartsVitePressThemeOptions;
    indicators?: StockIndicatorChartRegistry;
}
export interface StockIndicatorChartProps {
    indicator?: string;
    config?: StockIndicatorChartConfig;
    barCount?: number;
}
export type StockIndicatorChartPhase = "idle" | "loading" | "ready" | "empty" | "error";
export declare function chartSettingsFromOptions(options: IndyChartsVitePressOptions, isDarkTheme: boolean): ChartSettings;
//# sourceMappingURL=types.d.ts.map