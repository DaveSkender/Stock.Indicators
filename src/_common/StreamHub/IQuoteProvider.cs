namespace Skender.Stock.Indicators;

/// <inheritdoc/>
public interface IQuoteProvider<out T> : IChainProvider<T>
   where T : IQuote
{
    /// <summary>
    /// Gets the read-only list of quotes.
    /// </summary>
    IReadOnlyList<T> Quotes { get; }
}
