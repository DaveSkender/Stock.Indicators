function toQuotes(raw) {
    return raw.map((q, index) => ({
        date: parseQuoteDate(q.date, index),
        open: q.open,
        high: q.high,
        low: q.low,
        close: q.close,
        volume: q.volume
    }));
}
function parseQuoteDate(value, index) {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        throw new Error(`Invalid quote date at index ${index}: "${value}"`);
    }
    return date;
}
function endpointUrl(baseUrl, endpoint) {
    return new URL(endpoint, baseUrl).toString();
}
/**
 * Factory that creates a ready-to-use {@link ApiClient}.
 *
 * The `baseUrl` is normalised to always end with `/` so that relative
 * endpoint paths resolve correctly via `new URL(path, base)`.
 *
 * @param config - Connection and error-handling options.
 * @returns A fully configured {@link ApiClient} instance.
 *
 * @example
 * ```ts
 * const client = createApiClient({
 *   baseUrl: "https://api.example.com",
 *   onError: (ctx, err) => console.error(ctx, err),
 * });
 * const quotes = await client.getQuotes();
 * ```
 */
export function createApiClient(config) {
    const { endpoints, onError } = config;
    // Ensure baseUrl always ends with "/" so new URL(path, base) resolves correctly.
    const baseUrl = config.baseUrl.endsWith("/") ? config.baseUrl : `${config.baseUrl}/`;
    return {
        async getQuotes() {
            try {
                const response = await fetch(endpointUrl(baseUrl, endpoints?.quotes ?? "quotes"));
                if (!response.ok) {
                    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
                }
                const raw = (await response.json());
                return toQuotes(raw);
            }
            catch (error) {
                onError?.("Error fetching quotes", error);
                throw error;
            }
        },
        async getListings() {
            try {
                const response = await fetch(endpointUrl(baseUrl, endpoints?.indicators ?? "indicators"));
                if (!response.ok) {
                    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
                }
                const data = (await response.json());
                return data;
            }
            catch (error) {
                onError?.("Error fetching listings", error);
                throw error;
            }
        },
        async getSelectionData(selection, listing) {
            const endpointUrl = new URL(listing.endpoint, baseUrl);
            selection.params.forEach((p) => {
                if (p.value != null) {
                    endpointUrl.searchParams.set(p.paramName, String(p.value));
                }
            });
            const url = endpointUrl.toString();
            try {
                const response = await fetch(url);
                if (!response.ok) {
                    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
                }
                const data = (await response.json());
                return data;
            }
            catch (error) {
                onError?.("Error fetching selection data", error);
                throw error;
            }
        }
    };
}
//# sourceMappingURL=client.js.map