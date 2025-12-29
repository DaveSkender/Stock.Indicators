namespace Skender.Stock.Indicators;

/// <summary>
/// Observer for dual-stream indicators that process synchronized pairs of reusable inputs.
/// </summary>
/// <typeparam name="T">
/// The type of input data (must be IReusable).
/// </typeparam>
/// <remarks>
/// This interface marks observers that consume paired input values from two synchronized
/// data sources (e.g., Correlation, Beta, PRS). Both input series must be synchronized
/// with matching timestamps.
/// </remarks>
public interface IPairsObserver<in T> : IStreamObserver<T>
    where T : IReusable;
