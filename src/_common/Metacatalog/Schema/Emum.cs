namespace Skender.Stock.Indicators;

#pragma warning disable CA1008 // Enums should have zero value
// TODO: fix or add zero value

/// <summary>
/// Specifies the order in which elements are rendered on the chart.
/// </summary>
[Serializable]
public enum Order
{
    /// <summary>
    /// Render in front of all other elements.
    /// </summary>
    Front = 1,

    /// <summary>
    /// Render behind most elements but in front of the price.
    /// </summary>
    Behind = 50,

    // [reserved] Price is 75/76

    /// <summary>
    /// Render behind the price.
    /// </summary>
    BehindPrice = 80,

    /// <summary>
    /// Render behind all other elements.
    /// </summary>
    Back = 95

    // [reserved] Thresholds are 99
}
