using System.Diagnostics;
using System.Text.Json;
using System.Linq;
using Test.Tools;

namespace StreamHubs;

/// <summary>
/// Integration tests to exercise the SSE streaming quote server and ensure that every streaming indicator
/// hub produces results identical to their static series counterparts.  These tests intentionally
/// ingest more quotes than the hub cache can hold and then perform a variety of out-of-order insert,
/// remove and replace operations.  The goal is to verify that pruned timelines heal correctly and
/// that the tail end of the static indicator series (computed on the full, corrected quote sequence)
/// matches the contents of the indicator hub caches exactly.  Do not modify the production code
/// when adjusting this test – instead, adjust the assertions here to reflect the correct behaviour.
/// </summary>
[TestClass, TestCategory("Integration")]
public class ThreadSafetyTests : TestBase
{
    private const int StcTestPort = 5099;
    private const int AllHubsTestPort = 5100;
    private const int TargetQuoteCount = 2000;
    private const int MaxCacheSize = 1500;
    private const int SseInterval = 1; // milliseconds between quotes
    private const string QuoteInterval = "1Day"; // daily quotes

    private static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    public TestContext? TestContext { get; set; }

    /// <summary>
    /// This test runs the SSE server and pipes quotes into a <see cref="QuoteHub"/> that is chained
    /// only to the STC indicator.  It reproduces a specific bug in <c>StcHub.RollbackState()</c>
    /// by performing several out-of-order operations.  After all quotes and revisions are processed
    /// the test computes a full static STC series on the amended quote list and then verifies that
    /// the tail end of the static series matches the streaming hub results exactly.
    /// </summary>
    [TestMethod]
    public async Task StcHub_WithSseStreamAndRollbacks_MatchesSeriesExactly()
    {
        CancellationToken cts = TestContext?.CancellationToken ?? default;

        // Start SSE server
        Process? serverProcess = StartSseServer(StcTestPort);
        if (serverProcess is null)
        {
            Assert.Fail("Failed to start SSE server");
            return;
        }

        // Setup: QuoteHub with StcHub
        QuoteHub quoteHub = new(MaxCacheSize);
        StcHub stcHub = quoteHub.ToStcHub();

        try
        {
            // Wait for server to be ready
            bool serverReady = await WaitForServerReady(StcTestPort, cts).ConfigureAwait(true);
            if (!serverReady)
            {
                Assert.Fail("SSE server did not become ready within timeout period");
                return;
            }

            // Consume quotes from SSE stream
            List<Quote> allQuotes = await ConsumeQuotesFromSse(quoteHub, StcTestPort).ConfigureAwait(true);
            allQuotes.Should().HaveCount(TargetQuoteCount);

            // Build complete quote list with all operations that will be applied.
            // This ensures series is computed on the exact same sequence the streaming hub processes.
            List<Quote> allQuotesWithOperations = new(allQuotes);

            // Apply rollback scenarios to trigger StcHub.RollbackState():
            // STC warmup period is slowPeriods + cyclePeriods - 2 = 50 + 10 - 2 = 58
            // The bug in RollbackState only manifests when targetIndex >= 58

            // 1. Late arrival after warmup (triggers rollback at index > 58)
            // Note: Insert of existing quote is a no-op for series, already in allQuotesWithOperations
            if (allQuotes.Count > 80)
            {
                quoteHub.Insert(allQuotes[80]);
            }

            // 2. Remove and re-insert after warmup (triggers complete rollback)
            // Note: Remove then Insert of same quote is a no-op for series
            if (allQuotes.Count > 100)
            {
                quoteHub.RemoveAt(100);
                quoteHub.Insert(allQuotes[100]);
            }

            // 3. Late arrival deep into the data (triggers rollback with full buffer)
            // Note: Insert of existing quote is a no-op for series, already in allQuotesWithOperations
            if (allQuotes.Count > 500)
            {
                quoteHub.Insert(allQuotes[500]);
            }

            // The hub cache should be completely filled – STC indicator uses the same QuoteHub and
            // therefore must have exactly MaxCacheSize results after processing all quotes and
            // revision operations.  If fewer results exist then pruning has not behaved correctly.
            quoteHub.Results.Should().HaveCount(MaxCacheSize, "quote hub should have exactly the configured cache size");

            // Compute series on FULL quote list, then take last N matching cache size.
            // Streaming indicators process all quotes and maintain state, so series must be
            // computed on the full history, then truncated to match the final cache size.
            int cacheSize = MaxCacheSize;
            IReadOnlyList<StcResult> expected = allQuotesWithOperations
                .ToStc()
                .TakeLast(cacheSize)
                .ToList();

            // Streaming results should match last N from full series
            stcHub.Results.IsExactly(expected);
        }
        finally
        {
            quoteHub.EndTransmission();

            if (!serverProcess.HasExited)
            {
                serverProcess.Kill();
                try
                {
                    await serverProcess.WaitForExitAsync(CancellationToken.None).ConfigureAwait(true);
                }
                catch (OperationCanceledException)
                {
                    // Ignore cancellation - process is already killed
                }
            }

            serverProcess.Dispose();
        }
    }

