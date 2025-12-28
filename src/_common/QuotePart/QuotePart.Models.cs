namespace Skender.Stock.Indicators;

/// <summary>
/// Chainable component part of an <see cref="IQuote"/>.
/// </summary>
/// <param name="Timestamp">Date and time of record.</param>
/// <param name="Value">Value of the quote part</param>
[Serializable]
public record QuotePart
(
    DateTime Timestamp,
    double Value
) : IReusable
{
    /// <inheritdoc/>
    public double Value { get; } = Value;
}
