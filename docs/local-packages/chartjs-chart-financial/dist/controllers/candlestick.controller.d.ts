import { FinancialController } from "./financial.controller";
export declare class CandlestickController extends FinancialController {
    static id: string;
    static defaults: {
        dataElementType: string;
    } & Record<string, unknown>;
    updateElements(elements: unknown[], start: number, count: number, mode: string): void;
}
//# sourceMappingURL=candlestick.controller.d.ts.map