namespace Skender.Stock.Indicators;

public readonly record struct RenkoResult
(
    DateTime Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume,
    bool IsUp
) : IQuote
{
    double IReusable.Value => (double)Close;
}

