/**
 * Load static quote data synchronously (for VitePress SSG or build-time rendering).
 * Transforms raw string dates to Date objects.
 */
export function loadStaticQuotes(raw) {
    return raw.map(q => ({
        date: new Date(q.date),
        open: q.open,
        high: q.high,
        low: q.low,
        close: q.close,
        volume: q.volume
    }));
}
/**
 * Load static indicator data synchronously (for VitePress SSG or build-time rendering).
 * Passes through data as-is since indicator results are already in the correct format.
 */
export function loadStaticIndicatorData(data) {
    return data;
}
//# sourceMappingURL=static.js.map