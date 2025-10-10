using System.Diagnostics.CodeAnalysis;
using Tests.Data;

namespace Regression;

/// <summary>
/// Cross-platform validation tests to ensure deterministic output across .NET target frameworks.
/// </summary>
/// <remarks>
/// These tests validate FR25 requirement: "Regression tests validate deterministic output 
/// across net8.0 and net9.0 target frameworks to ensure version migration compatibility."
/// Mathematical precision is NON-NEGOTIABLE per Constitution Principle I.
/// ANY difference between frameworks indicates a precision bug requiring investigation.
/// </remarks>
[TestClass, TestCategory("Regression")]
public class CrossPlatformValidationTests : TestBase
{
    /// <summary>
    /// Validates that all indicators produce identical results across net8.0 and net9.0 frameworks.
    /// This test compares current framework outputs against baselines generated on any framework.
    /// </summary>
    [TestMethod]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test method naming convention")]
    public void CrossPlatformValidation_AllIndicators()
    {
        // Test representative indicators covering different calculation complexities
        ValidateIndicator<SmaResult>("sma.standard.json", () => Quotes.ToSma(20));
        ValidateIndicator<EmaResult>("ema.standard.json", () => Quotes.ToEma(20));
        ValidateIndicator<RsiResult>("rsi.standard.json", () => Quotes.ToRsi(14));
        ValidateIndicator<MacdResult>("macd.standard.json", () => Quotes.ToMacd(12, 26, 9));
        ValidateIndicator<AdxResult>("adx.standard.json", () => Quotes.ToAdx(14));
        ValidateIndicator<BollingerBandsResult>("bb.standard.json", () => Quotes.ToBollingerBands(20, 2));
        ValidateIndicator<StochResult>("stoch.standard.json", () => Quotes.ToStoch(14, 3, 3));
        ValidateIndicator<AtrResult>("atr.standard.json", () => Quotes.ToAtr(14));
        ValidateIndicator<CciResult>("cci.standard.json", () => Quotes.ToCci(20));
        ValidateIndicator<HmaResult>("hma.standard.json", () => Quotes.ToHma());
    }

    /// <summary>
    /// Validates deterministic output for high-precision indicators.
    /// </summary>
    [TestMethod]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test method naming convention")]
    public void CrossPlatformValidation_HighPrecisionIndicators()
    {
        ValidateIndicator<FisherTransformResult>("fisher.standard.json", () => Quotes.ToFisherTransform(10));
        ValidateIndicator<HurstResult>("hurst.standard.json", () => Quotes.ToHurst(100));
        ValidateIndicator<MamaResult>("mama.standard.json", () => Quotes.ToMama(0.5, 0.05));
    }

    /// <summary>
    /// Helper method to validate an indicator against its baseline.
    /// Uses exact binary equality - zero tolerance.
    /// </summary>
    private static void ValidateIndicator<TResult>(string baselineFilename, Func<IReadOnlyList<TResult>> indicatorFunc)
        where TResult : ISeries
    {
        // Load baseline (generated on any framework)
        IReadOnlyList<TResult> expected = Data.Results<TResult>(baselineFilename);

        // Execute indicator on current framework
        IReadOnlyList<TResult> actual = indicatorFunc();

        // Compare with exact binary equality
        actual.AssertEquals(expected);
    }
}
