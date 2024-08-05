namespace Skender.Stock.Indicators;

public record IchimokuResult
(
    DateTime Timestamp,
    decimal? TenkanSen,   // conversion line
    decimal? KijunSen,    // base line
    decimal? SenkouSpanA, // leading span A
    decimal? SenkouSpanB, // leading span B
    decimal? ChikouSpan   // lagging span
) : ISeries;
