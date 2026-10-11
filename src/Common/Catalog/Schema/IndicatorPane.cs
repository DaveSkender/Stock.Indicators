namespace FacioQuo.Stock.Indicators;

/// <summary>
/// Represents how to draw one of an indicator's chart panes.
/// </summary>
/// <remarks>
/// Only some panes are listed; an unlisted pane carries no range or reference line data here.
/// </remarks>
[Serializable]
public record IndicatorPane
{
    /// <summary>
    /// Gets the pane name, matching <see cref="IndicatorResult.ChartPane"/> on the results it draws.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the fixed lower bound of the pane's axis, or <c>null</c> to autoscale.
    /// </summary>
    public double? Minimum { get; init; }

    /// <summary>
    /// Gets the fixed upper bound of the pane's axis, or <c>null</c> to autoscale.
    /// </summary>
    public double? Maximum { get; init; }

    /// <summary>
    /// Gets the values of the pane's horizontal reference lines, in drawing order,
    /// such as the overbought and oversold levels; <c>null</c> when it has none.
    /// </summary>
    public IReadOnlyList<double>? Thresholds { get; init; }
}
