using System.Globalization;

namespace Skender.Stock.Indicators;

#pragma warning disable IDE0072 // Missing cases in switch statement

/// <summary>
/// Provides methods for calculating Moving Average Envelopes for a series of quotes.
/// </summary>
public static partial class MaEnvelopes
{
    /// <summary>
    /// Converts a list of source values to Moving Average Envelope results.
    /// </summary>
    /// <typeparam name="T">The type of the source values, which must implement <see cref="IReusable"/>.</typeparam>
    /// <param name="source">The list of source values to transform.</param>
    /// <param name="lookbackPeriods">The number of periods for the moving average.</param>
    /// <param name="percentOffset">The percentage offset for the envelopes. Default is 2.5.</param>
    /// <param name="movingAverageType">The type of moving average to use. Default is SMA.</param>
    /// <returns>A list of Moving Average Envelope results.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the moving average type is not supported.</exception>
    [Series("MA-ENV", "Moving Average Envelopes", Category.PriceChannel, ChartType.Overlay)]
    public static IReadOnlyList<MaEnvelopeResult> ToMaEnvelopes<T>(
        this IReadOnlyList<T> source,
        [ParamNum<int>("Lookback Periods", 20, 1, 250)]
        int lookbackPeriods,
        [ParamNum<double>("Percent Offset", 2.5, 0.1, 10)]
        double percentOffset = 2.5,
        [ParamEnum<MaType>("Moving Average Type", MaType.SMA)]
        MaType movingAverageType = MaType.SMA)
        where T : IReusable
    {
        // check parameter arguments
        // note: most validations are done in variant methods
        Validate(percentOffset);

        // initialize
        double offsetRatio = percentOffset / 100d;

        // get envelopes variant
        IEnumerable<MaEnvelopeResult> results = movingAverageType
        switch {
            MaType.ALMA => source.MaEnvAlma(lookbackPeriods, offsetRatio),
            MaType.DEMA => source.MaEnvDema(lookbackPeriods, offsetRatio),
            MaType.EMA => source.MaEnvEma(lookbackPeriods, offsetRatio),
            MaType.EPMA => source.MaEnvEpma(lookbackPeriods, offsetRatio),
            MaType.HMA => source.MaEnvHma(lookbackPeriods, offsetRatio),
            MaType.SMA => source.MaEnvSma(lookbackPeriods, offsetRatio),
            MaType.SMMA => source.MaEnvSmma(lookbackPeriods, offsetRatio),
            MaType.TEMA => source.MaEnvTema(lookbackPeriods, offsetRatio),
            MaType.WMA => source.MaEnvWma(lookbackPeriods, offsetRatio),

            _ => throw new ArgumentOutOfRangeException(
                    nameof(movingAverageType), movingAverageType,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Moving Average Envelopes does not support {0}.",
                        Enum.GetName(movingAverageType)))
        };

        return results.ToList();
    }

    private static IEnumerable<MaEnvelopeResult> MaEnvAlma<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.ToAlma(lookbackPeriods, offset: 0.85, sigma: 6)
        .Select(x => new MaEnvelopeResult(
            Timestamp: x.Timestamp,
            Centerline: x.Alma,
            UpperEnvelope: x.Alma + (x.Alma * offsetRatio),
            LowerEnvelope: x.Alma - (x.Alma * offsetRatio)));

    private static IEnumerable<MaEnvelopeResult> MaEnvDema<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.ToDema(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(
            Timestamp: x.Timestamp,
            Centerline: x.Dema,
            UpperEnvelope: x.Dema + (x.Dema * offsetRatio),
            LowerEnvelope: x.Dema - (x.Dema * offsetRatio)));

    private static IEnumerable<MaEnvelopeResult> MaEnvEma<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.ToEma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(
            Timestamp: x.Timestamp,
            Centerline: x.Ema,
            UpperEnvelope: x.Ema + (x.Ema * offsetRatio),
            LowerEnvelope: x.Ema - (x.Ema * offsetRatio)));

    private static IEnumerable<MaEnvelopeResult> MaEnvEpma<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.ToEpma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(
            Timestamp: x.Timestamp,
            Centerline: x.Epma,
            UpperEnvelope: x.Epma + (x.Epma * offsetRatio),
            LowerEnvelope: x.Epma - (x.Epma * offsetRatio)));

    private static IEnumerable<MaEnvelopeResult> MaEnvHma<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.ToHma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(
            Timestamp: x.Timestamp,
            Centerline: x.Hma,
            UpperEnvelope: x.Hma + (x.Hma * offsetRatio),
            LowerEnvelope: x.Hma - (x.Hma * offsetRatio)));

    private static IEnumerable<MaEnvelopeResult> MaEnvSma<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.ToSma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(
            Timestamp: x.Timestamp,
            Centerline: x.Sma,
            UpperEnvelope: x.Sma + (x.Sma * offsetRatio),
            LowerEnvelope: x.Sma - (x.Sma * offsetRatio)));

    private static IEnumerable<MaEnvelopeResult> MaEnvSmma<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.ToSmma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(
            Timestamp: x.Timestamp,
            Centerline: x.Smma,
            UpperEnvelope: x.Smma + (x.Smma * offsetRatio),
            LowerEnvelope: x.Smma - (x.Smma * offsetRatio)));

    private static IEnumerable<MaEnvelopeResult> MaEnvTema<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.ToTema(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(
            Timestamp: x.Timestamp,
            Centerline: x.Tema,
            UpperEnvelope: x.Tema + (x.Tema * offsetRatio),
            LowerEnvelope: x.Tema - (x.Tema * offsetRatio)));

    private static IEnumerable<MaEnvelopeResult> MaEnvWma<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.ToWma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(
            Timestamp: x.Timestamp,
            Centerline: x.Wma,
            UpperEnvelope: x.Wma + (x.Wma * offsetRatio),
            LowerEnvelope: x.Wma - (x.Wma * offsetRatio)));
}
