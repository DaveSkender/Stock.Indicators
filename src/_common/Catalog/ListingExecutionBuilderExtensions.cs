namespace Skender.Stock.Indicators;

/// <summary>
/// Extension methods for creating and working with custom indicator builders.
/// </summary>
public static class ListingExecutionBuilderExtensions
{
    /// <summary>
    /// Creates a customizable indicator builder from an indicator listing.
    /// </summary>
    /// <param name="listing">The base indicator listing.</param>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="value">Value to set</param>
    /// <returns>A <see cref="ListingExecutionBuilder"/> for fluent configuration.</returns>
    public static ListingExecutionBuilder WithParamValue(
        this IndicatorListing listing,
        string parameterName,
        object value)
            => new ListingExecutionBuilder(listing).WithParamValue(parameterName, value);

    /// <summary>
    /// Creates a customizable indicator builder from an indicator listing with multiple parameters.
    /// </summary>
    /// <param name="listing">The base indicator listing.</param>
    /// <param name="parameters">Dictionary of parameter names and values.</param>
    /// <returns>A <see cref="ListingExecutionBuilder"/> for fluent configuration.</returns>
    public static ListingExecutionBuilder WithParams(
        this IndicatorListing listing,
        Dictionary<string, object> parameters)
            => new ListingExecutionBuilder(listing).WithParams(parameters);

    /// <summary>
    /// Creates a customizable indicator builder from an indicator listing and sets the source quotes.
    /// </summary>
    /// <param name="listing">The base indicator listing.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>A <see cref="ListingExecutionBuilder"/> with quotes set.</returns>
    public static ListingExecutionBuilder From(
        this IndicatorListing listing,
        IEnumerable<IQuote> quotes)
            => new ListingExecutionBuilder(listing).FromSource(quotes);

    /// <summary>
    /// Creates a customizable indicator builder from an indicator listing and sets a series source.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the series, which must implement <see cref="IReusable"/>.
    /// </typeparam>
    /// <param name="listing">The base indicator listing.</param>
    /// <param name="series">The series data to process.</param>
    /// <param name="parameterName">The name of the series parameter. If null, attempts to find the first series parameter.</param>
    /// <returns>A <see cref="ListingExecutionBuilder"/> with the series parameter set.</returns>
    public static ListingExecutionBuilder From<T>(
        this IndicatorListing listing,
        IReadOnlyList<T> series,
        string? parameterName = null)
        where T : IReusable
        => new ListingExecutionBuilder(listing).FromSource(series, parameterName);

    /// <summary>
    /// Creates a customizable indicator builder from an indicator listing and sets a series source.
    /// Alias for From&lt;T&gt; to provide consistent naming with the fluent API.
    /// </summary>
    /// <typeparam name="T">The type of elements in the series, which must implement IReusable.</typeparam>
    /// <param name="listing">The base indicator listing.</param>
    /// <param name="series">The series data to process.</param>
    /// <param name="parameterName">The name of the series parameter. If null, attempts to find the first series parameter.</param>
    /// <returns>A <see cref="ListingExecutionBuilder"/> with the series parameter set.</returns>
    public static ListingExecutionBuilder FromSource<T>(
        this IndicatorListing listing,
        IReadOnlyList<T> series,
        string? parameterName = null)
        where T : IReusable
        => new ListingExecutionBuilder(listing).FromSource(series, parameterName);

    /// <summary>
    /// Executes an indicator using the default parameters from the catalog.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="listing">The indicator listing.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>The indicator results.</returns>
    public static IReadOnlyList<TResult> Execute<TResult>(
        this IndicatorListing listing,
        IEnumerable<IQuote> quotes)
        where TResult : class
        => ListingExecutor.Execute<TResult>(quotes, listing);

    /// <summary>
    /// Alternative syntax: Execute an indicator from quotes using a custom indicator builder.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="customIndicator">The custom indicator configuration.</param>
    /// <returns>The indicator results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required parameter is null</exception>
    public static IReadOnlyList<TResult> Execute<TResult>(
        this IEnumerable<IQuote> quotes,
        ListingExecutionBuilder customIndicator)
        where TResult : class
        => customIndicator == null
            ? throw new ArgumentNullException(nameof(customIndicator))
            : customIndicator.FromSource(quotes).Execute<TResult>();

    /// <summary>
    /// Alternative syntax: Execute an indicator from a series using a custom indicator builder.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the source series, which must implement IReusable.</typeparam>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="series">The series data to process.</param>
    /// <param name="customIndicator">The custom indicator configuration.</param>
    /// <param name="parameterName">The name of the series parameter. If null, attempts to find the first series parameter.</param>
    /// <returns>The indicator results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required parameter is null</exception>
    public static IReadOnlyList<TResult> Execute<TSource, TResult>(
        this IReadOnlyList<TSource> series,
        ListingExecutionBuilder customIndicator,
        string? parameterName = null)
        where TSource : IReusable
        where TResult : class
        => customIndicator == null
            ? throw new ArgumentNullException(nameof(customIndicator))
            : customIndicator.FromSource(series, parameterName).Execute<TResult>();
}
