import { BarController } from "chart.js";
interface ParsedFinancialLike {
    x: number;
    o: number;
    h: number;
    l: number;
    c: number;
    y?: number | null;
}
interface ControllerMetaLike {
    iScale: {
        axis: "x" | "y";
        _startPixel: number;
        _endPixel: number;
        ticks: unknown[];
        _length?: number;
        left: number;
        right: number;
        top: number;
        bottom: number;
        isHorizontal?: () => boolean;
        getPixelForTick: (index: number) => number;
        getPixelForValue: (value: number) => number;
        getLabelForValue: (value: number) => string;
    };
    vScale: {
        getBasePixel: () => number;
        getPixelForValue: (value: number) => number;
    };
    _parsed: ParsedFinancialLike[];
    data: Array<{
        draw: (ctx: CanvasRenderingContext2D) => void;
    }>;
}
interface Ruler {
    min: number;
    pixels: number[];
    start: number;
    end: number;
    stackCount: number;
    scale: ControllerMetaLike["iScale"];
    ratio: number;
}
export declare class FinancialController extends BarController {
    static overrides: {
        label: string;
        parsing: boolean;
        hover: {
            mode: string;
        };
        datasets: {
            categoryPercentage: number;
            barPercentage: number;
            animation: {
                numbers: {
                    type: string;
                    properties: string[];
                };
            };
        };
        plugins: {
            tooltip: {
                intersect: boolean;
                mode: string;
                callbacks: {
                    label(ctx: {
                        parsed: ParsedFinancialLike;
                    }): string;
                };
            };
        };
    };
    getLabelAndValue(index: number): {
        label: string;
        value: string;
    };
    getAllParsedValues(): number[];
    getMinMax(scale: object): {
        min: number;
        max: number;
    };
    _getRuler(): Ruler;
    calculateElementProperties(index: number, ruler: Ruler, reset: boolean, options: unknown): {
        base: number;
        x: number;
        y: number;
        width: number;
        open: number;
        high: number;
        low: number;
        close: number;
        direction: "up" | "down" | "unchanged";
    };
    draw(): void;
}
export {};
//# sourceMappingURL=financial.controller.d.ts.map