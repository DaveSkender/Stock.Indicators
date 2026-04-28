import { FinancialController } from "./financial.controller";
export declare class OhlcController extends FinancialController {
    static id: string;
    static defaults: {
        dataElementType: string;
        datasets: {
            barPercentage: number;
            categoryPercentage: number;
        };
    } & Record<string, unknown>;
    updateElements(elements: unknown[], start: number, count: number, mode: string): void;
}
//# sourceMappingURL=ohlc.controller.d.ts.map