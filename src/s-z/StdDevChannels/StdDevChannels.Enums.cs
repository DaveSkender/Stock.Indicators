namespace Skender.Stock.Indicators;

/// <summary>
/// Field selection for Standard Deviation Channels indicator.
/// </summary>
public enum StdDevChannelsField
{
    /// <summary>
    /// Centerline (linear regression).
    /// </summary>
    Centerline = 0,

    /// <summary>
    /// Upper channel.
    /// </summary>
    UpperChannel = 1,

    /// <summary>
    /// Lower channel.
    /// </summary>
    LowerChannel = 2
}
