/**
 * Calculate the optimal number of bars to display in a financial chart
 * based on available container width.
 *
 * Returns a value clamped between {@link MIN_BARS} (20) and
 * {@link MAX_BARS} (500).
 *
 * @param containerWidth - Available width in pixels for the chart.
 * @param pixelsPerBar - Pixels allocated per candlestick bar (default 5).
 * @returns The optimal bar count for the given width.
 */
export declare function calculateOptimalBars(containerWidth: number, pixelsPerBar?: number): number;
//# sourceMappingURL=calculate-optimal-bars.d.ts.map