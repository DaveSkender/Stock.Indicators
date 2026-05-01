interface ScaleLike {
    _length?: number;
    left: number;
    right: number;
    top: number;
    bottom: number;
    isHorizontal?: () => boolean;
    ticks: unknown[];
    getPixelForTick: (index: number) => number;
}
export declare function computeMinSampleSize(scale: ScaleLike, pixels: number[]): number;
export {};
//# sourceMappingURL=sample-size.d.ts.map