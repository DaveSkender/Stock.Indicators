namespace Skender.Stock.Indicators;

/// <summary>
/// Reusable base result model for chainable indicators.
/// </summary>
/// <inheritdoc cref="IReusable"/>
public abstract record Reusable(
    DateTime Timestamp
) : IReusable
{
    // we've done it this way to avoid
    // having to seal the inherited classes
    // to enable inheritance, but without
    // instantiating the Value property

    public abstract double Value { get; }
}
