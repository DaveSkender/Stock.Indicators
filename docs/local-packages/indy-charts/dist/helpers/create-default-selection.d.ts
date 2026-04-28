import type { IndicatorListing, IndicatorSelection } from "../config";
/**
 * Create an `IndicatorSelection` from an `IndicatorListing` with default
 * parameter values and empty result datasets.
 *
 * @param listing - The indicator listing that defines available params and results.
 * @param paramOverrides - Optional record mapping `paramName` to a numeric
 *   value that overrides the listing default.
 * @param idPrefix - Optional prefix for the generated unique ID (default `"chart"`).
 * @returns A fully initialized `IndicatorSelection` ready for API data population.
 */
export declare function createDefaultSelection(listing: IndicatorListing, paramOverrides?: Record<string, number>, idPrefix?: string): IndicatorSelection;
//# sourceMappingURL=create-default-selection.d.ts.map