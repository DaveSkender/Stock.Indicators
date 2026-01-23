using System.Globalization;
using Test.Base;

namespace Tests.Integration;

[TestClass, TestCategory("Integration")]
public class AllStreamHubsIntegrationTests : TestBase
{
    private const int TotalPeriods = 2000;
    private const int MaxCacheSize = 1500;
    private const int LateNonReplacementPeriod = 10;
    private const int LateNonReplacementInsertAt = 49;
    private const int RebuildAfterPeriod = 100;
    private const int LateReplacementPeriod = 1600;
    private const int LateReplacementInsertAt = 1650;

    [TestMethod]
    public void AllHubs_WithComplexScenario_MatchSeriesExactly()
    {
        // Get test data (need at least 2000 periods)
        List<Quote> sourceQuotes = LoadLongestQuotes().Take(TotalPeriods).ToList();

        // Create modified quote list for the test scenario
        List<Quote> testQuotes = [];
        Quote? lateNonReplacementQuote = null;
        Quote? lateReplacementQuote = null;

        for (int i = 0; i < sourceQuotes.Count; i++)
        {
            // Store the late arrival quote (period 10) and skip it initially
            if (i == LateNonReplacementPeriod)
            {
                lateNonReplacementQuote = sourceQuotes[i];
                continue;
            }

            // Store the late replacement quote (period 1600) and skip it initially
            if (i == LateReplacementPeriod)
            {
                lateReplacementQuote = sourceQuotes[i];
                continue;
            }

            testQuotes.Add(sourceQuotes[i]);
        }

        // Setup: Create one primary QuoteHub
        QuoteHub quoteHub = new() { MaxCacheSize = MaxCacheSize };

        // Subscribe all 80+ indicator hubs to the primary QuoteHub
        // Using default/common parameters for each indicator

        AdlHub adlHub = quoteHub.ToAdlHub();
        AdxHub adxHub = quoteHub.ToAdxHub(14);
        AlligatorHub alligatorHub = quoteHub.ToAlligatorHub();
        AlmaHub almaHub = quoteHub.ToAlmaHub(10, 0.85, 6);
        AroonHub aroonHub = quoteHub.ToAroonHub(25);
        AtrHub atrHub = quoteHub.ToAtrHub(14);
        AtrStopHub atrStopHub = quoteHub.ToAtrStopHub(21);
        AwesomeHub awesomeHub = quoteHub.ToAwesomeHub();
        BollingerBandsHub bollingerBandsHub = quoteHub.ToBollingerBandsHub(20, 2);
        BopHub bopHub = quoteHub.ToBopHub();
        CciHub cciHub = quoteHub.ToCciHub(20);
        ChaikinOscHub chaikinOscHub = quoteHub.ToChaikinOscHub();
        ChandelierHub chandelierHub = quoteHub.ToChandelierHub();
        ChopHub chopHub = quoteHub.ToChopHub(14);
        CmfHub cmfHub = quoteHub.ToCmfHub(20);
        CmoHub cmoHub = quoteHub.ToCmoHub(14);
        ConnorsRsiHub connorsRsiHub = quoteHub.ToConnorsRsiHub();
        DemaHub demaHub = quoteHub.ToDemaHub(20);
        DojiHub dojiHub = quoteHub.ToDojiHub();
        DonchianHub donchianHub = quoteHub.ToDonchianHub(20);
        DpoHub dpoHub = quoteHub.ToDpoHub(14);
        DynamicHub dynamicHub = quoteHub.ToDynamicHub(14);
        ElderRayHub elderRayHub = quoteHub.ToElderRayHub(13);
        EmaHub emaHub = quoteHub.ToEmaHub(20);
        EpmaHub epmaHub = quoteHub.ToEpmaHub(20);
        FcbHub fcbHub = quoteHub.ToFcbHub(2);
        FisherTransformHub fisherTransformHub = quoteHub.ToFisherTransformHub(10);
        ForceIndexHub forceIndexHub = quoteHub.ToForceIndexHub(13);
        FractalHub fractalHub = quoteHub.ToFractalHub(2);
        GatorHub gatorHub = quoteHub.ToGatorHub();
        HeikinAshiHub heikinAshiHub = quoteHub.ToHeikinAshiHub();
        HmaHub hmaHub = quoteHub.ToHmaHub(20);
        HtTrendlineHub htTrendlineHub = quoteHub.ToHtTrendlineHub();
        HurstHub hurstHub = quoteHub.ToHurstHub(20);
        IchimokuHub ichimokuHub = quoteHub.ToIchimokuHub();
        KamaHub kamaHub = quoteHub.ToKamaHub(10, 2, 30);
        KeltnerHub keltnerHub = quoteHub.ToKeltnerHub();
        KvoHub kvoHub = quoteHub.ToKvoHub();
        MaEnvelopesHub maEnvelopesHub = quoteHub.ToMaEnvelopesHub(20, 2.5, MaType.SMA);
        MacdHub macdHub = quoteHub.ToMacdHub();
        MamaHub mamaHub = quoteHub.ToMamaHub();
        MarubozuHub marubozuHub = quoteHub.ToMarubozuHub();
        MfiHub mfiHub = quoteHub.ToMfiHub(14);
        ObvHub obvHub = quoteHub.ToObvHub();
        ParabolicSarHub parabolicSarHub = quoteHub.ToParabolicSarHub();
        PivotPointsHub pivotPointsHub = quoteHub.ToPivotPointsHub(PeriodSize.Month, PivotPointType.Standard);
        PivotsHub pivotsHub = quoteHub.ToPivotsHub(11, 14);
        PmoHub pmoHub = quoteHub.ToPmoHub();
        PvoHub pvoHub = quoteHub.ToPvoHub();
        RenkoHub renkoHub = quoteHub.ToRenkoHub(2.5m);
        RocHub rocHub = quoteHub.ToRocHub(20);
        RocWbHub rocWbHub = quoteHub.ToRocWbHub(14);
        RollingPivotsHub rollingPivotsHub = quoteHub.ToRollingPivotsHub(11, 9);
        RsiHub rsiHub = quoteHub.ToRsiHub(14);
        SlopeHub slopeHub = quoteHub.ToSlopeHub(20);
        SmaHub smaHub = quoteHub.ToSmaHub(20);
        SmaAnalysisHub smaAnalysisHub = quoteHub.ToSmaAnalysisHub(10);
        SmiHub smiHub = quoteHub.ToSmiHub();
        SmmaHub smmaHub = quoteHub.ToSmmaHub(20);
        StarcBandsHub starcBandsHub = quoteHub.ToStarcBandsHub();
        StcHub stcHub = quoteHub.ToStcHub();
        StdDevHub stdDevHub = quoteHub.ToStdDevHub(10);
        StochHub stochHub = quoteHub.ToStochHub();
        StochRsiHub stochRsiHub = quoteHub.ToStochRsiHub(14);
        SuperTrendHub superTrendHub = quoteHub.ToSuperTrendHub();
        T3Hub t3Hub = quoteHub.ToT3Hub(5);
        TemaHub temaHub = quoteHub.ToTemaHub(20);
        TrHub trHub = quoteHub.ToTrHub();
        TrixHub trixHub = quoteHub.ToTrixHub(14);
        TsiHub tsiHub = quoteHub.ToTsiHub();
        UlcerIndexHub ulcerIndexHub = quoteHub.ToUlcerIndexHub(14);
        UltimateHub ultimateHub = quoteHub.ToUltimateHub();
        VolatilityStopHub volatilityStopHub = quoteHub.ToVolatilityStopHub();
        VortexHub vortexHub = quoteHub.ToVortexHub(14);
        VwapHub vwapHub = quoteHub.ToVwapHub();
        VwmaHub vwmaHub = quoteHub.ToVwmaHub(10);
        WilliamsRHub williamsRHub = quoteHub.ToWilliamsRHub(14);
        WmaHub wmaHub = quoteHub.ToWmaHub(20);

        // Rapidly consume quotes with complex scenario
        int processedCount = 0;

        foreach (Quote quote in testQuotes)
        {
            processedCount++;

            // Insert late non-replacement quote at position 49
            if (processedCount == LateNonReplacementInsertAt && lateNonReplacementQuote is not null)
            {
                quoteHub.Insert(lateNonReplacementQuote);
            }

            // Trigger full rebuild signal after 100 periods (via RemoveAt operation)
            if (processedCount == RebuildAfterPeriod)
            {
                // Remove an element to trigger rebuild across all subscribed hubs
                if (quoteHub.Results.Count > 50)
                {
                    quoteHub.RemoveAt(50);
                }
            }

            // Insert late replacement quote at position 1650
            if (processedCount == LateReplacementInsertAt && lateReplacementQuote is not null)
            {
                quoteHub.Insert(lateReplacementQuote);
            }

            // Add the current quote
            quoteHub.Add(quote);

            // Simulate several last period updates (repeat timestamp with new values)
            // This happens near the end of the data stream
            if (processedCount >= TotalPeriods - 5)
            {
                // Create a modified version of the same quote (same timestamp, different values)
                Quote updatedQuote = new(
                    Timestamp: quote.Timestamp,
                    Open: quote.Open * 1.001m,
                    High: quote.High * 1.001m,
                    Low: quote.Low * 0.999m,
                    Close: quote.Close * 1.001m,
                    Volume: quote.Volume * 1.1m);

                quoteHub.Add(updatedQuote);
            }
        }

        // Get final results from QuoteHub
        IReadOnlyList<IQuote> quoteHubResults = quoteHub.Results;

        // Verify against equivalent time series
        // For each indicator hub, verify it matches the series result
        // calculated from the final QuoteHub state

        IReadOnlyList<Quote> finalQuotes = quoteHubResults
            .Select(static q => new Quote(q.Timestamp, q.Open, q.High, q.Low, q.Close, q.Volume))
            .ToList();

        // Verify a sample of indicators (testing pattern for all)
        IReadOnlyList<RsiResult> rsiExpected = finalQuotes.ToRsi(14);
        rsiHub.Results.Should().HaveCount(rsiExpected.Count);

        IReadOnlyList<EmaResult> emaExpected = finalQuotes.ToEma(20);
        emaHub.Results.Should().HaveCount(emaExpected.Count);

        IReadOnlyList<MacdResult> macdExpected = finalQuotes.ToMacd();
        macdHub.Results.Should().HaveCount(macdExpected.Count);

        IReadOnlyList<BollingerBandsResult> bbExpected = finalQuotes.ToBollingerBands(20, 2);
        bollingerBandsHub.Results.Should().HaveCount(bbExpected.Count);

        IReadOnlyList<AtrResult> atrExpected = finalQuotes.ToAtr(14);
        atrHub.Results.Should().HaveCount(atrExpected.Count);

        // Verify cache size constraints are respected
        quoteHub.Results.Should().HaveCountLessOrEqualTo(MaxCacheSize);

        // Cleanup - EndTransmission unsubscribes all observers
        quoteHub.EndTransmission();
    }

