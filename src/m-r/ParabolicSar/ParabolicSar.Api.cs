namespace Skender.Stock.Indicators;

// PARABOLIC SAR (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/type[@name="Standard"]/*' />
    ///
    public static IEnumerable<ParabolicSarResult> GetParabolicSar<TQuote>(
        this IEnumerable<TQuote> quotes,
        double accelerationStep = 0.02,
        double maxAccelerationFactor = 0.2)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcParabolicSar(
                accelerationStep,
                maxAccelerationFactor,
                accelerationStep);

    // SERIES, from TQuote (alt)
    /// <include file='./info.xml' path='info/type[@name="Extended"]/*' />
    ///
    public static IEnumerable<ParabolicSarResult> GetParabolicSar<TQuote>(
        this IEnumerable<TQuote> quotes,
        double accelerationStep,
        double maxAccelerationFactor,
        double initialFactor)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcParabolicSar(
                accelerationStep,
                maxAccelerationFactor,
                initialFactor);
}
