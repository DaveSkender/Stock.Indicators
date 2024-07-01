namespace Skender.Stock.Indicators;

// MOVING AVERAGE ENVELOPES (SERIES)

public static partial class Indicator
{
    // calculate series
    private static IEnumerable<MaEnvelopeResult> CalcMaEnvelopes<T>(
        this List<T> source,
        int lookbackPeriods,
        double percentOffset,
        MaType movingAverageType)
        where T : IReusable
    {
        // check parameter arguments
        // note: most validations are done in variant methods
        MaEnvelopes.Validate(percentOffset);

        // initialize
        double offsetRatio = percentOffset / 100d;

        // get envelopes variant
        return movingAverageType switch {
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
                         EnglishCulture,
                         "Moving Average Envelopes does not support {0}.",
                         Enum.GetName(typeof(MaType), movingAverageType)))
        };
    }

    private static IEnumerable<MaEnvelopeResult> MaEnvAlma<T>(
        this List<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.GetAlma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Alma,
            UpperEnvelope = x.Alma + x.Alma * offsetRatio,
            LowerEnvelope = x.Alma - x.Alma * offsetRatio
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvDema<T>(
        this List<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.GetDema(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Dema,
            UpperEnvelope = x.Dema + x.Dema * offsetRatio,
            LowerEnvelope = x.Dema - x.Dema * offsetRatio
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvEma<T>(
        this List<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source
        .GetEma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Ema,
            UpperEnvelope = x.Ema + x.Ema * offsetRatio,
            LowerEnvelope = x.Ema - x.Ema * offsetRatio
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvEpma<T>(
        this List<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.GetEpma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Epma,
            UpperEnvelope = x.Epma + x.Epma * offsetRatio,
            LowerEnvelope = x.Epma - x.Epma * offsetRatio
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvHma<T>(
        this List<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.GetHma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Hma,
            UpperEnvelope = x.Hma + x.Hma * offsetRatio,
            LowerEnvelope = x.Hma - x.Hma * offsetRatio
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvSma<T>(
        this List<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.GetSma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Sma,
            UpperEnvelope = x.Sma + x.Sma * offsetRatio,
            LowerEnvelope = x.Sma - x.Sma * offsetRatio
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvSmma<T>(
        this List<T> source,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => source.GetSmma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Smma,
            UpperEnvelope = x.Smma + x.Smma * offsetRatio,
            LowerEnvelope = x.Smma - x.Smma * offsetRatio
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvTema<T>(
        this List<T> tpList,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => tpList.GetTema(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Tema,
            UpperEnvelope = x.Tema + x.Tema * offsetRatio,
            LowerEnvelope = x.Tema - x.Tema * offsetRatio
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvWma<T>(
        this List<T> tpList,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusable
        => tpList.GetWma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Wma,
            UpperEnvelope = x.Wma + x.Wma * offsetRatio,
            LowerEnvelope = x.Wma - x.Wma * offsetRatio
        });
}
