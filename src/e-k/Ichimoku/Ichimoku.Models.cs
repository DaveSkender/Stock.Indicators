namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of an Ichimoku Cloud calculation.
/// </summary>
/// <param name="Timestamp">Timestamp of the result.</param>
/// <param name="TenkanSen">Value of the Tenkan-sen (conversion line).</param>
/// <param name="KijunSen">Value of the Kijun-sen (base line).</param>
/// <param name="SenkouSpanA">Value of the Senkou Span A (leading span A).</param>
/// <param name="SenkouSpanB">Value of the Senkou Span B (leading span B).</param>
/// <param name="ChikouSpan">Value of the Chikou Span (lagging span).</param>
[Serializable]
public record IchimokuResult
(
   DateTime Timestamp,
   decimal? TenkanSen,   // conversion line
   decimal? KijunSen,    // base line
   decimal? SenkouSpanA, // leading span A
   decimal? SenkouSpanB, // leading span B
   decimal? ChikouSpan   // lagging span
) : ISeries;
