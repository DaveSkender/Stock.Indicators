export interface FinancialBarLike {
    horizontal?: boolean;
    getProps: (keys: string[], useFinalPosition?: boolean) => {
        x: number;
        y: number;
        base: number;
        low: number;
        high: number;
        width: number;
        height: number;
    };
}
export interface BarBounds {
    left: number;
    top: number;
    right: number;
    bottom: number;
}
export declare function getBarBounds(bar: FinancialBarLike, useFinalPosition?: boolean): BarBounds;
export declare function inRange(bar: FinancialBarLike, x: number | null, y: number | null, useFinalPosition?: boolean): boolean;
//# sourceMappingURL=bar-bounds.d.ts.map