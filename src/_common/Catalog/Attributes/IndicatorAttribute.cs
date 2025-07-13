namespace Skender.Stock.Indicators;

/// <summary>
/// Base classification attribute for indicators used with the catalog system.
/// </summary>
/// <param name="id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="style">Indicator style (series, buffer, stream hub)</param>
/// <remarks>
/// <para>
/// This is the base attribute class for indicator classification. Do not use this attribute directly.
/// Instead, use one of the derived attributes:
/// <list type="bullet">
///   <item><see cref="SeriesIndicatorAttribute"/> - For series-style indicators</item>
///   <item><see cref="StreamIndicatorAttribute"/> - For stream-style indicators</item>
///   <item><see cref="BufferIndicatorAttribute"/> - For buffer-style indicators</item>
/// </list>
/// </para>
/// <para>
/// When an indicator is marked with one of these attributes, the catalog system:
/// <list type="bullet">
///   <item>Validates explicit catalog listings for consistency</item>
///   <item>Extract parameter information from method signatures</item>
///   <item>Include the indicator in search and filtering operations</item>
///   <item>Group indicators by style and category</item>
/// </list>
/// </para>
/// </remarks>
internal abstract class IndicatorAttribute(
    string id,
    Style style
) : Attribute
{
    internal string Id { get; } = id;
    internal Style Style { get; } = style;
}