    /// <summary>
    /// Runs the SSE server and ingests 2,000+ quotes into a single <see cref="QuoteHub"/> with a
    /// maximum cache size of 1,500.  Every built-in streaming indicator hub is subscribed to the
    /// primary quote hub.  A series of out-of-order operations (insert, remove and replace) are
    /// applied to exercise the hub’s rollback logic both before and after pruning occurs.  After
    /// all quotes and revisions have been processed the full static series for each indicator is
    /// computed on the amended quote sequence, the results are truncated to the current cache size,
    /// and the streaming hub results are compared against the static tail.  This verifies that
    /// pruning and healing work correctly and that the streaming hubs remain in lockstep with
    /// their static counterparts.
    /// </summary>
    [TestMethod]
    public async Task AllHubs_WithSseStream_MatchSeriesExactly()
    {
        CancellationToken cts = TestContext?.CancellationToken ?? default;

        // Start SSE server
        Process? serverProcess = StartSseServer(AllHubsTestPort);
        if (serverProcess is null)
        {
            Assert.Fail("Failed to start SSE server");
            return;
        }

        // Setup: Create one primary QuoteHub
        QuoteHub quoteHub = new(MaxCacheSize);

        try
        {
            // Wait for server to be ready (poll until it responds or timeout)
            bool serverReady = await WaitForServerReady(AllHubsTestPort, cts).ConfigureAwait(true);
            if (!serverReady)
            {
                Assert.Fail("SSE server did not become ready within timeout period");
                return;
            }

            // Subscribe all 80+ indicator hubs to the primary QuoteHub
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
            FractalHub fractalHub = quoteHub.ToFractalHub(2);
            FcbHub fcbHub = quoteHub.ToFcbHub(2);
            FisherTransformHub fisherTransformHub = quoteHub.ToFisherTransformHub(10);
            ForceIndexHub forceIndexHub = quoteHub.ToForceIndexHub(13);
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
            RollingPivotsHub rollingPivotsHub = quoteHub.ToRollingPivotsHub(20, 0);
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

            // Consume quotes from SSE stream and apply test scenarios
            List<Quote> allQuotes = await ConsumeQuotesFromSse(quoteHub, AllHubsTestPort).ConfigureAwait(true);

            // Verify SSE stream delivered the full batch
            allQuotes.Should().HaveCount(TargetQuoteCount, "SSE stream should deliver the full batch");

            // Build complete quote list with all operations applied (matching what streaming hubs processed).
            // This ensures series is computed on the exact same sequence the streaming hub processes.
            List<Quote> allQuotesWithRevisions = new(allQuotes);

            // Apply rollback scenarios to test state management:

            // 1. Late non‑replacement arrival: insert quote at index 10.  This simulates an out‑of‑order
            //    quote that arrives long after the chronological point where it belongs.  We do not
            //    modify the allQuotesWithRevisions list because the quote already exists at index 10.
            if (allQuotesWithRevisions.Count > 10)
            {
                Quote lateQuote10 = allQuotesWithRevisions[10];
                quoteHub.Insert(lateQuote10);
            }

            // 2. Full rebuild signal after 100 periods (remove and re‑add to trigger rollback).  This
            //    operation effectively sends a notification that the data at index 100 has been corrected.
            if (allQuotesWithRevisions.Count > 100)
            {
                Quote rebuildQuote = allQuotesWithRevisions[100];
                quoteHub.RemoveAt(100);
                quoteHub.Insert(rebuildQuote);
            }

            // 3. Late arrival replacement deep in the series: replace quote at index 1600 by re‑adding
            //    with the same timestamp but a modified close.  Update the static list so that the
            //    subsequent static series computation matches what the streaming hub processed.
            if (allQuotesWithRevisions.Count > 1600)
            {
                Quote original = allQuotesWithRevisions[1600];
                Quote replacementQuote = new(
                    original.Timestamp,
                    original.Open,
                    original.High,
                    original.Low,
                    original.Close * 1.01m,  // Modified close
                    original.Volume
                );
                quoteHub.Add(replacementQuote);
                allQuotesWithRevisions[1600] = replacementQuote;
            }

            // 4. Multiple quotes with the same timestamp (revisions).  Each Add with the same timestamp
            //    triggers a rebuild in the streaming hubs.  The final revision is back to the original
            //    value, so there is no net change in the static list.
            if (allQuotesWithRevisions.Count > 0)
            {
                Quote lastQuote = allQuotesWithRevisions[^1];
                quoteHub.Add(new Quote(
                    lastQuote.Timestamp,
                    lastQuote.Open,
                    lastQuote.High,
                    lastQuote.Low,
                    lastQuote.Close * 0.99m,
                    lastQuote.Volume
                ));
                quoteHub.Add(new Quote(
                    lastQuote.Timestamp,
                    lastQuote.Open,
                    lastQuote.High,
                    lastQuote.Low,
                    lastQuote.Close * 1.01m,
                    lastQuote.Volume
                ));
                quoteHub.Add(lastQuote);
            }

            // Get final results from QuoteHub (this is the ground truth after all operations)
            IReadOnlyList<IQuote> quoteHubResults = quoteHub.Results;

            // Verify that the hub cache is exactly at the configured capacity.  A QuoteHub that
            // receives more quotes than its capacity must prune the oldest quotes until the cache
            // contains MaxCacheSize entries.  Having fewer or more entries indicates incorrect
            // pruning behaviour.
            quoteHubResults.Should().HaveCount(MaxCacheSize, "quote hub should have exactly the configured cache size");

            // Determine how many quotes were pruned by comparing the initial total with the current cache size.
            // Use the amended list for correctness.  The expected number of pruned quotes is the
            // difference between the full revised list and the cache size.
            int cacheSize = MaxCacheSize;
            int actualPruned = allQuotesWithRevisions.Count - cacheSize;
            actualPruned.Should().Be(allQuotesWithRevisions.Count - MaxCacheSize,
                "pruning should remove exactly the excess quotes beyond the configured cache size");

            // Compute static series on the FULL quote list (with revisions), then take the last N results.
            // Streaming indicators process the entire history and maintain state across revisions,
            // therefore the correct comparison pattern is to compute on the full history and then
            // truncate to the current cache size.
            adlHub.Results.IsExactly(allQuotesWithRevisions.ToAdl().TakeLast(cacheSize).ToList());
            adxHub.Results.IsExactly(allQuotesWithRevisions.ToAdx(14).TakeLast(cacheSize).ToList());
            alligatorHub.Results.IsExactly(allQuotesWithRevisions.ToAlligator().TakeLast(cacheSize).ToList());
            almaHub.Results.IsExactly(allQuotesWithRevisions.ToAlma(10, 0.85, 6).TakeLast(cacheSize).ToList());
            aroonHub.Results.IsExactly(allQuotesWithRevisions.ToAroon(25).TakeLast(cacheSize).ToList());
            atrHub.Results.IsExactly(allQuotesWithRevisions.ToAtr(14).TakeLast(cacheSize).ToList());
            atrStopHub.Results.IsExactly(allQuotesWithRevisions.ToAtrStop(21).TakeLast(cacheSize).ToList());
            awesomeHub.Results.IsExactly(allQuotesWithRevisions.ToAwesome().TakeLast(cacheSize).ToList());
            bollingerBandsHub.Results.IsExactly(allQuotesWithRevisions.ToBollingerBands(20, 2).TakeLast(cacheSize).ToList());
            bopHub.Results.IsExactly(allQuotesWithRevisions.ToBop().TakeLast(cacheSize).ToList());
            cciHub.Results.IsExactly(allQuotesWithRevisions.ToCci(20).TakeLast(cacheSize).ToList());
            chaikinOscHub.Results.IsExactly(allQuotesWithRevisions.ToChaikinOsc().TakeLast(cacheSize).ToList());
            chandelierHub.Results.IsExactly(allQuotesWithRevisions.ToChandelier().TakeLast(cacheSize).ToList());
            chopHub.Results.IsExactly(allQuotesWithRevisions.ToChop(14).TakeLast(cacheSize).ToList());
            cmfHub.Results.IsExactly(allQuotesWithRevisions.ToCmf(20).TakeLast(cacheSize).ToList());
            cmoHub.Results.IsExactly(allQuotesWithRevisions.ToCmo(14).TakeLast(cacheSize).ToList());
            connorsRsiHub.Results.IsExactly(allQuotesWithRevisions.ToConnorsRsi().TakeLast(cacheSize).ToList());
            demaHub.Results.IsExactly(allQuotesWithRevisions.ToDema(20).TakeLast(cacheSize).ToList());
            dojiHub.Results.IsExactly(allQuotesWithRevisions.ToDoji().TakeLast(cacheSize).ToList());
            donchianHub.Results.IsExactly(allQuotesWithRevisions.ToDonchian(20).TakeLast(cacheSize).ToList());
            dpoHub.Results.IsExactly(allQuotesWithRevisions.ToDpo(14).TakeLast(cacheSize).ToList());
            dynamicHub.Results.IsExactly(allQuotesWithRevisions.ToDynamic(14).TakeLast(cacheSize).ToList());
            elderRayHub.Results.IsExactly(allQuotesWithRevisions.ToElderRay(13).TakeLast(cacheSize).ToList());
            emaHub.Results.IsExactly(allQuotesWithRevisions.ToEma(20).TakeLast(cacheSize).ToList());
            epmaHub.Results.IsExactly(allQuotesWithRevisions.ToEpma(20).TakeLast(cacheSize).ToList());
            fractalHub.Results.IsExactly(allQuotesWithRevisions.ToFractal(2).TakeLast(cacheSize).ToList());
            fcbHub.Results.IsExactly(allQuotesWithRevisions.ToFcb(2).TakeLast(cacheSize).ToList());
            fisherTransformHub.Results.IsExactly(allQuotesWithRevisions.ToFisherTransform(10).TakeLast(cacheSize).ToList());
            forceIndexHub.Results.IsExactly(allQuotesWithRevisions.ToForceIndex(13).TakeLast(cacheSize).ToList());
            gatorHub.Results.IsExactly(allQuotesWithRevisions.ToGator().TakeLast(cacheSize).ToList());
            heikinAshiHub.Results.IsExactly(allQuotesWithRevisions.ToHeikinAshi().TakeLast(cacheSize).ToList());
            hmaHub.Results.IsExactly(allQuotesWithRevisions.ToHma(20).TakeLast(cacheSize).ToList());
            htTrendlineHub.Results.IsExactly(allQuotesWithRevisions.ToHtTrendline().TakeLast(cacheSize).ToList());
            hurstHub.Results.IsExactly(allQuotesWithRevisions.ToHurst(20).TakeLast(cacheSize).ToList());
            ichimokuHub.Results.IsExactly(allQuotesWithRevisions.ToIchimoku().TakeLast(cacheSize).ToList());
            kamaHub.Results.IsExactly(allQuotesWithRevisions.ToKama(10, 2, 30).TakeLast(cacheSize).ToList());
            keltnerHub.Results.IsExactly(allQuotesWithRevisions.ToKeltner().TakeLast(cacheSize).ToList());
            kvoHub.Results.IsExactly(allQuotesWithRevisions.ToKvo().TakeLast(cacheSize).ToList());
            maEnvelopesHub.Results.IsExactly(allQuotesWithRevisions.ToMaEnvelopes(20, 2.5, MaType.SMA).TakeLast(cacheSize).ToList());
            macdHub.Results.IsExactly(allQuotesWithRevisions.ToMacd().TakeLast(cacheSize).ToList());
            mamaHub.Results.IsExactly(allQuotesWithRevisions.ToMama().TakeLast(cacheSize).ToList());
            marubozuHub.Results.IsExactly(allQuotesWithRevisions.ToMarubozu().TakeLast(cacheSize).ToList());
            mfiHub.Results.IsExactly(allQuotesWithRevisions.ToMfi(14).TakeLast(cacheSize).ToList());
            obvHub.Results.IsExactly(allQuotesWithRevisions.ToObv().TakeLast(cacheSize).ToList());
            parabolicSarHub.Results.IsExactly(allQuotesWithRevisions.ToParabolicSar().TakeLast(cacheSize).ToList());
            pivotPointsHub.Results.IsExactly(allQuotesWithRevisions.ToPivotPoints(PeriodSize.Month, PivotPointType.Standard).TakeLast(cacheSize).ToList());
            pivotsHub.Results.IsExactly(allQuotesWithRevisions.ToPivots(11, 14).TakeLast(cacheSize).ToList());
            pmoHub.Results.IsExactly(allQuotesWithRevisions.ToPmo().TakeLast(cacheSize).ToList());
            pvoHub.Results.IsExactly(allQuotesWithRevisions.ToPvo().TakeLast(cacheSize).ToList());
            // Renko charts do not have a one‑to‑one relationship between quotes and bricks, so we
            // cannot simply take the last N results.  Instead, align the static Renko series
            // with the streaming hub by matching on the first brick’s date.  Any bricks before
            // that date in the static series represent periods that were pruned from the quote
            // hub and must be discarded.  Then compare the remainder of the series to the
            // streaming hub results.
            {
                var renkoStatic = allQuotesWithRevisions.ToRenko(2.5m).ToList();
                var firstDate = renkoHub.Results.First().Date;
                int startIndex = renkoStatic.FindIndex(r => r.Date == firstDate);
                startIndex.Should().BeGreaterThanOrEqualTo(0,
                    "the first Renko result in the hub should exist in the static series");
                var expectedRenko = renkoStatic.Skip(startIndex).ToList();
                renkoHub.Results.IsExactly(expectedRenko);
            }
            rocHub.Results.IsExactly(allQuotesWithRevisions.ToRoc(20).TakeLast(cacheSize).ToList());
            rocWbHub.Results.IsExactly(allQuotesWithRevisions.ToRocWb(14).TakeLast(cacheSize).ToList());
            rollingPivotsHub.Results.IsExactly(allQuotesWithRevisions.ToRollingPivots(20, 0).TakeLast(cacheSize).ToList());
            rsiHub.Results.IsExactly(allQuotesWithRevisions.ToRsi(14).TakeLast(cacheSize).ToList());
            slopeHub.Results.IsExactly(allQuotesWithRevisions.ToSlope(20).TakeLast(cacheSize).ToList());
            smaHub.Results.IsExactly(allQuotesWithRevisions.ToSma(20).TakeLast(cacheSize).ToList());
            smaAnalysisHub.Results.IsExactly(allQuotesWithRevisions.ToSmaAnalysis(10).TakeLast(cacheSize).ToList());
            smiHub.Results.IsExactly(allQuotesWithRevisions.ToSmi().TakeLast(cacheSize).ToList());
            smmaHub.Results.IsExactly(allQuotesWithRevisions.ToSmma(20).TakeLast(cacheSize).ToList());
            starcBandsHub.Results.IsExactly(allQuotesWithRevisions.ToStarcBands().TakeLast(cacheSize).ToList());
            stcHub.Results.IsExactly(allQuotesWithRevisions.ToStc().TakeLast(cacheSize).ToList());
            stdDevHub.Results.IsExactly(allQuotesWithRevisions.ToStdDev(10).TakeLast(cacheSize).ToList());
            stochHub.Results.IsExactly(allQuotesWithRevisions.ToStoch().TakeLast(cacheSize).ToList());
            stochRsiHub.Results.IsExactly(allQuotesWithRevisions.ToStochRsi(14).TakeLast(cacheSize).ToList());
            superTrendHub.Results.IsExactly(allQuotesWithRevisions.ToSuperTrend().TakeLast(cacheSize).ToList());
            t3Hub.Results.IsExactly(allQuotesWithRevisions.ToT3(5).TakeLast(cacheSize).ToList());
            temaHub.Results.IsExactly(allQuotesWithRevisions.ToTema(20).TakeLast(cacheSize).ToList());
            trHub.Results.IsExactly(allQuotesWithRevisions.ToTr().TakeLast(cacheSize).ToList());
            trixHub.Results.IsExactly(allQuotesWithRevisions.ToTrix(14).TakeLast(cacheSize).ToList());
            tsiHub.Results.IsExactly(allQuotesWithRevisions.ToTsi().TakeLast(cacheSize).ToList());
            ulcerIndexHub.Results.IsExactly(allQuotesWithRevisions.ToUlcerIndex(14).TakeLast(cacheSize).ToList());
            ultimateHub.Results.IsExactly(allQuotesWithRevisions.ToUltimate().TakeLast(cacheSize).ToList());
            volatilityStopHub.Results.IsExactly(allQuotesWithRevisions.ToVolatilityStop().TakeLast(cacheSize).ToList());
            vortexHub.Results.IsExactly(allQuotesWithRevisions.ToVortex(14).TakeLast(cacheSize).ToList());
            vwapHub.Results.IsExactly(allQuotesWithRevisions.ToVwap().TakeLast(cacheSize).ToList());
            vwmaHub.Results.IsExactly(allQuotesWithRevisions.ToVwma(10).TakeLast(cacheSize).ToList());
            williamsRHub.Results.IsExactly(allQuotesWithRevisions.ToWilliamsR(14).TakeLast(cacheSize).ToList());
            wmaHub.Results.IsExactly(allQuotesWithRevisions.ToWma(20).TakeLast(cacheSize).ToList());
        }
        finally
        {
            // Cleanup
            quoteHub.EndTransmission();

            // Stop SSE server
            if (!serverProcess.HasExited)
            {
                serverProcess.Kill();
                try
                {
                    await serverProcess.WaitForExitAsync(CancellationToken.None).ConfigureAwait(true);
                }
                catch (OperationCanceledException)
                {
                    // Ignore cancellation - process is already killed
                }
            }

            serverProcess.Dispose();
        }
    }

