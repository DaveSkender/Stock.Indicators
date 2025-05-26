namespace Skender.Stock.Indicators;

/// <summary>
/// Classification attribute for an indicator for catalog generation.
/// </summary>
/// <param name="id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="style">Indicator style (series, buffer, stream hub)</param>
internal abstract class IndicatorAttribute(
    string id,
    Style style
) : Attribute
{
    public string Id { get; } = id;
    public Style Style {get;} = style;
}
