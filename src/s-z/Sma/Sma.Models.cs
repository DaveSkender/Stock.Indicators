namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class SmaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Sma { get; set; }

    double IReusableResult.Value => Sma.Null2NaN();
}

[Serializable]
public sealed record class SmaAnalysis : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Sma { get; set; } // simple moving average
    public double? Mad { get; set; } // mean absolute deviation
    public double? Mse { get; set; } // mean square error
    public double? Mape { get; set; } // mean absolute percentage error

    double IReusableResult.Value => Sma.Null2NaN();
}

public interface ISma :
    IChainObserver<SmaResult>,
    IChainProvider
{
    int LookbackPeriods { get; }
}
