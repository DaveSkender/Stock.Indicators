namespace Skender.Stock.Indicators;

/// <summary>
/// Chainable component part of an <see cref="IQuote"/>.
/// </summary>
[Serializable]
public record QuotePart : IReusable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuotePart"/> class.
    /// </summary>
    /// <param name="Timestamp">Date and time of record.</param>
    /// <param name="Value">Value of the quote part</param>
    [JsonConstructor]
    public QuotePart(DateTime Timestamp, double Value)
    {
        this.Timestamp = Timestamp;
        this.Value = Value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuotePart"/> class using an <see cref="IReusable"/> object.
    /// </summary>
    /// <param name="reusable">The reusable object to initialize from.</param>
    public QuotePart(IReusable reusable)
        : this(reusable?.Timestamp ?? default,
               reusable?.Value ?? default)
    { }

    /// <inheritdoc/>
    public DateTime Timestamp { get; init; }

    /// <inheritdoc/>
    public double Value { get; init; }
}
