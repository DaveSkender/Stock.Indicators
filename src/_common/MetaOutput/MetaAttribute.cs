/// <summary>
/// Attribute to define metadata for indicator methods.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MetaAttribute"/> class.
/// </remarks>
/// <param name="kind">The kind of indicator method (series, buffer, stream, etc.).</param>
/// <param name="category">The category of the indicator's category (moving average, momentum, etc).</param>
/// <param name="chart">The type of chart commonly used for this indicator.</param>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class MetaAttribute(
    Kind kind,
    Category category,
    ChartType chart
    ) : Attribute
{
    /// <summary>
    /// Gets the kind of indicator associated with the attribute.
    /// </summary>
    public Kind Kind { get; } = kind;

    /// <summary>
    /// Gets the category associated with the attribute.
    /// </summary>
    public Category Category { get; } = category;

    /// <summary>
    /// Gets the type of chart associated with the attribute.
    /// </summary>
    public ChartType Chart { get; } = chart;
}
