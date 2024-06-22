namespace Skender.Stock.Indicators;

// MOVING AVERAGE ENVELOPES (SERIES)

public static partial class Indicator
{
    // calculate series
    internal static IEnumerable<MaEnvelopeResult> CalcMaEnvelopes<T>(
        this List<T> tpList,
        int lookbackPeriods,
        double percentOffset,
        MaType movingAverageType)
        where T: IReusableResult
    {
        // check parameter arguments
        // note: most validations are done in variant methods
        MaEnvelopes.Validate(percentOffset);

        // initialize
        double offsetRatio = percentOffset / 100d;

        // get envelopes variant
        return movingAverageType switch {
            MaType.ALMA => tpList.MaEnvAlma(lookbackPeriods, offsetRatio),
            MaType.DEMA => tpList.MaEnvDema(lookbackPeriods, offsetRatio),
            MaType.EMA => tpList.MaEnvEma(lookbackPeriods, offsetRatio),
            MaType.EPMA => tpList.MaEnvEpma(lookbackPeriods, offsetRatio),
            MaType.HMA => tpList.MaEnvHma(lookbackPeriods, offsetRatio),
            MaType.SMA => tpList.MaEnvSma(lookbackPeriods, offsetRatio),
            MaType.SMMA => tpList.MaEnvSmma(lookbackPeriods, offsetRatio),
            MaType.TEMA => tpList.MaEnvTema(lookbackPeriods, offsetRatio),
            MaType.WMA => tpList.MaEnvWma(lookbackPeriods, offsetRatio),

            _ => throw new ArgumentOutOfRangeException(
                     nameof(movingAverageType), movingAverageType,
                     string.Format(
                         EnglishCulture,
                         "Moving Average Envelopes does not support {0}.",
                         Enum.GetName(typeof(MaType), movingAverageType)))
        };
    }

    private static IEnumerable<MaEnvelopeResult> MaEnvAlma<T>(
        this List<T> tpList,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusableResult
        => tpList.GetAlma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Alma,
            UpperEnvelope = x.Alma + (x.Alma * offsetRatio),
            LowerEnvelope = x.Alma - (x.Alma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvDema<T>(
        this List<T> tpList,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusableResult
        => tpList.GetDema(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Dema,
            UpperEnvelope = x.Dema + (x.Dema * offsetRatio),
            LowerEnvelope = x.Dema - (x.Dema * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvEma<T>(
        this List<T> tpList,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusableResult
        => tpList
        .GetEma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Ema,
            UpperEnvelope = x.Ema + (x.Ema * offsetRatio),
            LowerEnvelope = x.Ema - (x.Ema * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvEpma<T>(
        this List<T> tpList,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusableResult
        => tpList.GetEpma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Epma,
            UpperEnvelope = x.Epma + (x.Epma * offsetRatio),
            LowerEnvelope = x.Epma - (x.Epma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvHma<T>(
        this List<T> tpList,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusableResult
        => tpList.GetHma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Hma,
            UpperEnvelope = x.Hma + (x.Hma * offsetRatio),
            LowerEnvelope = x.Hma - (x.Hma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvSma<T>(
        this List<T> tpList,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusableResult
        => tpList.GetSma<T>(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Sma,
            UpperEnvelope = x.Sma + (x.Sma * offsetRatio),
            LowerEnvelope = x.Sma - (x.Sma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvSmma<T>(
        this List<T> tpList,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusableResult
        => tpList.GetSmma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Smma,
            UpperEnvelope = x.Smma + (x.Smma * offsetRatio),
            LowerEnvelope = x.Smma - (x.Smma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvTema<T>(
        this List<T> tpList,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusableResult
        => tpList.GetTema(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Tema,
            UpperEnvelope = x.Tema + (x.Tema * offsetRatio),
            LowerEnvelope = x.Tema - (x.Tema * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvWma<T>(
        this List<T> tpList,
        int lookbackPeriods,
        double offsetRatio)
        where T : IReusableResult
        => tpList.GetWma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult {
            Timestamp = x.Timestamp,
            Centerline = x.Wma,
            UpperEnvelope = x.Wma + (x.Wma * offsetRatio),
            LowerEnvelope = x.Wma - (x.Wma * offsetRatio)
        });
}
