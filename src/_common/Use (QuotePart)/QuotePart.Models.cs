namespace Skender.Stock.Indicators;

/// <summary>
/// Chainable component part of an <see cref="IQuote"/>.
/// </summary>
public record QuotePart
(
    DateTime Timestamp,
    double Value
) : Reusable(Timestamp)
{
    public QuotePart(IReusable reusable)
        : this(reusable?.Timestamp ?? default,
               reusable?.Value ?? default)
    { }

    public override double Value { get; } = Value;
}
