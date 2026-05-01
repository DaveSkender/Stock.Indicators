/*
 * Derived from chartjs-chart-financial (https://github.com/chartjs/chartjs-chart-financial)
 * Version reference: upstream plugin v0.2.x API surface.
 * License: MIT.
 */
import { defaults } from "chart.js";
import { valueOrDefault } from "chart.js/helpers";
import { FinancialElement } from "./financial.element";
export class OhlcElement extends FinancialElement {
    draw(ctx) {
        const me = this;
        const { x, open, high, low, close } = me;
        const ohlcDefaults = defaults.elements.ohlc;
        const armLengthRatio = valueOrDefault(me.armLengthRatio, ohlcDefaults.armLengthRatio ?? 0.8);
        let armLength = valueOrDefault(me.armLength, ohlcDefaults.armLength);
        if (armLength === null) {
            armLength = me.width * armLengthRatio * 0.5;
        }
        const isUp = me.direction === "up" || (me.direction == null && close < open);
        const isDown = me.direction === "down" || (me.direction == null && close > open);
        if (isUp) {
            ctx.strokeStyle = valueOrDefault(me.color?.up, ohlcDefaults.color?.up) ?? "#000000";
        }
        else if (isDown) {
            ctx.strokeStyle = valueOrDefault(me.color?.down, ohlcDefaults.color?.down) ?? "#000000";
        }
        else {
            ctx.strokeStyle =
                valueOrDefault(me.color?.unchanged, ohlcDefaults.color?.unchanged) ?? "#000000";
        }
        ctx.lineWidth = valueOrDefault(me.lineWidth, ohlcDefaults.lineWidth ?? 2);
        ctx.beginPath();
        ctx.moveTo(x, high);
        ctx.lineTo(x, low);
        ctx.moveTo(x - (armLength ?? 0), open);
        ctx.lineTo(x, open);
        ctx.moveTo(x + (armLength ?? 0), close);
        ctx.lineTo(x, close);
        ctx.stroke();
    }
}
OhlcElement.id = "ohlc";
//# sourceMappingURL=ohlc.element.js.map