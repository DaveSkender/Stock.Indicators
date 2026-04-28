const THRESHOLD_ORDER_OFFSET = 100;
export function baseDataset(r, c) {
    switch (r.lineType) {
        case "solid": {
            const lineDataset = {
                label: r.label,
                type: "line",
                data: [],
                yAxisID: "y",
                pointRadius: 0,
                borderWidth: r.lineWidth,
                borderColor: r.color,
                backgroundColor: r.color,
                fill: c.fill == null
                    ? false
                    : {
                        target: c.fill.target,
                        above: c.fill.colorAbove,
                        below: c.fill.colorBelow
                    },
                order: r.order
            };
            return lineDataset;
        }
        case "dash": {
            const dashDataset = {
                label: r.label,
                type: "line",
                data: [],
                yAxisID: "y",
                pointRadius: 0,
                borderWidth: r.lineWidth,
                borderDash: [3, 2],
                borderColor: r.color,
                backgroundColor: r.color,
                order: r.order
            };
            return dashDataset;
        }
        case "dots": {
            const dotsDataset = {
                label: r.label,
                type: "line",
                data: [],
                yAxisID: "y",
                pointRadius: r.lineWidth,
                pointBorderWidth: 0,
                pointBorderColor: r.color,
                pointBackgroundColor: r.color,
                showLine: false,
                order: r.order
            };
            return dotsDataset;
        }
        case "bar": {
            const barDataset = {
                label: r.label,
                type: "bar",
                data: [],
                yAxisID: "y",
                borderWidth: 0,
                borderColor: r.color,
                backgroundColor: r.color,
                order: r.order
            };
            if (c.stack) {
                barDataset.stack = c.stack;
            }
            return barDataset;
        }
        case "pointer": {
            const ptDataset = {
                label: r.label,
                type: "line",
                data: [],
                yAxisID: "y",
                pointRadius: r.lineWidth,
                pointBorderWidth: 0,
                pointBorderColor: r.color,
                pointBackgroundColor: r.color,
                pointStyle: "triangle",
                pointRotation: 0,
                showLine: false,
                order: r.order
            };
            return ptDataset;
        }
        case "none": {
            // hide instead of exclude 'none' lines,
            // otherwise, it breaks line offset fill
            const noneDataset = {
                label: r.label,
                type: "line",
                data: [],
                yAxisID: "y",
                showLine: false,
                pointRadius: 0,
                borderWidth: 0,
                borderColor: r.color,
                backgroundColor: r.color,
                fill: false,
                order: r.order
            };
            return noneDataset;
        }
    }
    throw new Error(`Unsupported lineType: "${r.lineType}" for result "${r.dataName}"`);
}
export function createThresholdDataset(threshold, firstResult, index) {
    // note: thresholds can't be annotated lines since
    // offset fill will only work between certain objects.
    const lineData = [];
    const sourceData = firstResult.dataset.data;
    sourceData.forEach((d) => {
        lineData.push({ x: d.x, y: threshold.value });
    });
    return {
        label: "threshold",
        type: "line",
        data: lineData,
        yAxisID: "y",
        pointRadius: 0,
        borderWidth: 2.5,
        borderDash: threshold.style === "dash" ? [5, 2] : [],
        borderColor: threshold.color,
        backgroundColor: threshold.color,
        spanGaps: true,
        fill: threshold.fill == null
            ? false
            : {
                target: threshold.fill.target,
                above: threshold.fill.colorAbove,
                below: threshold.fill.colorBelow
            },
        order: index + THRESHOLD_ORDER_OFFSET
    };
}
//# sourceMappingURL=datasets.js.map