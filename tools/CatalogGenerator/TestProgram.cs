// This is just a simple test program to verify that attributes are correctly recognized
// by our source generator

namespace Stock.Indicators.Generator.Test;
#pragma warning disable CS9113 // Parameter is unread

// Add test attributes for development/testing purposes
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class SeriesAttribute(string id, string name) : Attribute
{
    public string Id { get; } = id;
    public string Name { get; } = name;
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class StreamHubAttribute(string id, string name) : Attribute
{
    public string Id { get; } = id;
    public string Name { get; } = name;
}

[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
public class BufferAttribute(string id, string name) : Attribute
{
    public string Id { get; } = id;
    public string Name { get; } = name;
}

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public class ParamAttribute(string displayName, double minValue, double maxValue, double defaultValue) : Attribute
{
    public string DisplayName { get; } = displayName;
    public double MinValue { get; } = minValue;
    public double MaxValue { get; } = maxValue;
    public double DefaultValue { get; } = defaultValue;
}

// Test class with indicator methods for each style
public static class TestIndicators
{
    // Series-style indicator
    [Series("SMA", "Simple Moving Average Series")]
    public static void ToSma(
        this IEnumerable<IQuote> quotes,

        [Param("Lookback Periods", 1, 500, 20)]
        int lookbackPeriods)
    {
        // This is just a test method - no implementation needed
    }

    // Stream-hub style indicator
    [StreamHub("RSI-STREAM", "RSI Stream Hub Indicator")]
    public static void ToRsiStream(
        this IEnumerable<IQuote> quotes,

        [Param("RSI Periods", 2, 100, 14)]
        int rsiPeriods)
    {
        // This is just a test method - no implementation needed
    }

    // Additional test indicators for each style
    [Series("TEST-SERIES", "Test Series Indicator Style")]
    public static void ToTestSeries(
        this IEnumerable<IQuote> quotes,

        [Param("Test Period", 1, 100, 14)]
        int period)
    {
        // This is just a test method - no implementation needed
    }

    [StreamHub("TEST-STREAM", "Test Stream Hub Indicator Style")]
    public static void ToTestStream(
        this IEnumerable<IQuote> quotes,

        [Param("Stream Period", 2, 50, 10)]
        int streamPeriod)
    {
        // This is just a test method - no implementation needed
    }
}

// Buffer-style indicator test classes (separate classes to avoid constructor conflicts)
[method: Buffer("BB-BUFFER", "Bollinger Bands Buffer Indicator")]
public class BollingerBandsBuffer(
    [Param("Lookback Periods", 2, 100, 20)]
    int lookbackPeriods,

    [Param("Standard Deviations", 0.1, 5.0, 2.0)]
    double standardDeviations)
{ }

[method: Buffer("TEST-BUFFER", "Test Buffer Indicator Style")]
public class TestBuffer(
    [Param("Buffer Size", 2, 200, 20)]
    int bufferSize,

    [Param("Smoothing Factor", 0.1, 1.0, 0.5)]
    double smoothingFactor)
{ }

// Simple dummy interfaces for test purposes
public interface IEnumerable<T> { }
public interface IQuote { }

internal class Program
{
    private static void Main()
    {
        Console.WriteLine("This is a test program for the CatalogGenerator.");
        Console.WriteLine("The source generator should process the following attribute types:");
        Console.WriteLine("- SeriesAttribute (for standard indicators)");
        Console.WriteLine("- StreamHubAttribute (for streaming indicators)");
        Console.WriteLine("- BufferAttribute (for buffer-style indicators)");
    }
}
