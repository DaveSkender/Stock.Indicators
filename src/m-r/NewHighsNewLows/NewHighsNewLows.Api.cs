namespace Skender.Stock.Indicators;

/// <summary>
/// The New Highs-New Lows indicator ("NH-NL") is a trend indicator that displays the daily difference between the number of times the stock reached new 52-week highs and the number of times the stock reached new 52-week lows.
/// <para>
/// See <see href="https://dotnet.StockIndicators.dev/indicators/NewHighsNewLows/#content?utm_source=library&amp;utm_medium=inline-help&amp;utm_campaign=embedded"> documentation</see> for more information.
/// </para>
/// </summary>
public static partial class Indicator
{
    /// <summary>
    /// Retrieve the NewHighsNewLows for a particular symbol.
    /// </summary>
    /// <typeparam name="TQuote">Configurable Quote type.  See Guide for more information.</typeparam>
    /// <param name="quotes">Historical price quotes.</param>
    /// <param name="tradingDays">Optional.  Number of periods to validate the highs and lows.</param>
    /// <returns>Time series of NewHighsNewLows values.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Invalid parameter value provided.</exception>
    public static IEnumerable<NewHighsNewLowsResult> GetNewHighsNewLows<TQuote>(
        this IEnumerable<TQuote> quotes,
        int tradingDays = 252)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .GetNewHighsNewLows(tradingDays);

    /// <summary>
    /// Retrieve the NewHighsNewLows for an entire sector or market.
    /// </summary>
    /// <param name="newHighsNewLowsResults">Time collection of series of NewHighsNewLows values to calculate NewHighsNewLows for entire market or sector.</param>
    /// <returns>Time series of NewHighsNewLows values.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Invalid parameter value provided.</exception>
    public static IEnumerable<NewHighsNewLowsResult> GetNewHighsNewLows(
        this IEnumerable<IEnumerable<NewHighsNewLowsResult>> newHighsNewLowsResults) => newHighsNewLowsResults
            .GetNewHighsNewLows();
}
