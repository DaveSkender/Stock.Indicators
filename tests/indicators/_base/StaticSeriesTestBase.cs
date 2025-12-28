namespace Test.Base;

/// <summary>
/// Base tests that all series indicators should have.
/// </summary>
public abstract class StaticSeriesTestBase : TestBaseWithPrecision
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
}
