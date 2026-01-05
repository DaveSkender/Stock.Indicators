namespace Skender.Stock.Indicators;

public interface IStreamHubBase<out TOut>
    where TOut : IReusable
{
    /// <summary>
    /// Read-only list of the stored cache values.
    /// </summary>
    IReadOnlyList<TOut> Results { get; }
}
