/*
 * Derived from chartjs-chart-financial (https://github.com/chartjs/chartjs-chart-financial)
 * Version reference: upstream plugin v0.2.x API surface.
 * License: MIT.
 */
import { BarController, defaults } from "chart.js";
import { clipArea, isNullOrUndef, unclipArea } from "chart.js/helpers";
import { computeMinSampleSize } from "../helpers/sample-size";
export class FinancialController extends BarController {
    getLabelAndValue(index) {
        const controller = this;
        const parsed = this.getParsed(index);
        const axis = controller._cachedMeta.iScale.axis;
        const { o, h, l, c } = parsed;
        return {
            label: `${controller._cachedMeta.iScale.getLabelForValue(parsed[axis] ?? 0)}`,
            value: `O: ${o}  H: ${h}  L: ${l}  C: ${c}`
        };
    }
    getAllParsedValues() {
        const controller = this;
        const axis = controller._cachedMeta.iScale.axis;
        return controller._cachedMeta._parsed.map(point => point[axis] ?? 0);
    }
    getMinMax(scale) {
        const controller = this;
        const parsed = controller._cachedMeta._parsed;
        const axis = controller._cachedMeta.iScale.axis;
        if (parsed.length < 2) {
            return { min: 0, max: 1 };
        }
        if (scale === controller._cachedMeta.iScale) {
            return { min: parsed[0][axis] ?? 0, max: parsed[parsed.length - 1][axis] ?? 0 };
        }
        let min = Number.POSITIVE_INFINITY;
        let max = Number.NEGATIVE_INFINITY;
        for (const point of parsed) {
            min = Math.min(min, point.l);
            max = Math.max(max, point.h);
        }
        return { min, max };
    }
    _getRuler() {
        const controller = this;
        const iScale = controller._cachedMeta.iScale;
        const axis = iScale.axis;
        const pixels = [];
        for (let i = 0; i < controller._cachedMeta.data.length; ++i) {
            const parsed = this.getParsed(i);
            pixels.push(iScale.getPixelForValue(parsed[axis] ?? 0));
        }
        const barThickness = controller.options.barThickness;
        const min = computeMinSampleSize(iScale, pixels);
        return {
            min,
            pixels,
            start: iScale._startPixel,
            end: iScale._endPixel,
            stackCount: controller._getStackCount(),
            scale: iScale,
            ratio: barThickness
                ? 1
                : controller.options.categoryPercentage * controller.options.barPercentage
        };
    }
    calculateElementProperties(index, ruler, reset, options) {
        const controller = this;
        const vScale = controller._cachedMeta.vScale;
        const base = vScale.getBasePixel();
        const iPixels = controller._calculateBarIndexPixels(index, ruler, options);
        // Defensive check for data access
        const data = controller.chart?.data?.datasets?.[controller.index]?.data?.[index];
        if (!data ||
            typeof data.o !== "number" ||
            typeof data.h !== "number" ||
            typeof data.l !== "number" ||
            typeof data.c !== "number") {
            // Return safe defaults if data is missing or invalid
            return {
                base,
                x: iPixels.center,
                y: base,
                width: iPixels.size,
                open: base,
                high: base,
                low: base,
                close: base,
                direction: "unchanged"
            };
        }
        const open = vScale.getPixelForValue(data.o);
        const high = vScale.getPixelForValue(data.h);
        const low = vScale.getPixelForValue(data.l);
        const close = vScale.getPixelForValue(data.c);
        const direction = data.c > data.o ? "up" : data.c < data.o ? "down" : "unchanged";
        return {
            base: reset ? base : low,
            x: iPixels.center,
            y: (low + high) / 2,
            width: iPixels.size,
            open,
            high,
            low,
            close,
            direction
        };
    }
    draw() {
        const controller = this;
        const chart = controller.chart;
        const rects = controller._cachedMeta.data;
        clipArea(chart.ctx, chart.chartArea);
        for (const rect of rects) {
            rect.draw(chart.ctx);
        }
        unclipArea(chart.ctx);
    }
}
FinancialController.overrides = {
    label: "",
    parsing: false,
    hover: {
        mode: "label"
    },
    datasets: {
        categoryPercentage: 0.8,
        barPercentage: 0.9,
        animation: {
            numbers: {
                type: "number",
                properties: ["x", "y", "base", "width", "open", "high", "low", "close"]
            }
        }
    },
    plugins: {
        tooltip: {
            intersect: false,
            mode: "index",
            callbacks: {
                label(ctx) {
                    const point = ctx.parsed;
                    if (!isNullOrUndef(point.y)) {
                        return "";
                    }
                    const { o, h, l, c } = point;
                    return `O: ${o}  H: ${h}  L: ${l}  C: ${c}`;
                }
            }
        }
    }
};
const currentDefaults = defaults;
currentDefaults.financial ?? (currentDefaults.financial = {
    color: {
        up: "rgba(80, 160, 115, 1)",
        down: "rgba(215, 85, 65, 1)",
        unchanged: "rgba(90, 90, 90, 1)"
    }
});
//# sourceMappingURL=financial.controller.js.map