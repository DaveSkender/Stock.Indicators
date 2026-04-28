/*
 * Derived from chartjs-chart-financial (https://github.com/chartjs/chartjs-chart-financial)
 * Version reference: upstream plugin v0.2.x API surface.
 * License: MIT.
 */
import { Element } from "chart.js";
import { inRange } from "../helpers/bar-bounds";
export class FinancialElement extends Element {
    height() {
        return this.base - this.y;
    }
    inRange(mouseX, mouseY, useFinalPosition) {
        return inRange(this, mouseX, mouseY, useFinalPosition);
    }
    inXRange(mouseX, useFinalPosition) {
        return inRange(this, mouseX, null, useFinalPosition);
    }
    inYRange(mouseY, useFinalPosition) {
        return inRange(this, null, mouseY, useFinalPosition);
    }
    getRange(axis) {
        return axis === "x" ? this.width / 2 : this.height() / 2;
    }
    getCenterPoint(useFinalPosition) {
        const { x, low, high } = this.getProps(["x", "low", "high"], useFinalPosition);
        return {
            x,
            y: (high + low) / 2
        };
    }
    tooltipPosition(useFinalPosition) {
        const { x, open, close } = this.getProps(["x", "open", "close"], useFinalPosition);
        return {
            x,
            y: (open + close) / 2
        };
    }
}
//# sourceMappingURL=financial.element.js.map