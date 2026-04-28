import { indyChartsVitePressOptionsKey } from "./context";
import { StockIndicatorChart } from "./stock-indicator-chart";
export { StockIndicatorChart } from "./stock-indicator-chart";
export function setupIndyChartsForVitePress(app, options) {
    app.provide(indyChartsVitePressOptionsKey, options);
    app.component("StockIndicatorChart", StockIndicatorChart);
}
//# sourceMappingURL=index.js.map