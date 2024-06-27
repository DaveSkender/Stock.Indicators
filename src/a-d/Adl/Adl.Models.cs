namespace Skender.Stock.Indicators;

public record struct AdlResult
(
    DateTime Timestamp,
    double Adl,
    double? MoneyFlowMultiplier = null,
    double? MoneyFlowVolume = null)
    : IReusable
{
    readonly double IReusable.Value => Adl;
}

public interface IAdl : IStreamObserver
{
    // no public parameters
}
