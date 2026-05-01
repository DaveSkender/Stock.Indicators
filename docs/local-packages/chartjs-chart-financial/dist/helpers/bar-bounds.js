/*
 * Derived from chartjs-chart-financial (https://github.com/chartjs/chartjs-chart-financial)
 * Version reference: upstream plugin v0.2.x API surface.
 * License: MIT.
 */
export function getBarBounds(bar, useFinalPosition) {
    const { x, y, low, high, width, height } = bar.getProps(["x", "y", "base", "low", "high", "width", "height"], useFinalPosition);
    let left;
    let right;
    let top;
    let bottom;
    if (bar.horizontal) {
        const half = height / 2;
        left = Math.min(low, high);
        right = Math.max(low, high);
        top = y - half;
        bottom = y + half;
    }
    else {
        const half = width / 2;
        left = x - half;
        right = x + half;
        top = Math.min(low, high);
        bottom = Math.max(low, high);
    }
    return { left, top, right, bottom };
}
export function inRange(bar, x, y, useFinalPosition) {
    const skipX = x === null;
    const skipY = y === null;
    const bounds = skipX && skipY ? undefined : getBarBounds(bar, useFinalPosition);
    if (!bounds)
        return false;
    return ((skipX || (x != null && x >= bounds.left && x <= bounds.right)) &&
        (skipY || (y != null && y >= bounds.top && y <= bounds.bottom)));
}
//# sourceMappingURL=bar-bounds.js.map