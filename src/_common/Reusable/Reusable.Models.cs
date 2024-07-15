namespace Skender.Stock.Indicators;

/// <summary>
/// Reusable base result model for chainable indicators.
/// </summary>
/// <inheritdoc cref="IReusable"/>
public record Reusable
(
    DateTime Timestamp,
    double Value
) : IReusable;

// TODO: make abstract, use QuotePart as instantiation
