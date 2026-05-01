/*
 * Derived from chartjs-chart-financial (https://github.com/chartjs/chartjs-chart-financial)
 * Version reference: upstream plugin v0.2.x API surface.
 * License: MIT.
 */
function getScaleLength(scale) {
    if (typeof scale._length === "number") {
        return scale._length;
    }
    const horizontal = scale.isHorizontal
        ? scale.isHorizontal()
        : scale.right - scale.left >= scale.bottom - scale.top;
    return horizontal ? scale.right - scale.left : scale.bottom - scale.top;
}
export function computeMinSampleSize(scale, pixels) {
    let min = getScaleLength(scale);
    let prev;
    for (let i = 1; i < pixels.length; ++i) {
        min = Math.min(min, Math.abs(pixels[i] - pixels[i - 1]));
    }
    for (let i = 0; i < scale.ticks.length; ++i) {
        const curr = scale.getPixelForTick(i);
        min = i > 0 && prev != null ? Math.min(min, Math.abs(curr - prev)) : min;
        prev = curr;
    }
    return min;
}
//# sourceMappingURL=sample-size.js.map