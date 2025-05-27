/*
 * NOTE: This file contains simplified example code for documentation purposes only.
 * It is not part of the actual implementation and is not intended to compile.
 * Some code elements are simplified for clarity and may not follow all best practices.
 * See the actual implementation files for proper usage patterns.
 */

namespace Skender.Stock.Indicators;

/// <summary>
/// Example implementations of different indicator styles for documentation purposes.
/// These examples are for illustration only and are not part of the actual implementation.
/// </summary>
public static class IndicatorStyleExamples
{
    /// <summary>
    /// Base class for indicator results in examples (for documentation purposes only)
    /// </summary>
    public abstract class ResultBase
    {
        /// <summary>
        /// Date of the result
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// Constructor with required date
        /// </summary>
        protected ResultBase(DateTime date)
        {
            Date = date;
        }
    }

    #region Series Indicator Example

    /// <summary>
    /// Example of a Series-style indicator implementation.
    /// Series indicators process a collection of input values and return results for each input.
    /// </summary>
    public static partial class SeriesIndicatorExample
    {
        /// <summary>
        /// Example method with SeriesIndicator attribute.
        /// </summary>
        /// <param name="source">Source data collection</param>
        /// <param name="lookbackPeriods">Number of periods to calculate</param>
        /// <returns>Collection of results</returns>
        [SeriesIndicator("SIE")]
        public static IReadOnlyList<SeriesResult> ToSeries<T>(
            this IReadOnlyList<T> source,
            int lookbackPeriods)
            where T : IReusable
        {
            // Implementation would go here
            return new List<SeriesResult>().AsReadOnly();
        }

        /// <summary>
        /// Sample result class for a series indicator.
        /// </summary>
        public class SeriesResult : ResultBase
        {
            public SeriesResult(DateTime timestamp) : base(timestamp) { }

            /// <summary>
            /// Main result value of the indicator.
            /// </summary>
            public decimal? Value { get; set; }
        }

        /// <summary>
        /// Catalog listing for this indicator.
        /// </summary>
        public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
            .WithName("Series Indicator Example")
            .WithId("SIE")
            .WithStyle(Style.Series) // This matches the attribute style
            .WithCategory(Category.General)
            .AddParameter<int>(
                parameterName: "lookbackPeriods", // Must match method parameter name exactly
                displayName: "Lookback Period",
                description: "Number of periods to calculate",
                isRequired: true,
                minimum: 1)
            .AddResult(
                dataName: "Value", // Must match result property name
                displayName: "Value",
                dataType: ResultType.Default,
                isDefault: true)
            .Build();
    }

    #endregion

    #region Stream Indicator Example

    /// <summary>
    /// Example of a Stream-style indicator implementation.
    /// Stream indicators process values one at a time in a streaming fashion.
    /// </summary>
    public static partial class StreamIndicatorExample
    {
        /// <summary>
        /// Example method with StreamIndicator attribute.
        /// </summary>
        /// <returns>A stream indicator instance</returns>
        [StreamIndicator("STE")]
        public static StreamCalculator GetStream()
        {
            return new StreamCalculator();
        }

        /// <summary>
        /// Stream calculator for processing values incrementally.
        /// </summary>
        public class StreamCalculator
        {
            private readonly List<StreamResult> _results = new();

            /// <summary>
            /// Add a new data point to the stream.
            /// </summary>
            /// <param name="timestamp">Date of the data point</param>
            /// <param name="value">Value to process</param>
            /// <returns>The latest result</returns>
            public StreamResult Add(DateTime timestamp, decimal value)
            {
                // Implementation would go here
                var result = new StreamResult(timestamp) { Value = value };
                _results.Add(result);
                return result;
            }

            /// <summary>
            /// Get all results calculated so far.
            /// </summary>
            public IReadOnlyList<StreamResult> Results => _results.AsReadOnly();
        }

        /// <summary>
        /// Sample result class for a stream indicator.
        /// </summary>
        public class StreamResult : ResultBase
        {
            public StreamResult(DateTime timestamp) : base(timestamp) { }

            /// <summary>
            /// Main result value of the indicator.
            /// </summary>
            public decimal? Value { get; set; }
        }

        /// <summary>
        /// Catalog listing for this indicator.
        /// </summary>
        public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
            .WithName("Stream Indicator Example")
            .WithId("STE")
            .WithStyle(Style.Stream) // This matches the attribute style
            .WithCategory(Category.General)
            // No input parameters for the factory method
            .AddResult(
                dataName: "Value", // Must match result property name
                displayName: "Value",
                dataType: ResultType.Default,
                isDefault: true)
            .Build();
    }

    #endregion

    #region Buffer Indicator Example

    /// <summary>
    /// Example of a Buffer-style indicator implementation.
    /// Buffer indicators maintain internal state and calculate based on a rolling window of values.
    /// </summary>
    public static partial class BufferIndicatorExample
    {
        /// <summary>
        /// Example method with BufferIndicator attribute.
        /// </summary>
        /// <param name="lookbackPeriods">Size of the buffer</param>
        /// <returns>A buffer indicator instance</returns>
        [BufferIndicator("BIE")]
        public static BufferCalculator GetBuffer(int lookbackPeriods)
        {
            return new BufferCalculator(lookbackPeriods);
        }

        /// <summary>
        /// Buffer calculator for processing values in a fixed-size window.
        /// </summary>
        public class BufferCalculator
        {
            private readonly int _lookbackPeriods;
            private readonly List<BufferResult> _results = new();

            /// <summary>
            /// Initialize a new buffer calculator.
            /// </summary>
            /// <param name="lookbackPeriods">Size of the buffer</param>
            public BufferCalculator(int lookbackPeriods)
            {
                _lookbackPeriods = lookbackPeriods;
            }

            /// <summary>
            /// Add a new value to the buffer.
            /// </summary>
            /// <param name="timestamp">Date of the value</param>
            /// <param name="value">Value to add</param>
            /// <returns>The latest result</returns>
            public BufferResult Add(DateTime timestamp, decimal value)
            {
                // Implementation would go here
                var result = new BufferResult(timestamp) { Value = value };
                _results.Add(result);

                // If we exceed the buffer size, remove oldest
                if (_results.Count > _lookbackPeriods)
                {
                    _results.RemoveAt(0);
                }

                return result;
            }

            /// <summary>
            /// Get all results in the buffer.
            /// </summary>
            public IReadOnlyList<BufferResult> Results => _results.AsReadOnly();
        }

        /// <summary>
        /// Sample result class for a buffer indicator.
        /// </summary>
        public class BufferResult : ResultBase
        {
            public BufferResult(DateTime timestamp) : base(timestamp) { }

            /// <summary>
            /// Main result value of the indicator.
            /// </summary>
            public decimal? Value { get; set; }
        }

        /// <summary>
        /// Catalog listing for this indicator.
        /// </summary>
        public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
            .WithName("Buffer Indicator Example")
            .WithId("BIE")
            .WithStyle(Style.Buffer) // This matches the attribute style
            .WithCategory(Category.General)
            .AddParameter<int>(
                parameterName: "lookbackPeriods", // Must match method parameter name exactly
                displayName: "Lookback Period",
                description: "Size of the buffer",
                isRequired: true,
                minimum: 1)
            .AddResult(
                dataName: "Value", // Must match result property name
                displayName: "Value",
                dataType: ResultType.Default,
                isDefault: true)
            .Build();
    }

    #endregion
}
