namespace Skender.Stock.Indicators;

public readonly record struct HeikinAshiResult
(
    DateTime Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume
) : IQuote
{
    double IReusable.Value => (double)Close;
}
