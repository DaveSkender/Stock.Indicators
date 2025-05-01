namespace Skender.Stock.Indicators;

/// <summary>
/// Parameter attribute for time-series indicator parameters used in catalog generation.
/// </summary>
/// <param name="displayName">The display name of the parameter.</param>
/// <param name="seriesType">The type of elements in the series.</param>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class ParamSeriesAttribute(
    string displayName,
    SeriesType seriesType = SeriesType.Reusable
) : ParamAttribute<object>(
    displayName: displayName,
    defaultValue: null!,  // Value doesn't matter for series parameters
    minValue: null!,      // Value doesn't matter for series parameters
    maxValue: null!       // Value doesn't matter for series parameters
)
{
    /// <summary>
    /// Gets the type of elements in the series.
    /// </summary>
    public SeriesType SeriesType { get; } = seriesType;
}

/// <summary>
/// Specifies the element type requirements for series parameters.
/// </summary>
internal enum SeriesType
{
    /// <summary>
    /// Elements must implement IQuote interface.
    /// </summary>
    Quote,

    /// <summary>
    /// Elements must implement IReusable interface.
    /// </summary>
    Reusable
}
