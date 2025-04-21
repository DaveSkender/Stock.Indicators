namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for Quote Part Hub.
/// </summary>
public interface IQuotePart
{
    /// <summary>
    /// Gets the selected candle part.
    /// </summary>
    CandlePart CandlePartSelection { get; }

    // TODO: consider renaming to IBarPartHub, with IQuote to IBar
}
