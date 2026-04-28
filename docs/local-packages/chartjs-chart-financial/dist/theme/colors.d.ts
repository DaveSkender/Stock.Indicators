import { FinancialPalette, FinancialThemeMode } from "../types/financial.types";
export declare const COLORS: {
    readonly GREEN: "#2E7D32";
    readonly GRAY: "#9E9E9E";
    readonly RED: "#DD2C00";
    readonly CANDLE_UP: "#1B5E20";
    readonly CANDLE_DOWN: "#B71C1C";
    readonly CANDLE_UNCHANGED: "#616161";
    readonly VOLUME_UP: "#1B5E2060";
    readonly VOLUME_DOWN: "#B71C1C60";
    readonly VOLUME_UNCHANGED: "#61616160";
};
export declare function getFinancialPalette(mode: FinancialThemeMode): FinancialPalette;
export declare function getCandleColor(open: number, close: number, palette: FinancialPalette): string;
export declare function getVolumeColor(open: number, close: number, palette: FinancialPalette): string;
//# sourceMappingURL=colors.d.ts.map