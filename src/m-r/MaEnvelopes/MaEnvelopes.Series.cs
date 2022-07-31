namespace Skender.Stock.Indicators;

// MOVING AVERAGE ENVELOPES (SERIES)
public static partial class Indicator
{
    // calculate series
    internal static IEnumerable<MaEnvelopeResult> CalcMaEnvelopes(
        this List<(DateTime, double)> tpList,
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

    private static IEnumerable<MaEnvelopeResult> MaEnvAlma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        double offsetRatio)
        => tpList.GetAlma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Alma,
            UpperEnvelope = x.Alma + (x.Alma * offsetRatio),
            LowerEnvelope = x.Alma - (x.Alma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvDema(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        double offsetRatio)
        => tpList.GetDema(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Dema,
            UpperEnvelope = x.Dema + (x.Dema * offsetRatio),
            LowerEnvelope = x.Dema - (x.Dema * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvEma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        double offsetRatio)
        => tpList.GetEma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Ema,
            UpperEnvelope = x.Ema + (x.Ema * offsetRatio),
            LowerEnvelope = x.Ema - (x.Ema * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvEpma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        double offsetRatio)
        => tpList.GetEpma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Epma,
            UpperEnvelope = x.Epma + (x.Epma * offsetRatio),
            LowerEnvelope = x.Epma - (x.Epma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvHma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        double offsetRatio)
        => tpList.GetHma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Hma,
            UpperEnvelope = x.Hma + (x.Hma * offsetRatio),
            LowerEnvelope = x.Hma - (x.Hma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvSma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        double offsetRatio)
        => tpList.GetSma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Sma,
            UpperEnvelope = x.Sma + (x.Sma * offsetRatio),
            LowerEnvelope = x.Sma - (x.Sma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvSmma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        double offsetRatio)
        => tpList.GetSmma(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Smma,
            UpperEnvelope = x.Smma + (x.Smma * offsetRatio),
            LowerEnvelope = x.Smma - (x.Smma * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvTema(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        double offsetRatio)
        => tpList.GetTema(lookbackPeriods)
        .Select(x => new MaEnvelopeResult(x.Date)
        {
            Centerline = x.Tema,
            UpperEnvelope = x.Tema + (x.Tema * offsetRatio),
            LowerEnvelope = x.Tema - (x.Tema * offsetRatio)
        });

    private static IEnumerable<MaEnvelopeResult> MaEnvWma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        double offsetRatio)
        => tpList.GetWma(lookbackPeriods)
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
