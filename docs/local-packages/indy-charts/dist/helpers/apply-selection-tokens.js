/**
 * Replace `[P1]`, `[P2]`, … placeholder tokens in a selection's label and
 * result labels with the corresponding parameter values.
 *
 * Mutates the supplied {@link selection} in place and returns it for chaining.
 *
 * @param selection - The indicator selection whose labels contain tokens.
 * @returns The same selection reference with tokens replaced.
 */
export function applySelectionTokens(selection) {
    selection.params.forEach((param, index) => {
        if (param.value == null)
            return;
        const token = new RegExp(`\\[P${index + 1}\\]`, "g");
        const valueText = String(param.value);
        selection.label = selection.label.replace(token, valueText);
        selection.results.forEach(result => {
            result.label = result.label.replace(token, valueText);
        });
    });
    return selection;
}
//# sourceMappingURL=apply-selection-tokens.js.map