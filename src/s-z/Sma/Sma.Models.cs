namespace Skender.Stock.Indicators;

[Serializable]
public class SmaResult : ResultBase
{
    public decimal? Sma { get; set; }
}

[Serializable]
public class SmaExtendedResult : SmaResult
{
    public double? Mad { get; set; } // mean absolute deviation
    public double? Mse { get; set; } // mean square error
    public double? Mape { get; set; } // mean absolute percentage error
}
