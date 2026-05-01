import { ScatterDataPoint } from "chart.js";
import { FinancialDataPoint } from "../types/financial.types";
import { IndicatorDataRow, IndicatorListing, IndicatorResult, Quote } from "../config/types";
export declare function processQuoteData(quotes: Quote[]): {
    priceData: FinancialDataPoint[];
    volumeAxisSize: number;
};
export declare function buildDataPoints(data: IndicatorDataRow[], result: IndicatorResult, listing: IndicatorListing): {
    dataPoints: ScatterDataPoint[];
    pointColor: string[];
    pointRotation: number[];
};
export declare function addExtraBars(dataPoints: ScatterDataPoint[], extraBars: number): void;
export declare function getCandlePointConfiguration(match: number, candle: Quote): {
    yValue: number;
    color: string;
    rotation: number;
};
//# sourceMappingURL=transformers.d.ts.map