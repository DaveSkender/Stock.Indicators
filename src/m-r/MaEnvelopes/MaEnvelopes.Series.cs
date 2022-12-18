using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// MOVING AVERAGE ENVELOPES (SERIES)
public static partial class Indicator
{
    // calculate series
    internal static IEnumerable<MaEnvelopeResult> CalcMaEnvelopes(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods,
        double percentOffset,
        MaType movingAverageType)
    {
        // check parameter arguments
        // note: most validations are done in variant methods
        ValidateMaEnvelopes(percentOffset);

        // initialize
        double offsetRatio = percentOffset / 100d;

        // get envelopes variant
        return movingAverageType switch
        {
            MaType.ALMA => tpColl.MaEnvAlma(lookbackPeriods, offsetRatio),
            MaType.DEMA => tpColl.MaEnvDema(lookbackPeriods, offsetRatio),
            MaType.EMA => tpColl.MaEnvEma(lookbackPeriods, offsetRatio),
            MaType.EPMA => tpColl.MaEnvEpma(lookbackPeriods, offsetRatio),
            MaType.HMA => tpColl.MaEnvHma(lookbackPeriods, offsetRatio),
            MaType.SMA => tpColl.MaEnvSma(lookbackPeriods, offsetRatio),
            MaType.SMMA => tpColl.MaEnvSmma(lookbackPeriods, offsetRatio),
            MaType.TEMA => tpColl.MaEnvTema(lookbackPeriods, offsetRatio),
            MaType.WMA => tpColl.MaEnvWma(lookbackPeriods, offsetRatio),

            _ => throw new ArgumentOutOfRangeException(
                     nameof(movingAverageType), movingAverageType,
                     string.Format(
                         EnglishCulture,
                         "Moving Average Envelopes does not support {0}.",
                         Enum.GetName(typeof(MaType), movingAverageType)))
        };
    }

    private static IEnumerable<MaEnvelopeResult> MaEnvAlma(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods,
        double offsetRatio)
        => tpColl.GetAlma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Alma,
            UpperEnvelope = x.Alma + (x.Alma * offsetRatio),
            LowerEnvelope = x.Alma - (x.Alma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvDema(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods,
        double offsetRatio)
        => tpColl.GetDema(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Dema,
            UpperEnvelope = x.Dema + (x.Dema * offsetRatio),
            LowerEnvelope = x.Dema - (x.Dema * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvEma(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods,
        double offsetRatio)
        => tpColl.GetEma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Ema,
            UpperEnvelope = x.Ema + (x.Ema * offsetRatio),
            LowerEnvelope = x.Ema - (x.Ema * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvEpma(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods,
        double offsetRatio)
        => tpColl.GetEpma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Epma,
            UpperEnvelope = x.Epma + (x.Epma * offsetRatio),
            LowerEnvelope = x.Epma - (x.Epma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvHma(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods,
        double offsetRatio)
        => tpColl.GetHma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Hma,
            UpperEnvelope = x.Hma + (x.Hma * offsetRatio),
            LowerEnvelope = x.Hma - (x.Hma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvSma(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods,
        double offsetRatio)
        => tpColl.GetSma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Sma,
            UpperEnvelope = x.Sma + (x.Sma * offsetRatio),
            LowerEnvelope = x.Sma - (x.Sma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvSmma(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods,
        double offsetRatio)
        => tpColl.GetSmma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Smma,
            UpperEnvelope = x.Smma + (x.Smma * offsetRatio),
            LowerEnvelope = x.Smma - (x.Smma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvTema(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods,
        double offsetRatio)
        => tpColl.GetTema(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Tema,
            UpperEnvelope = x.Tema + (x.Tema * offsetRatio),
            LowerEnvelope = x.Tema - (x.Tema * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvWma(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods,
        double offsetRatio)
        => tpColl.GetWma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Wma,
            UpperEnvelope = x.Wma + (x.Wma * offsetRatio),
            LowerEnvelope = x.Wma - (x.Wma * offsetRatio)
        });

    // parameter validation
    private static void ValidateMaEnvelopes(
        double percentOffset)
    {
        // check parameter arguments
        if (percentOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(percentOffset), percentOffset,
                "Percent Offset must be greater than 0 for Moving Average Envelopes.");
        }
    }
}
