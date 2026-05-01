import { Quote, RawQuote, IndicatorDataRow } from "../config/types";
/**
 * Load static quote data synchronously (for VitePress SSG or build-time rendering).
 * Transforms raw string dates to Date objects.
 */
export declare function loadStaticQuotes(raw: RawQuote[]): Quote[];
/**
 * Load static indicator data synchronously (for VitePress SSG or build-time rendering).
 * Passes through data as-is since indicator results are already in the correct format.
 */
export declare function loadStaticIndicatorData(data: unknown[]): IndicatorDataRow[];
//# sourceMappingURL=static.d.ts.map