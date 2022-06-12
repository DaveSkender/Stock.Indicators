namespace Skender.Stock.Indicators;

// PARABOLIC SAR (API)
public static partial class Indicator
{
    /// <include file='./info.xml' path='indicator/type[@name="Standard"]/*' />
    ///
    public static IEnumerable<ParabolicSarResult> GetParabolicSar<TQuote>(
        this IEnumerable<TQuote> quotes,
        decimal accelerationStep = 0.02m,
        decimal maxAccelerationFactor = 0.2m)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcParabolicSar(
                accelerationStep,
                maxAccelerationFactor,
                accelerationStep);

    /// <include file='./info.xml' path='indicator/type[@name="Extended"]/*' />
    ///
    public static IEnumerable<ParabolicSarResult> GetParabolicSar<TQuote>(
        this IEnumerable<TQuote> quotes,
        decimal accelerationStep,
        decimal maxAccelerationFactor,
        decimal initialFactor)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcParabolicSar(
                accelerationStep,
                maxAccelerationFactor,
                initialFactor);
}