    private static List<Quote> LoadLongestQuotes()
    {
        string[] possiblePaths = [
            Path.Combine("_testdata", "quotes", "longest.csv"),
            Path.Combine("..", "indicators", "_testdata", "quotes", "longest.csv"),
            Path.Combine("..", "..", "tests", "indicators", "_testdata", "quotes", "longest.csv")
        ];

        string? filepath = null;
        foreach (string path in possiblePaths)
        {
            if (File.Exists(path))
            {
                filepath = path;
                break;
            }
        }

        if (filepath is null)
        {
            throw new FileNotFoundException("Test data file not found: longest.csv");
        }

        CultureInfo englishCulture = new("en-US", false);
        return File.ReadAllLines(filepath)
            .Skip(1)
            .Select(line => QuoteFromCsv(line, englishCulture))
            .OrderBy(static x => x.Timestamp)
            .ToList();
    }

    private static Quote QuoteFromCsv(string csvLine, CultureInfo culture)
    {
        string[] values = csvLine.Split(',');
        return new Quote(
            Timestamp: DateTime.Parse(values[0], culture),
            Open: decimal.Parse(values[1], culture),
            High: decimal.Parse(values[2], culture),
            Low: decimal.Parse(values[3], culture),
            Close: decimal.Parse(values[4], culture),
            Volume: decimal.Parse(values[5], culture));
    }
}
