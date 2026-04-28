import { computed, defineComponent, h, inject, nextTick, onMounted, onUnmounted, ref, watch } from "vue";
import { createApiClient, loadStaticIndicatorData } from "../api";
import { ChartManager } from "../charts";
import { applySelectionTokens, createDefaultSelection } from "../helpers";
import { setupIndyCharts } from "../setup";
import { indyChartsVitePressOptionsKey } from "./context";
import { chartSettingsFromOptions } from "./types";
const DEFAULT_BAR_COUNT = 250;
const DATA_UNAVAILABLE_ERROR_MESSAGE = "Chart data is currently unavailable. Check the API service and try again.";
const MISSING_SETUP_ERROR_MESSAGE = "setupIndyChartsForVitePress() has not been called.";
function slug(value) {
    return value
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, "-")
        .replace(/^-|-$/g, "");
}
function findListing(listings, uiid) {
    return listings.find(listing => listing.uiid.toLowerCase() === uiid.toLowerCase());
}
function registryConfig(options, indicator) {
    const registry = options.indicators ?? {};
    return indicator ? registry[indicator] : undefined;
}
function isAuthorFacingError(error) {
    if (!(error instanceof Error))
        return false;
    return (error.message === MISSING_SETUP_ERROR_MESSAGE ||
        error.message === "A chart indicator or config.uiid is required." ||
        error.message.startsWith("Indicator listing not found for uiid") ||
        error.message.endsWith("chart canvas is not available."));
}
function normalizeWindowSize(value, total) {
    if (total <= 0)
        return 0;
    const normalized = Number.isFinite(value) ? Math.floor(value) : 1;
    return Math.max(1, Math.min(normalized, total));
}
export const StockIndicatorChart = defineComponent({
    name: "StockIndicatorChart",
    props: {
        indicator: String,
        config: Object,
        barCount: Number
    },
    setup(props) {
        const options = inject(indyChartsVitePressOptionsKey);
        const phase = ref("idle");
        const title = ref("Stock indicator chart");
        const errorMessage = ref("Chart data is unavailable.");
        const chartType = ref("overlay");
        const overlayCanvas = ref(null);
        const oscillatorCanvas = ref(null);
        const rootId = computed(() => slug(props.config?.id ?? props.indicator ?? "chart"));
        const testIdPrefix = computed(() => `stock-indicator-chart-${rootId.value}`);
        let manager;
        let loadToken = 0;
        let disposed = false;
        let themeObserver;
        function destroyChart() {
            manager?.destroy();
            manager = undefined;
        }
        function shouldObserveDarkMode() {
            return options?.theme?.observeVitePressDarkMode ?? true;
        }
        function isDarkTheme() {
            if (typeof document !== "undefined" && shouldObserveDarkMode()) {
                return document.documentElement.classList.contains("dark");
            }
            return options?.theme?.isDarkTheme ?? false;
        }
        function currentSettings() {
            if (!options) {
                return { isDarkTheme: false, showTooltips: true };
            }
            return chartSettingsFromOptions(options, isDarkTheme());
        }
        function resolvedConfig() {
            if (!options) {
                throw new Error(MISSING_SETUP_ERROR_MESSAGE);
            }
            const indicator = props.indicator ?? options.defaults?.indicator;
            const registered = registryConfig(options, indicator);
            return {
                ...registered,
                ...props.config,
                uiid: props.config?.uiid ?? registered?.uiid ?? indicator
            };
        }
        async function loadChart() {
            const token = ++loadToken;
            destroyChart();
            phase.value = "loading";
            errorMessage.value = "Chart data is unavailable.";
            try {
                if (!options) {
                    throw new Error(MISSING_SETUP_ERROR_MESSAGE);
                }
                const config = resolvedConfig();
                if (!config.uiid) {
                    throw new Error("A chart indicator or config.uiid is required.");
                }
                const client = createApiClient(options.api);
                const [quotes, listings] = await Promise.all([client.getQuotes(), client.getListings()]);
                if (disposed || token !== loadToken)
                    return;
                const listing = findListing(listings, config.uiid);
                if (!listing) {
                    throw new Error(`Indicator listing not found for uiid "${config.uiid}".`);
                }
                const quoteCount = normalizeWindowSize(config.quoteCount ?? options.defaults?.quoteCount ?? DEFAULT_BAR_COUNT, quotes.length);
                const chartQuotes = quotes.slice(-quoteCount);
                if (chartQuotes.length === 0) {
                    phase.value = "empty";
                    return;
                }
                const selection = createDefaultSelection(listing, config.params, `${rootId.value}-`);
                if (config.results?.length) {
                    const wanted = new Set(config.results.map(result => result.toLowerCase()));
                    const filtered = selection.results.filter(result => wanted.has(result.dataName.toLowerCase()));
                    if (filtered.length === 0) {
                        console.warn(`[indy-charts] None of the requested results [${config.results.join(", ")}] ` +
                            `match available result names for uiid "${config.uiid}". ` +
                            `Available: [${selection.results.map(r => r.dataName).join(", ")}].`);
                    }
                    selection.results = filtered;
                }
                applySelectionTokens(selection);
                const rows = loadStaticIndicatorData(await client.getSelectionData(selection, listing));
                const chartRows = rows.slice(-chartQuotes.length);
                if (disposed || token !== loadToken)
                    return;
                if (chartRows.length === 0) {
                    phase.value = "empty";
                    return;
                }
                title.value = config.title ?? listing.name;
                chartType.value = listing.chartType;
                phase.value = "ready";
                await nextTick();
                if (disposed || token !== loadToken)
                    return;
                if (!overlayCanvas.value) {
                    throw new Error("Overlay chart canvas is not available.");
                }
                setupIndyCharts();
                const barCount = props.barCount ?? config.barCount ?? options.defaults?.barCount ?? DEFAULT_BAR_COUNT;
                const normalizedBarCount = normalizeWindowSize(barCount, chartQuotes.length);
                const chartManager = new ChartManager({ settings: currentSettings() });
                manager = chartManager;
                chartManager.initializeOverlay(overlayCanvas.value, chartQuotes, normalizedBarCount);
                chartManager.processSelectionData(selection, listing, chartRows);
                chartManager.displaySelection(selection, listing);
                if (listing.chartType === "oscillator") {
                    if (!oscillatorCanvas.value) {
                        throw new Error("Oscillator chart canvas is not available.");
                    }
                    chartManager.createOscillator(oscillatorCanvas.value, selection, listing);
                }
            }
            catch (error) {
                if (disposed || token !== loadToken)
                    return;
                destroyChart();
                phase.value = "error";
                errorMessage.value = isAuthorFacingError(error)
                    ? error.message
                    : DATA_UNAVAILABLE_ERROR_MESSAGE;
            }
        }
        function updateTheme() {
            manager?.updateTheme(currentSettings());
        }
        onMounted(() => {
            if (typeof document !== "undefined" &&
                typeof MutationObserver !== "undefined" &&
                shouldObserveDarkMode()) {
                themeObserver = new MutationObserver(updateTheme);
                themeObserver.observe(document.documentElement, {
                    attributes: true,
                    attributeFilter: ["class"]
                });
            }
            void loadChart();
        });
        onUnmounted(() => {
            disposed = true;
            loadToken += 1;
            themeObserver?.disconnect();
            destroyChart();
        });
        watch(() => [props.indicator, props.barCount, props.config], () => {
            if (!disposed && phase.value !== "idle") {
                void loadChart();
            }
        }, { deep: true });
        return () => h("section", { class: "indy-demo stock-indicator-chart", "data-testid": `${testIdPrefix.value}-root` }, [
            h("div", { class: "indy-demo__header" }, [
                h("p", { class: "indy-demo__title" }, title.value),
                h("p", { class: "indy-demo__hint" }, phase.value === "ready" ? "Live API data" : "")
            ]),
            phase.value === "loading"
                ? [
                    h("div", {
                        class: "indy-demo__status indy-demo__status--loading",
                        "data-testid": `${testIdPrefix.value}-loading`
                    }, "Loading chart data..."),
                    h("div", {
                        class: "indy-demo__stack indy-demo__stack--loading",
                        "data-testid": `${testIdPrefix.value}-loading-layout`
                    }, [
                        h("div", { class: "indy-demo__panel" }, [
                            h("p", { class: "indy-demo__panel-title" }, "Price + volume"),
                            h("div", {
                                class: "indy-demo__canvas-wrap indy-demo__canvas-wrap--overlay indy-demo__canvas-wrap--placeholder"
                            })
                        ])
                    ])
                ]
                : null,
            phase.value === "empty"
                ? h("div", {
                    class: "indy-demo__status indy-demo__status--error",
                    "data-testid": `${testIdPrefix.value}-empty`
                }, "No chart data is available.")
                : null,
            phase.value === "error"
                ? h("div", {
                    class: "indy-demo__status indy-demo__status--error",
                    "data-testid": `${testIdPrefix.value}-error`
                }, [
                    h("span", errorMessage.value),
                    h("div", { class: "indy-demo__status-actions" }, [
                        h("button", {
                            type: "button",
                            class: "indy-demo__retry",
                            onClick: () => void loadChart()
                        }, "Retry")
                    ])
                ])
                : null,
            phase.value === "ready"
                ? h("div", { class: "indy-demo__stack" }, [
                    h("div", { class: "indy-demo__panel" }, [
                        h("p", { class: "indy-demo__panel-title" }, "Price + volume"),
                        h("div", { class: "indy-demo__canvas-wrap indy-demo__canvas-wrap--overlay" }, [
                            h("canvas", {
                                ref: overlayCanvas,
                                class: "indy-demo__canvas",
                                "data-testid": `${testIdPrefix.value}-overlay-canvas`
                            })
                        ])
                    ]),
                    chartType.value === "oscillator"
                        ? h("div", { class: "indy-demo__panel" }, [
                            h("p", { class: "indy-demo__panel-title" }, title.value),
                            h("div", { class: "indy-demo__canvas-wrap indy-demo__canvas-wrap--oscillator" }, [
                                h("canvas", {
                                    ref: oscillatorCanvas,
                                    class: "indy-demo__canvas",
                                    "data-testid": `${testIdPrefix.value}-oscillator-canvas`
                                })
                            ])
                        ])
                        : null
                ])
                : null
        ]);
    }
});
//# sourceMappingURL=stock-indicator-chart.js.map