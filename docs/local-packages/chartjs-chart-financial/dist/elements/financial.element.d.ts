import { Element } from "chart.js";
export declare class FinancialElement extends Element {
    x: number;
    y: number;
    base: number;
    low: number;
    high: number;
    open: number;
    close: number;
    direction: "up" | "down" | "unchanged";
    width: number;
    height(): number;
    inRange(mouseX: number, mouseY: number, useFinalPosition?: boolean): boolean;
    inXRange(mouseX: number, useFinalPosition?: boolean): boolean;
    inYRange(mouseY: number, useFinalPosition?: boolean): boolean;
    getRange(axis: "x" | "y"): number;
    getCenterPoint(useFinalPosition?: boolean): {
        x: number;
        y: number;
    };
    tooltipPosition(useFinalPosition?: boolean): {
        x: number;
        y: number;
    };
}
//# sourceMappingURL=financial.element.d.ts.map