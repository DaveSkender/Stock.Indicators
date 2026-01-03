namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for TdiGm configuration parameters
/// </summary>
public interface ITdiGm
{
    int RsiPeriod { get; }
    int BandLength { get; }
    int FastLength { get; }
    int SlowLength { get; }
}
