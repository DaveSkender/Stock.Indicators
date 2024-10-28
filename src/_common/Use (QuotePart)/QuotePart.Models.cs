namespace Skender.Stock.Indicators;

/// <summary>
/// Chainable component part of an <see cref="IQuote"/>.
/// </summary>
[Serializable]
public record QuotePart
(
    DateTime Timestamp,
    double Value
) : IReusable
{
    public QuotePart(IReusable reusable)
        : this(reusable?.Timestamp ?? default,
               reusable?.Value ?? default)
    { }

    public double Value { get; } = Value;
}
