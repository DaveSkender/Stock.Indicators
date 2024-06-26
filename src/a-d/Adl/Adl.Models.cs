namespace Skender.Stock.Indicators;

public record struct AdlResult
(
    DateTime Timestamp,
    double Adl,
    double? MoneyFlowMultiplier = null,
    double? MoneyFlowVolume = null)
    : IReusableResult
{
    readonly double IReusableResult.Value => Adl;
}

public interface IAdl : IStreamObserver
{
    // no public parameters
}
