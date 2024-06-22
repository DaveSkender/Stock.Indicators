namespace Skender.Stock.Indicators;

// QUOTE MODELS

public interface IQuote : IReusableResult
{
    decimal Open { get; }
    decimal High { get; }
    decimal Low { get; }
    decimal Close { get; }
    decimal Volume { get; }
}

/// <summary>
/// Built-in Quote type.
/// Custom IQuote types are also supported.
/// </summary>
public record struct Quote(
    DateTime Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume)
    : IQuote
{
    readonly double IReusableResult.Value
        => (double)Close;
}

internal record struct QuoteD(
    DateTime Timestamp,
    double Open,
    double High,
    double Low,
    double Close,
    double Volume);
