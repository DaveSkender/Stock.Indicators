/*
 * Derived from chartjs-chart-financial (https://github.com/chartjs/chartjs-chart-financial)
 * Version reference: upstream plugin v0.2.x API surface.
 * License: MIT.
 */
import { Chart } from "chart.js";
import { merge } from "chart.js/helpers";
import { CandlestickElement } from "../elements/candlestick.element";
import { FinancialController } from "./financial.controller";
export class CandlestickController extends FinancialController {
    updateElements(elements, start, count, mode) {
        const controller = this;
        const dataset = controller.getDataset();
        const ruler = controller._ruler ?? this._getRuler();
        const firstOptions = controller.resolveDataElementOptions(start, mode);
        const sharedOptions = controller.getSharedOptions(firstOptions);
        const includeOptions = controller.includeOptions(mode, sharedOptions);
        controller.updateSharedOptions(sharedOptions, mode, firstOptions);
        for (let i = 0; i < count; i++) {
            const dataIndex = start + i;
            const options = sharedOptions ?? controller.resolveDataElementOptions(dataIndex, mode);
            const baseProperties = controller.calculateElementProperties(dataIndex, ruler, mode === "reset", options);
            const properties = {
                ...baseProperties,
                datasetLabel: dataset.label ?? "",
                color: dataset.color,
                borderColor: dataset.borderColor,
                borderWidth: dataset.borderWidth,
                ...(includeOptions ? { options } : {})
            };
            controller.updateElement(elements[dataIndex], dataIndex, properties, mode);
        }
    }
}
CandlestickController.id = "candlestick";
CandlestickController.defaults = merge({
    dataElementType: CandlestickElement.id
}, Chart.defaults.financial);
//# sourceMappingURL=candlestick.controller.js.map