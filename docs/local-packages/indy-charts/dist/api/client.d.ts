import { IndicatorListing, IndicatorSelection, Quote } from "../config/types";
/**
 * Configuration for {@link createApiClient}.
 *
 * @example
 * ```ts
 * const config: ApiClientConfig = {
 *   baseUrl: "https://api.example.com/",
 *   onError: (ctx, err) => console.error(ctx, err),
 * };
 * ```
 */
export interface ApiClientConfig {
    /**
     * Root URL of the API server. A trailing slash is recommended but optional —
     * `createApiClient` normalises it internally.
     *
     * @example "https://api.example.com/"
     */
    baseUrl: string;
    /** Optional endpoint overrides for hosts that mount API routes elsewhere. */
    endpoints?: {
        quotes?: string;
        indicators?: string;
    };
    /**
     * Optional error callback invoked whenever a fetch operation throws or
     * receives a non-2xx response.  The error is **re-thrown** after the
     * callback returns, so callers still need to handle it.
     *
     * @param context - Human-readable description of the failed operation.
     * @param error   - The original caught value (usually an `Error` instance).
     */
    onError?: (context: string, error: unknown) => void;
}
/**
 * Lightweight HTTP client for the stock-charts API.
 *
 * Obtain an instance with {@link createApiClient}.
 */
export interface ApiClient {
    /**
     * Fetches the raw OHLCV quote history from `GET /quotes`.
     *
     * @returns Resolved array of {@link Quote} objects sorted chronologically.
     * @throws  Re-throws any network or HTTP error (after calling `onError`).
     */
    getQuotes(): Promise<Quote[]>;
    /**
     * Fetches all available indicator listings from `GET /indicators`.
     *
     * @returns Resolved array of {@link IndicatorListing} descriptors.
     * @throws  Re-throws any network or HTTP error (after calling `onError`).
     */
    getListings(): Promise<IndicatorListing[]>;
    /**
     * Fetches computed indicator data for the given selection and listing.
     * Query-string parameters are derived from {@link IndicatorSelection.params}.
     *
     * @param selection - The user's current indicator parameter choices.
     * @param listing   - The indicator descriptor that provides the endpoint path.
     * @returns Resolved array of raw data rows for the indicator series.
     * @throws  Re-throws any network or HTTP error (after calling `onError`).
     */
    getSelectionData(selection: IndicatorSelection, listing: IndicatorListing): Promise<unknown[]>;
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
export declare function createApiClient(config: ApiClientConfig): ApiClient;
//# sourceMappingURL=client.d.ts.map