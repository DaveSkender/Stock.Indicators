let localCounter = 0;
/**
 * Generate a unique ID for a chart selection.
 *
 * Uses `crypto.randomUUID()` when available, otherwise falls back to a
 * timestamp-based counter. An optional {@link prefix} is prepended.
 */
function nextId(prefix) {
    if (typeof crypto !== "undefined" && typeof crypto.randomUUID === "function") {
        return `${prefix}${crypto.randomUUID()}`;
    }
    localCounter += 1;
    return `${prefix}${Date.now()}-${localCounter}`;
}
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
export function createDefaultSelection(listing, paramOverrides, idPrefix = "chart") {
    const selection = {
        ucid: nextId(idPrefix),
        uiid: listing.uiid,
        label: listing.legendTemplate,
        chartType: listing.chartType,
        params: [],
        results: []
    };
    // Hydrate default parameters from the listing, applying any overrides.
    listing.parameters?.forEach((config) => {
        const param = {
            paramName: config.paramName,
            displayName: config.displayName,
            minimum: config.minimum,
            maximum: config.maximum,
            value: paramOverrides?.[config.paramName] ?? config.defaultValue
        };
        selection.params.push(param);
    });
    // Hydrate result placeholders with default colors, labels, and empty datasets.
    listing.results?.forEach((config) => {
        const result = {
            label: config.tooltipTemplate,
            color: config.defaultColor,
            dataName: config.dataName,
            displayName: config.displayName,
            lineType: config.lineType,
            lineWidth: typeof config.lineWidth === "number" ? config.lineWidth : 2,
            order: config.order,
            dataset: { type: "line", data: [] }
        };
        selection.results.push(result);
    });
    return selection;
}
//# sourceMappingURL=create-default-selection.js.map