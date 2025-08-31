namespace Skender.Stock.Indicators;

/// <summary>
/// Abstract base class for indicator stream hubs.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
/// <typeparam name="TResult">The type of the indicator result.</typeparam>
public abstract class IndicatorStreamHubBase<TIn, TResult> : ChainProvider<TIn, TResult>
    where TIn : IReusable
    where TResult : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndicatorStreamHubBase{TIn, TResult}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="indicatorName">The name of the indicator.</param>
    /// <param name="parameters">Optional parameters for the indicator name.</param>
    protected IndicatorStreamHubBase(
        IChainProvider<TIn> provider,
        string indicatorName,
        params object[] parameters) : base(provider)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        string paramString = parameters.Length > 0 
            ? $"({string.Join(",", parameters)})" 
            : string.Empty;
        hubName = $"{indicatorName}{paramString}";

        // Don't call ValidateParameters here as derived properties aren't set yet
        // Validation will be called manually after derived constructor completes
    }

    /// <summary>
    /// Initializes the indicator after construction. Call this from derived constructor.
    /// </summary>
    protected void Initialize()
    {
        ValidateParameters();
        Reinitialize();
    }

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public abstract int LookbackPeriods { get; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <summary>
    /// Validates the indicator parameters.
    /// </summary>
    protected abstract void ValidateParameters();
}