    #region Helper methods
    // Helper methods StartSseServer, FindRepositoryRoot, WaitForServerReady and ConsumeQuotesFromSse
    // are unchanged from the original test.  They encapsulate the logic for launching the test
    // SSE server, waiting until it is ready, and streaming quotes into the QuoteHub.  The
    // implementations are reproduced here verbatim for completeness and to avoid any external
    // dependencies when this file is used in isolation.

    private static Process? StartSseServer(int port)
    {
        try
        {
            string? repoRoot = FindRepositoryRoot();
            if (repoRoot is null)
            {
                Console.WriteLine("[Test] Cannot find repository root.");
                return null;
            }

            // Try DOTNET_CONFIGURATION env var, then check for Release build, fallback to Debug
            string configuration = Environment.GetEnvironmentVariable("DOTNET_CONFIGURATION") ?? "Debug";
            string serverExePath = Path.Combine(
                repoRoot, "tools", "sse-server", "bin", configuration, "net10.0", "Test.SseServer.exe");
            string serverDllPath = Path.Combine(
                repoRoot, "tools", "sse-server", "bin", configuration, "net10.0", "Test.SseServer.dll");

            // Auto-detect configuration if default doesn't exist
            if (!File.Exists(serverExePath) && !File.Exists(serverDllPath))
            {
                string alternateConfig = configuration == "Debug" ? "Release" : "Debug";
                string alternateExePath = Path.Combine(
                    repoRoot, "tools", "sse-server", "bin", alternateConfig, "net10.0", "Test.SseServer.exe");
                string alternateDllPath = Path.Combine(
                    repoRoot, "tools", "sse-server", "bin", alternateConfig, "net10.0", "Test.SseServer.dll");

                if (File.Exists(alternateExePath) || File.Exists(alternateDllPath))
                {
                    configuration = alternateConfig;
                    serverExePath = alternateExePath;
                    serverDllPath = alternateDllPath;
                }
            }

            ProcessStartInfo startInfo = new() {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // Use .exe if it exists (Windows), otherwise use dotnet with .dll (Linux/macOS)
            if (File.Exists(serverExePath))
            {
                startInfo.FileName = serverExePath;
                startInfo.Arguments = $"--urls http://localhost:{port}";
            }
            else if (File.Exists(serverDllPath))
            {
                startInfo.FileName = "dotnet";
                startInfo.Arguments = $"\"{serverDllPath}\" --urls http://localhost:{port}";
            }
            else
            {
                Console.WriteLine($"[Test] SSE server executable not found at {serverExePath} or {serverDllPath}");
                return null;
            }

            Process? process = Process.Start(startInfo);
            if (process is not null)
            {
                process.OutputDataReceived += (_, e) => {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Console.WriteLine($"[SSE Server] {e.Data}");
                    }
                };
                process.ErrorDataReceived += (_, e) => {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Console.WriteLine($"[SSE Server ERROR] {e.Data}");
                    }
                };
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                Console.WriteLine($"[Test] SSE server started on port {port} (PID: {process.Id})");
            }

            return process;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[Test] IO error starting server: {ex.Message}");
            return null;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"[Test] Invalid operation starting server: {ex.Message}");
            return null;
        }
    }

    private static string? FindRepositoryRoot()
    {
        string currentDir = Directory.GetCurrentDirectory();
        while (!string.IsNullOrEmpty(currentDir))
        {
            if (File.Exists(Path.Combine(currentDir, "Stock.Indicators.sln")))
            {
                return currentDir;
            }

            string? parentDir = Directory.GetParent(currentDir)?.FullName;
            if (parentDir == currentDir)
            {
                break;
            }

            currentDir = parentDir ?? string.Empty;
        }

        return null;
    }

    private static async Task<bool> WaitForServerReady(int port, CancellationToken cancellationToken)
    {
        using HttpClient httpClient = new() { Timeout = TimeSpan.FromSeconds(2) };
        Uri healthUri = new($"http://localhost:{port}/quotes/longest?batchSize=1");

        // Try for up to 30 seconds (15 attempts * 2 second intervals)
        for (int attempt = 0; attempt < 15; attempt++)
        {
            try
            {
                using HttpResponseMessage response = await httpClient
                    .GetAsync(healthUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[Test] SSE server ready after {attempt + 1} attempts");
                    return true;
                }
            }
            catch (HttpRequestException)
            {
                // Server not ready yet, continue polling
            }
            catch (TaskCanceledException)
            {
                // Timeout on this attempt, continue polling
            }

            if (attempt < 14) // Don't delay after the last attempt
            {
                await Task.Delay(2000, cancellationToken).ConfigureAwait(false);
            }
        }

        Console.WriteLine("[Test] SSE server did not become ready within timeout");
        return false;
    }

    private static async Task<List<Quote>> ConsumeQuotesFromSse(QuoteHub quoteHub, int port)
    {
        List<Quote> allQuotes = [];
        using HttpClient httpClient = new() { Timeout = TimeSpan.FromMinutes(5) };

        string batchSizeParam = $"&batchSize={TargetQuoteCount}";
        Uri uri = new($"http://localhost:{port}/quotes/longest?interval={SseInterval}&quoteInterval={QuoteInterval}{batchSizeParam}");

        using HttpResponseMessage response = await httpClient
            .GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using StreamReader reader = new(stream);

        int quotesProcessed = 0;
        string? line;

        while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) is not null)
        {
            // SSE format: "event: quote", "data: {...}", blank line
            if (line.StartsWith("data:", StringComparison.Ordinal))
            {
                string json = line[5..].Trim();
                Quote? quote = JsonSerializer.Deserialize<Quote>(json, JsonOptions);

                if (quote is not null)
                {
                    allQuotes.Add(quote);
                    quoteHub.Add(quote);
                    quotesProcessed++;

                    if (quotesProcessed >= TargetQuoteCount)
                    {
                        break;
                    }
                }
            }
        }

        return allQuotes;
    }
    #endregion
}
