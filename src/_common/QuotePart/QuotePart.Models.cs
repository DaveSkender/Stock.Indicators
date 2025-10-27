namespace Skender.Stock.Indicators;

/// <summary>
/// Chainable component part of an <see cref="IQuote"/>.
/// </summary>
/// <param name="Timestamp">Date and time of record.</param>
/// <param name="Value"></param>
[Serializable]
public record QuotePart
(
    DateTime Timestamp,
    double Value
) : IReusable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuotePart"/> class using an <see cref="IReusable"/> object.
    /// </summary>
    /// <param name="reusable">The reusable object to initialize from.</param>
    public QuotePart(IReusable reusable)
        : this(reusable?.Timestamp ?? default,
               reusable?.Value ?? default)
    { }

    /// <inheritdoc/>
    public double Value { get; } = Value;
}
