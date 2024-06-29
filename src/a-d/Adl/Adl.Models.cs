namespace Skender.Stock.Indicators;

public readonly record struct AdlResult
(
    DateTime Timestamp,
    double Adl,
    double? MoneyFlowMultiplier = null,
    double? MoneyFlowVolume = null
) : IReusable
{
    double IReusable.Value => Adl;
}

public interface IAdl : IStreamObserver
{
    // no public parameters
}
