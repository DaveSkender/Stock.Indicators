import { ChartDataset, ScatterDataPoint } from "chart.js";
import { FinancialDataPoint, FinancialPalette } from "./types/financial.types";
interface QuoteLike {
    date: Date;
    open: number;
    high: number;
    low: number;
    close: number;
    volume: number;
}
export declare function toFinancialDataPoint(quote: QuoteLike): FinancialDataPoint;
/** Builds a typed candlestick dataset from normalized OHLC points. */
export declare function buildCandlestickDataset(priceData: FinancialDataPoint[], borderColor: FinancialPalette["candleBorder"]): ChartDataset<"candlestick", FinancialDataPoint[]>;
/** Builds a typed volume dataset with up/down/unchanged candle coloring. */
export declare function buildVolumeDataset(quotes: QuoteLike[], extraBars: number, palette: FinancialPalette): ChartDataset<"bar", ScatterDataPoint[]>;
export {};
//# sourceMappingURL=datasets.d.ts.map