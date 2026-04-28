import { COLORS } from "../theme/colors";
const CANDLE_HIGH_MULTIPLIER = 1.01;
const CANDLE_LOW_MULTIPLIER = 0.99;
const CATEGORIES = {
    CANDLESTICK_PATTERN: "candlestick-pattern"
};
export function processQuoteData(quotes) {
    const priceData = [];
    let sumVol = 0;
    quotes.forEach((q) => {
        priceData.push({
            x: new Date(q.date).valueOf(),
            o: q.open,
            h: q.high,
            l: q.low,
            c: q.close
        });
        sumVol += q.volume;
    });
    const volumeAxisSize = 20 * (sumVol / Math.max(1, quotes.length)) || 0;
    return { priceData, volumeAxisSize };
}
export function buildDataPoints(data, result, listing) {
    const dataPoints = [];
    const pointColor = [];
    const pointRotation = [];
    data.forEach(row => {
        const rawValue = row[result.dataName];
        let yValue = typeof rawValue === "number" ? rawValue : undefined;
        // apply candle pointers
        if (yValue !== undefined && listing.category === CATEGORIES.CANDLESTICK_PATTERN) {
            const candleConfig = getCandlePointConfiguration(row["match"], row.candle);
            yValue = candleConfig.yValue;
            pointColor.push(candleConfig.color);
            pointRotation.push(candleConfig.rotation);
        }
        else {
            const resultConfig = listing.results.find(x => x.dataName === result.dataName);
            pointColor.push(resultConfig?.defaultColor ?? COLORS.GRAY);
            pointRotation.push(0);
        }
        if (typeof yValue !== "number") {
            yValue = NaN;
        }
        dataPoints.push({ x: new Date(row.date).valueOf(), y: yValue });
    });
    return { dataPoints, pointColor, pointRotation };
}
export function addExtraBars(dataPoints, extraBars) {
    const maxTime = dataPoints.length > 0
        ? Math.max(...dataPoints.map(h => {
            const dateTime = h.x != null ? new Date(h.x).getTime() : 0;
            return Number.isFinite(dateTime) ? dateTime : 0;
        }))
        : 0;
    // Fall back to today when dataPoints is empty or every timestamp was invalid.
    const baseDate = maxTime > 0 ? new Date(maxTime) : new Date();
    for (let i = 0; i < extraBars; i++) {
        // Advance to the next business day, skipping Saturday (6) and Sunday (0),
        // so extra bars align with expected trading sessions on daily charts.
        do {
            baseDate.setDate(baseDate.getDate() + 1);
        } while (baseDate.getDay() === 0 || baseDate.getDay() === 6);
        dataPoints.push({ x: baseDate.valueOf(), y: NaN });
    }
}
export function getCandlePointConfiguration(match, candle) {
    switch (match) {
        case -100:
            return {
                yValue: CANDLE_HIGH_MULTIPLIER * candle.high,
                color: COLORS.RED,
                rotation: 180
            };
        case 100:
            return {
                yValue: CANDLE_LOW_MULTIPLIER * candle.low,
                color: COLORS.GREEN,
                rotation: 0
            };
        default:
            return {
                yValue: CANDLE_LOW_MULTIPLIER * candle.low,
                color: COLORS.GRAY,
                rotation: 0
            };
    }
}
//# sourceMappingURL=transformers.js.map