import type { ChartComponent } from "chart.js";
export declare const financialRegisterables: ReadonlyArray<ChartComponent>;
/**
 * Registers financial chart controllers/elements and required dependencies once.
 *
 * Derived from chartjs-chart-financial and adapted for Chart.js v4 + TypeScript.
 */
export declare function registerFinancialCharts(): void;
/** Internal test hook. */
export declare function __resetFinancialChartsRegistrationForTests(): void;
//# sourceMappingURL=register-financial.d.ts.map