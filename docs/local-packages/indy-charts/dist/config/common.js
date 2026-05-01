import "chartjs-adapter-date-fns";
import { enUS } from "date-fns/locale";
export const FONT_FAMILY = "'Google Sans', Roboto, Verdana, Helvetica, Arial, sans-serif";
export function baseChartOptions(settings) {
    const options = {
        plugins: {
            title: {
                display: false
            },
            legend: {
                display: false
            },
            tooltip: {
                enabled: settings.showTooltips,
                intersect: false
            },
            annotation: {
                clip: false,
                annotations: {}
            }
        },
        layout: {
            padding: {
                top: 0,
                left: 1,
                bottom: 0,
                right: 1
            },
            autoPadding: false
        },
        responsive: true,
        maintainAspectRatio: false,
        animation: false,
        font: {
            family: FONT_FAMILY
        },
        scales: {
            x: defaultXAxisOptions(),
            y: {
                alignToPixels: true,
                display: true,
                type: "linear",
                axis: "y",
                position: "right",
                beginAtZero: false,
                ticks: {
                    display: true,
                    mirror: true,
                    font: {
                        family: FONT_FAMILY,
                        size: 12,
                        lineHeight: 1
                    },
                    showLabelBackdrop: true,
                    backdropColor: settings.isDarkTheme ? "#12131680" : "#FAF9FD90",
                    backdropPadding: {
                        top: 0,
                        left: 5,
                        bottom: 0,
                        right: 0
                    },
                    padding: 0
                },
                border: {
                    display: false
                },
                grid: {
                    drawOnChartArea: true,
                    drawTicks: false,
                    lineWidth: 0.5,
                    color: settings.isDarkTheme ? "#2E2E2E" : "#E0E0E0"
                }
            }
        }
    };
    return options;
}
export function defaultXAxisOptions() {
    const timeUnit = "day";
    const options = {
        alignToPixels: true,
        display: false,
        offset: false,
        type: "timeseries",
        time: {
            unit: timeUnit
        },
        adapters: {
            date: {
                locale: enUS
            }
        },
        ticks: {
            display: false,
            source: "auto",
            padding: 0,
            autoSkip: true,
            maxRotation: 0,
            minRotation: 0,
            font: {
                size: 9
            }
        },
        border: {
            display: false
        },
        grid: {
            display: false,
            drawOnChartArea: false
        }
    };
    return options;
}
//# sourceMappingURL=common.js.map