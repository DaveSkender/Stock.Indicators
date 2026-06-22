using System.Collections.ObjectModel;
using System.Globalization;

namespace Test.Application;

/// <summary>
/// Test application for Skender.Stock.Indicators v2 API
/// This application demonstrates all public interfaces available in the library
/// to facilitate migration testing to v3.
/// </summary>
public static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 0)
        {
            Console.WriteLine("Arguments received: " + string.Join(", ", args));
        }

        Console.WriteLine("===========================================");
        Console.WriteLine("Testing Skender.Stock.Indicators v2");
        Console.WriteLine("===========================================\n");

        // Generate test data
        (IEnumerable<Quote> quotes, IEnumerable<(DateTime, double)> tuples) = GenerateTestData();

        // Test all indicator categories
        TestIndicators(quotes, tuples);
        TestUtilities(quotes);
        TestResultUtilities(quotes);

        Console.WriteLine("\n===========================================");
        Console.WriteLine("All tests completed successfully!");
        Console.WriteLine("===========================================");
    }

    /// <summary>
    /// Generates sufficient test data for all indicators
    /// </summary>
    private static (IEnumerable<Quote>, IEnumerable<(DateTime, double)>) GenerateTestData()
    {
        Console.WriteLine("Generating test data...");

        CultureInfo culture = CultureInfo.InvariantCulture;
        List<Quote> quotesList = [];
        DateTime startDate = DateTime.Parse("2022-01-03", culture);
        const decimal basePrice = 100m;

        for (int i = 0; i < 150; i++)
        {
            // Skip weekends
            DateTime currentDate = startDate.AddDays(i);
            if (currentDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                continue;
            }

            // Generate realistic-looking OHLCV data
            decimal open = basePrice + (i * 0.5m) + (i % 5 * 0.3m);
            decimal high = open + (2m + (i % 3 * 0.5m));
            decimal low = open - (1m + (i % 4 * 0.3m));
            decimal close = open + (((i % 7) - 3) * 0.4m);
            decimal volume = 1000m + (i * 10m);

            quotesList.Add(new Quote {
                Date = currentDate,
                Open = open,
                High = high,
                Low = low,
                Close = close,
                Volume = volume
            });
        }

        IEnumerable<Quote> quotes = quotesList;
        IEnumerable<(DateTime, double)> tuples = quotes.Select(static x => (x.Date, (double)x.Close));

        Console.WriteLine($"Generated {quotesList.Count} quotes for testing\n");

        return (quotes, tuples);
    }

    /// <summary>
    /// Tests all indicator methods
    /// </summary>
    private static void TestIndicators(IEnumerable<Quote> quotes, IEnumerable<(DateTime, double)> tuples)
    {
        Console.WriteLine("Testing indicators...");

        // A-D INDICATORS
        _ = quotes.GetAdl();
        _ = quotes.GetAdx(lookbackPeriods: 14);
        _ = quotes.GetAlligator();
        _ = quotes.GetAlligator(jawPeriods: 13, jawOffset: 8, teethPeriods: 8, teethOffset: 5, lipsPeriods: 5, lipsOffset: 3);
        _ = tuples.GetAlligator(jawPeriods: 13, jawOffset: 8, teethPeriods: 8, teethOffset: 5, lipsPeriods: 5, lipsOffset: 3);
        _ = quotes.GetAlma(lookbackPeriods: 9, offset: 0.85, sigma: 6);
        _ = tuples.GetAlma(lookbackPeriods: 9, offset: 0.85, sigma: 6);
        _ = quotes.GetAroon(lookbackPeriods: 25);
        _ = quotes.GetAtr(lookbackPeriods: 14);
        _ = quotes.GetAtrStop(lookbackPeriods: 14);
        _ = quotes.GetAtrStop(lookbackPeriods: 21, multiplier: 3, endType: EndType.Close);
        _ = quotes.GetAwesome();
        _ = quotes.GetAwesome(fastPeriods: 5, slowPeriods: 34);
        _ = tuples.GetAwesome(fastPeriods: 5, slowPeriods: 34);

        // B INDICATORS
        _ = quotes.GetBeta(quotesMarket: quotes, lookbackPeriods: 20);
        _ = quotes.GetBeta(quotesMarket: quotes, lookbackPeriods: 20, type: BetaType.All);
        _ = quotes.GetBeta(quotesMarket: quotes, lookbackPeriods: 20, type: BetaType.Down);
        _ = quotes.GetBeta(quotesMarket: quotes, lookbackPeriods: 20, type: BetaType.Standard);
        _ = quotes.GetBeta(quotesMarket: quotes, lookbackPeriods: 20, type: BetaType.Up);
        _ = tuples.GetBeta(mrktTuple: tuples, lookbackPeriods: 20);
        _ = quotes.GetBollingerBands(lookbackPeriods: 20, standardDeviations: 2);
        _ = tuples.GetBollingerBands(lookbackPeriods: 20, standardDeviations: 2);
        _ = quotes.GetBop();
        _ = quotes.GetBop(smoothPeriods: 14);

        // C INDICATORS
        _ = quotes.GetDoji(maxPriceChangePercent: 0.1);
        _ = quotes.GetMarubozu(minBodyPercent: 95);
        _ = quotes.GetCci(lookbackPeriods: 20);
        _ = quotes.GetChaikinOsc();
        _ = quotes.GetChaikinOsc(fastPeriods: 3, slowPeriods: 10);
        _ = quotes.GetChandelier(lookbackPeriods: 22, multiplier: 3, type: ChandelierType.Long);
        _ = quotes.GetChop(lookbackPeriods: 14);
        _ = quotes.GetCmf(lookbackPeriods: 20);
        _ = quotes.GetCmo(lookbackPeriods: 14);
        _ = tuples.GetCmo(lookbackPeriods: 14);
        _ = quotes.GetConnorsRsi(rsiPeriods: 3, streakPeriods: 2, rankPeriods: 100);
        _ = tuples.GetConnorsRsi(rsiPeriods: 3, streakPeriods: 2, rankPeriods: 100);
        _ = quotes.GetCorrelation(quotesB: quotes, lookbackPeriods: 20);
        _ = tuples.GetCorrelation(tuplesB: tuples, lookbackPeriods: 20);

        // D INDICATORS
        _ = quotes.GetDema(lookbackPeriods: 20);
        _ = tuples.GetDema(lookbackPeriods: 20);
        _ = quotes.GetDonchian(lookbackPeriods: 20);
        _ = quotes.GetDpo(lookbackPeriods: 20);
        _ = tuples.GetDpo(lookbackPeriods: 20);
        _ = quotes.GetDynamic(lookbackPeriods: 20);
        _ = quotes.GetDynamic(lookbackPeriods: 20, kFactor: 0.6);
        _ = tuples.GetDynamic(lookbackPeriods: 20, kFactor: 0.6);

        // E-G INDICATORS
        _ = quotes.GetElderRay(lookbackPeriods: 13);
        IEnumerable<EmaResult> ema1 = quotes.GetEma(lookbackPeriods: 20);
        _ = tuples.GetEma(lookbackPeriods: 20);
        _ = quotes.GetEpma(lookbackPeriods: 20);
        _ = tuples.GetEpma(lookbackPeriods: 20);
        _ = quotes.GetFcb(windowSpan: 2);
        _ = quotes.GetFisherTransform(lookbackPeriods: 10);
        _ = tuples.GetFisherTransform(lookbackPeriods: 10);
        _ = quotes.GetForceIndex(lookbackPeriods: 2);
        _ = quotes.GetFractal();
        _ = quotes.GetFractal(leftSpan: 2, rightSpan: 2, endType: EndType.HighLow);
        _ = quotes.GetGator();
        _ = tuples.GetGator();

        // H-K INDICATORS
        _ = quotes.GetHeikinAshi();
        _ = quotes.GetHma(lookbackPeriods: 20);
        _ = tuples.GetHma(lookbackPeriods: 20);
        _ = quotes.GetHtTrendline();
        _ = tuples.GetHtTrendline();
        _ = quotes.GetHurst(lookbackPeriods: 100);
        _ = tuples.GetHurst(lookbackPeriods: 100);
        _ = quotes.GetIchimoku();
        _ = quotes.GetIchimoku(tenkanPeriods: 9, kijunPeriods: 26, senkouBPeriods: 52);
        _ = quotes.GetKama(erPeriods: 10, fastPeriods: 2, slowPeriods: 30);
        _ = tuples.GetKama(erPeriods: 10, fastPeriods: 2, slowPeriods: 30);
        _ = quotes.GetKeltner(emaPeriods: 20, multiplier: 2, atrPeriods: 10);
        _ = quotes.GetKvo();
        _ = quotes.GetKvo(fastPeriods: 34, slowPeriods: 55, signalPeriods: 13);

        // M-P INDICATORS
        _ = quotes.GetMacd(fastPeriods: 12, slowPeriods: 26, signalPeriods: 9);
        _ = tuples.GetMacd(fastPeriods: 12, slowPeriods: 26, signalPeriods: 9);
        _ = quotes.GetMaEnvelopes(lookbackPeriods: 20, percentOffset: 3.5, movingAverageType: MaType.SMA);
        _ = tuples.GetMaEnvelopes(lookbackPeriods: 20, percentOffset: 3.5, movingAverageType: MaType.SMA);
        _ = quotes.GetMama();
        _ = quotes.GetMama(fastLimit: 0.5, slowLimit: 0.05);
        _ = tuples.GetMama(fastLimit: 0.5, slowLimit: 0.05);
        _ = quotes.GetMfi(lookbackPeriods: 14);
        _ = quotes.GetObv();
        _ = quotes.GetObv(smaPeriods: 3);
        _ = quotes.GetParabolicSar(accelerationStep: 0.02, maxAccelerationFactor: 0.2);
        _ = quotes.GetParabolicSar(accelerationStep: 0.02, maxAccelerationFactor: 0.2, initialFactor: 0.02);
        _ = quotes.GetPivotPoints(windowSize: PeriodSize.Week);
        _ = quotes.GetPivotPoints(windowSize: PeriodSize.Week, pointType: PivotPointType.Standard);
        _ = quotes.GetPivots();
        _ = quotes.GetPivots(leftSpan: 2, rightSpan: 2, maxTrendPeriods: 20, endType: EndType.HighLow);
        _ = quotes.GetPmo(timePeriods: 35, smoothPeriods: 20, signalPeriods: 10);
        _ = tuples.GetPmo(timePeriods: 35, smoothPeriods: 20, signalPeriods: 10);
        _ = quotes.GetPrs(quotesBase: quotes);
        _ = quotes.GetPrs(quotesBase: quotes, lookbackPeriods: 5, smaPeriods: 3);
        _ = tuples.GetPrs(tupleBase: tuples, lookbackPeriods: 5, smaPeriods: 3);
        _ = quotes.GetPvo();
        _ = quotes.GetPvo(fastPeriods: 12, slowPeriods: 26, signalPeriods: 9);

        // R-S INDICATORS
        _ = quotes.GetRenko(brickSize: 2.0m);
        _ = quotes.GetRenko(brickSize: 2.0m, endType: EndType.Close);
        _ = quotes.GetRenkoAtr(atrPeriods: 14, endType: EndType.Close);
        _ = quotes.GetRoc(lookbackPeriods: 20);
        _ = quotes.GetRoc(lookbackPeriods: 20, smaPeriods: 3);
        _ = tuples.GetRoc(lookbackPeriods: 20);
        _ = quotes.GetRocWb(lookbackPeriods: 20, emaPeriods: 9, stdDevPeriods: 2);
        _ = tuples.GetRocWb(lookbackPeriods: 20, emaPeriods: 9, stdDevPeriods: 2);
        _ = quotes.GetRollingPivots(windowPeriods: 20, offsetPeriods: 2);
        _ = quotes.GetRollingPivots(windowPeriods: 14, offsetPeriods: 7, pointType: PivotPointType.Standard);
        _ = quotes.GetRsi(lookbackPeriods: 14);
        _ = tuples.GetRsi(lookbackPeriods: 14);
        _ = quotes.GetSlope(lookbackPeriods: 20);
        _ = tuples.GetSlope(lookbackPeriods: 20);
        _ = quotes.GetSmaAnalysis(lookbackPeriods: 20);
        _ = tuples.GetSmaAnalysis(lookbackPeriods: 20);
        IEnumerable<SmaResult> sma1 = quotes.GetSma(lookbackPeriods: 20);
        _ = tuples.GetSma(lookbackPeriods: 20);
        _ = quotes.GetSmi(lookbackPeriods: 20);
        _ = quotes.GetSmi(lookbackPeriods: 13, firstSmoothPeriods: 25, secondSmoothPeriods: 2, signalPeriods: 3);
        _ = quotes.GetSmma(lookbackPeriods: 20);
        _ = tuples.GetSmma(lookbackPeriods: 20);
        _ = quotes.GetStarcBands(smaPeriods: 20, multiplier: 2, atrPeriods: 10);
        _ = quotes.GetStc(cyclePeriods: 10, fastPeriods: 23, slowPeriods: 50);
        _ = tuples.GetStc(cyclePeriods: 10, fastPeriods: 23, slowPeriods: 50);
        _ = quotes.GetStdDevChannels(lookbackPeriods: 20, stdDeviations: 2);
        _ = tuples.GetStdDevChannels(lookbackPeriods: 20, stdDeviations: 2);
        _ = quotes.GetStdDev(lookbackPeriods: 20);
        _ = quotes.GetStdDev(lookbackPeriods: 20, smaPeriods: 20);
        _ = tuples.GetStdDev(lookbackPeriods: 20);
        _ = quotes.GetStoch(lookbackPeriods: 14, signalPeriods: 3, smoothPeriods: 3);
        _ = quotes.GetStoch(lookbackPeriods: 14, signalPeriods: 3, smoothPeriods: 3, kFactor: 3, dFactor: 2, movingAverageType: MaType.SMA);
        _ = quotes.GetStochRsi(rsiPeriods: 14, stochPeriods: 14, signalPeriods: 3, smoothPeriods: 3);
        _ = tuples.GetStochRsi(rsiPeriods: 14, stochPeriods: 14, signalPeriods: 3, smoothPeriods: 3);
        _ = quotes.GetSuperTrend(lookbackPeriods: 10, multiplier: 3);

        // T-Z INDICATORS
        _ = quotes.GetT3();
        _ = quotes.GetT3(lookbackPeriods: 5, volumeFactor: 0.7);
        _ = tuples.GetT3(lookbackPeriods: 5, volumeFactor: 0.7);
        _ = quotes.GetTema(lookbackPeriods: 20);
        _ = tuples.GetTema(lookbackPeriods: 20);
        _ = quotes.GetTrix(lookbackPeriods: 15);
        _ = quotes.GetTrix(lookbackPeriods: 20, signalPeriods: 3);
        _ = tuples.GetTrix(lookbackPeriods: 20);
        _ = quotes.GetTr();
        _ = quotes.GetTsi();
        _ = quotes.GetTsi(lookbackPeriods: 20, smoothPeriods: 13, signalPeriods: 7);
        _ = tuples.GetTsi(lookbackPeriods: 20, smoothPeriods: 13, signalPeriods: 7);
        _ = quotes.GetUlcerIndex();
        _ = quotes.GetUlcerIndex(lookbackPeriods: 14);
        _ = tuples.GetUlcerIndex(lookbackPeriods: 14);
        _ = quotes.GetUltimate();
        _ = quotes.GetUltimate(shortPeriods: 7, middlePeriods: 14, longPeriods: 28);
        _ = quotes.GetVolatilityStop();
        _ = quotes.GetVolatilityStop(lookbackPeriods: 7, multiplier: 3);
        _ = quotes.GetVortex(lookbackPeriods: 14);
        _ = quotes.GetVwap();
        _ = quotes.GetVwma(lookbackPeriods: 20);
        _ = quotes.GetWilliamsR(lookbackPeriods: 14);
        _ = quotes.GetWma(lookbackPeriods: 20);
        _ = tuples.GetWma(lookbackPeriods: 20);
        _ = quotes.GetZigZag(endType: EndType.Close, percentChange: 5m);

        Console.WriteLine("✓ All indicators tested successfully");
    }

    /// <summary>
    /// Tests quote utility methods
    /// </summary>
    private static void TestUtilities(IEnumerable<Quote> quotes)
    {
        Console.WriteLine("\nTesting utilities...");

        CultureInfo culture = CultureInfo.InvariantCulture;

        // QUOTE UTILITIES
        _ = quotes.Use(candlePart: CandlePart.Close);
        _ = quotes.ToSortedCollection();
        _ = quotes.ToTupleCollection(candlePart: CandlePart.Close);
        _ = quotes.Aggregate(newSize: PeriodSize.Week);
        _ = quotes.Validate();

        // Individual quote utilities
        Quote quote = new() {
            Date = DateTime.Parse("2022-01-03", culture),
            Open = 100,
            High = 110,
            Low = 90,
            Close = 105,
            Volume = 1000
        };

        _ = quote.ToCandle();
        _ = quotes.ToCandles();

        // Find methods
        _ = quotes.Find(DateTime.Parse("2022-01-05", culture));

        Console.WriteLine("✓ All utilities tested successfully");
    }

    /// <summary>
    /// Tests result utility methods
    /// </summary>
    private static void TestResultUtilities(IEnumerable<Quote> quotes)
    {
        Console.WriteLine("\nTesting result utilities...");

        CultureInfo culture = CultureInfo.InvariantCulture;

        // Get sample results for testing
        IEnumerable<EmaResult> ema1 = quotes.GetEma(lookbackPeriods: 20);
        IEnumerable<SmaResult> sma1 = quotes.GetSma(lookbackPeriods: 20);

        // Find methods on results
        _ = ema1.Find(DateTime.Parse("2022-01-03", culture));

        // Result utilities - ToTupleChainable and ToTupleNaN
        try
        {
            Collection<(DateTime Date, double Value)> reusable = sma1.ToTupleChainable();
            Console.WriteLine($"  - ToTupleChainable: {reusable.Count} items");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  - ToTupleChainable failed: {ex.Message}");
        }

        try
        {
            Collection<(DateTime Date, double Value)> nanny = sma1.ToTupleNaN();
            Console.WriteLine($"  - ToTupleNaN: {nanny.Count} items");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  - ToTupleNaN failed: {ex.Message}");
        }

        Console.WriteLine("✓ All result utilities tested successfully");
    }
}
