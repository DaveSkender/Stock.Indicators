namespace Tests.Data;

/// <summary>
/// Base tests that all series indicators should have.
/// </summary>
public abstract class StaticSeriesTestBase : TestBase
{
    /// <summary>
    /// Tests default use case and parameters arguments
    /// </summary>
    public abstract void Standard();

    /// <summary>
    /// Tests proper handling of incompatible quote data
    /// </summary>
    public abstract void BadData();

    /// <summary>
    /// Tests that empty quotes sets return empty results set
    /// </summary>
    public abstract void NoQuotes();
}
