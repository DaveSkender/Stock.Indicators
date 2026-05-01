import { getVolumeColor } from "./theme/colors";
export function toFinancialDataPoint(quote) {
    return {
        x: new Date(quote.date).valueOf(),
        o: quote.open,
        h: quote.high,
        l: quote.low,
        c: quote.close
    };
}
/** Builds a typed candlestick dataset from normalized OHLC points. */
export function buildCandlestickDataset(priceData, borderColor) {
    return {
        type: "candlestick",
        label: "Price",
        data: priceData,
        yAxisID: "y",
        borderColor: borderColor,
        order: 75
    };
}
/** Builds a typed volume dataset with up/down/unchanged candle coloring. */
export function buildVolumeDataset(quotes, extraBars, palette) {
    const volumeData = [];
    const volumeColors = [];
    quotes.forEach(quote => {
        volumeData.push({ x: new Date(quote.date).valueOf(), y: quote.volume });
        volumeColors.push(getVolumeColor(quote.open, quote.close, palette));
    });
    const maxTimeSource = volumeData.map(point => {
        const dateTime = point.x != null ? new Date(point.x).getTime() : 0;
        return Number.isFinite(dateTime) ? dateTime : 0;
    });
    const maxTimeCandidate = maxTimeSource.length > 0 ? Math.max(...maxTimeSource) : new Date().getTime();
    const maxTime = Number.isFinite(maxTimeCandidate) ? maxTimeCandidate : new Date().getTime();
    const nextDate = new Date(maxTime);
    for (let i = 1; i < extraBars; i++) {
        nextDate.setDate(nextDate.getDate() + 1);
        volumeData.push({ x: new Date(nextDate).valueOf(), y: Number.NaN });
        volumeColors.push(palette.volume.unchanged);
    }
    return {
        type: "bar",
        label: "Volume",
        data: volumeData,
        yAxisID: "volumeAxis",
        backgroundColor: volumeColors,
        borderWidth: 0,
        order: 76
    };
}
//# sourceMappingURL=datasets.js.map