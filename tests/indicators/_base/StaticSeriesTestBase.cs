namespace Test.Base;

/// <summary>
/// Base tests that all series indicators should have.
/// </summary>
public abstract class StaticSeriesTestBase : TestBase
{
    /// <summary>
    /// Tests default use case and parameters arguments
    /// </summary>
    public abstract void DefaultParameters_ReturnsExpectedResults();

    /// <summary>
    /// Tests proper handling of incompatible quote data
    /// </summary>
    public abstract void BadQuotes_DoesNotFail();

    /// <summary>
    /// Tests that empty quotes sets return empty results set
    /// </summary>
    public abstract void NoQuotes_ReturnsEmpty();


    /// <summary>
    /// Precision constants for BeApproximately() assertions for manually calculated values only.
    /// Maps to equivalent .Round() precision expectations for Series-style indicators only.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: DO NOT USE this to address floating point precision issues.
    /// </remarks>
    protected const double Money3 = 0.0005;    // 3 decimal places: ±0.0005 (equivalent to .Round(3))
    /// <summary>
    /// 4 decimal places: &#177;0.00005 (equivalent to .Round(4))
    /// </summary>
    protected const double Money4 = 0.00005;
    /// <summary>
    /// 5 decimal places: &#177;0.000005 (equivalent to .Round(5))
    /// </summary>
    protected const double Money5 = 0.000005;
    /// <summary>
    /// 6 decimal places: &#177;0.0000005 (equivalent to .Round(6))
    /// </summary>
    protected const double Money6 = 0.0000005;
}
