export const COLORS = {
    GREEN: "#2E7D32",
    GRAY: "#9E9E9E",
    RED: "#DD2C00",
    CANDLE_UP: "#1B5E20",
    CANDLE_DOWN: "#B71C1C",
    CANDLE_UNCHANGED: "#616161",
    VOLUME_UP: "#1B5E2060",
    VOLUME_DOWN: "#B71C1C60",
    VOLUME_UNCHANGED: "#61616160"
};
const LIGHT_PALETTE = {
    candle: {
        up: COLORS.CANDLE_UP,
        down: COLORS.CANDLE_DOWN,
        unchanged: COLORS.CANDLE_UNCHANGED
    },
    candleBorder: {
        up: COLORS.CANDLE_UP,
        down: COLORS.CANDLE_DOWN,
        unchanged: COLORS.CANDLE_UNCHANGED
    },
    volume: {
        up: COLORS.VOLUME_UP,
        down: COLORS.VOLUME_DOWN,
        unchanged: COLORS.VOLUME_UNCHANGED
    }
};
// TODO: DARK_PALETTE currently uses the same colors as LIGHT_PALETTE.
// Future work should define distinct dark-mode variants with appropriate contrast
// adjustments for borders and volume colors to improve visibility in dark themes.
const DARK_PALETTE = {
    candle: {
        up: COLORS.CANDLE_UP,
        down: COLORS.CANDLE_DOWN,
        unchanged: COLORS.CANDLE_UNCHANGED
    },
    candleBorder: {
        up: COLORS.CANDLE_UP,
        down: COLORS.CANDLE_DOWN,
        unchanged: COLORS.CANDLE_UNCHANGED
    },
    volume: {
        up: COLORS.VOLUME_UP,
        down: COLORS.VOLUME_DOWN,
        unchanged: COLORS.VOLUME_UNCHANGED
    }
};
export function getFinancialPalette(mode) {
    return mode === "dark" ? DARK_PALETTE : LIGHT_PALETTE;
}
export function getCandleColor(open, close, palette) {
    if (close > open)
        return palette.candle.up;
    if (close < open)
        return palette.candle.down;
    return palette.candle.unchanged;
}
export function getVolumeColor(open, close, palette) {
    if (close > open)
        return palette.volume.up;
    if (close < open)
        return palette.volume.down;
    return palette.volume.unchanged;
}
//# sourceMappingURL=colors.js.map