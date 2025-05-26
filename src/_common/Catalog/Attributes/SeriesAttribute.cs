namespace Skender.Stock.Indicators;

/// <summary>
/// Classification attribute for a series-style indicator.
/// </summary>
/// <param name="id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="name">Name of the indicator</param>
/// <param name="category">Category of the indicator</param>
/// <param name="chartType">Chart type of the indicator</param>
/// <param name="legendOverride">Optional custom legend format to override the automatic format</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal sealed class SeriesAttribute(
    string id,
    string name,
    Category category,
    ChartType chartType,
    string? legendOverride = null
) : IndicatorAttribute(
        id: id,
        name: name,
        style: Style.Series,
        category: category,
        chartType: chartType,
        legendOverride: legendOverride
    )
{ }
