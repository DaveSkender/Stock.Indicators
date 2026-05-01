import { FONT_FAMILY } from "./common";
export function commonLegendAnnotation(labelText, xPos, yPos, yAdj = 0, settings) {
    const fontColor = settings.isDarkTheme ? "#757575" : "#121316";
    const fillColor = settings.isDarkTheme ? "#12131680" : "#FAF9FD90";
    const legendFont = {
        family: FONT_FAMILY,
        size: 13,
        style: "normal",
        weight: "normal",
        lineHeight: 1
    };
    const annotation = {
        id: "legend",
        type: "label",
        content: [labelText],
        textAlign: "start",
        font: legendFont,
        color: fontColor,
        backgroundColor: fillColor,
        padding: 0,
        position: "start",
        xScaleID: "x",
        yScaleID: "y",
        xValue: xPos,
        yValue: yPos,
        xAdjust: 0,
        yAdjust: yAdj
    };
    return annotation;
}
//# sourceMappingURL=annotations.js.map