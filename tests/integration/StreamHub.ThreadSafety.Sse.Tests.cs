using System.Diagnostics;
using System.Text.Json;
using Test.Tools;

namespace StreamHubs;

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

            // Apply rollback scenarios to trigger StcHub.RollbackState():
            // STC warmup period is slowPeriods + cyclePeriods - 2 = 50 + 10 - 2 = 58
            // The bug in RollbackState only manifests when targetIndex >= 58

            // 1. Late arrival after warmup (triggers rollback at index > 58)
            if (allQuotes.Count > 80)
            {
                quoteHub.Insert(allQuotes[80]);
            }

            // 2. Remove and re-insert after warmup (triggers complete rollback)
            if (allQuotes.Count > 100)
            {
                quoteHub.RemoveAt(100);
                quoteHub.Insert(allQuotes[100]);
            }

            // 3. Late arrival deep into the data (triggers rollback with full buffer)
            if (allQuotes.Count > 500)
            {
                quoteHub.Insert(allQuotes[500]);
            }

            // Compute series on the FULL quote list (including quotes that were pruned from cache).
            // Then take last N results to match the final cache size.
            // This is the correct comparison because streaming indicators process all quotes
            // and maintain running state, even after pruning removes early quotes from cache.
            int finalCacheSize = quoteHub.Results.Count;
            IReadOnlyList<StcResult> expectedResults = allQuotes
                .ToStc()
                .TakeLast(finalCacheSize)
                .ToList();

            // Compare: StcHub streaming results should match truncated series
            stcHub.Results.IsExactly(expectedResults);
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

            // Apply rollback scenarios to test state management:

            // 1. Late non-replacement arrival: insert quote at index 10, arriving at position 49
            Quote lateQuote10 = allQuotes[10];
            quoteHub.Insert(lateQuote10);

            // 2. Full rebuild signal after 100 periods (remove and re-add to trigger rollback)
            Quote rebuildQuote = allQuotes[100];
            quoteHub.RemoveAt(100);
            quoteHub.Insert(rebuildQuote);

            // 3. Late arrival replacement: replace quote at index 1600 by re-adding with same timestamp
            if (allQuotes.Count > 1600)
            {
                Quote original = allQuotes[1600];
                Quote replacementQuote = new(
                    original.Timestamp,
                    original.Open,
                    original.High,
                    original.Low,
                    original.Close * 1.01m,  // Modified close
                    original.Volume
                );
                quoteHub.Add(replacementQuote);
            }

            // 4. Multiple quotes with same timestamp (revisions) - each Add with same timestamp triggers rebuild
            Quote lastQuote = allQuotes[^1];

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

            quoteHub.Add(lastQuote); // final revision back to original

            // Get final results from QuoteHub (this is the ground truth after all operations)
            IReadOnlyList<IQuote> quoteHubResults = quoteHub.Results;

            // Verify results are not empty and cache size is respected
            quoteHubResults.Should().NotBeEmpty("quote hub should have results");

            quoteHubResults.Should().HaveCount(MaxCacheSize - 2);

            // Verify pruning actually occurred (we sent more quotes than cache size)
            int actualPruned = TargetQuoteCount - quoteHubResults.Count;
            actualPruned.Should().Be(TargetQuoteCount - MaxCacheSize + 2);

            // Build complete quote list with all revisions applied (for series comparison).
            // This mirrors what the streaming indicators processed, including quotes that were later pruned.
            List<Quote> allQuotesWithRevisions = [.. allQuotes];

            // Apply revision from operation 3: replace quote at index 1600
            if (allQuotes.Count > 1600)
            {
                Quote original = allQuotes[1600];
                allQuotesWithRevisions[1600] = new Quote(
                    original.Timestamp,
                    original.Open,
                    original.High,
                    original.Low,
                    original.Close * 1.01m,
                    original.Volume
                );
            }

            // Operation 4 final revision is back to original, so no change needed

            // Get final cache size for truncation
            int finalCacheSize = quoteHubResults.Count;

            // Verify ALL indicator hubs match their equivalent series calculations.
            // Series must be computed on the FULL set of quotes (including those that were pruned),
            // with all revisions applied, then truncated to match the final cache size.
            // This matches how streaming indicators work: they process all quotes and maintain
            // running state, even after pruning removes early quotes from the cache.
            adlHub.Results.IsExactly(allQuotesWithRevisions.ToAdl().TakeLast(finalCacheSize).ToList());
            adxHub.Results.IsExactly(allQuotesWithRevisions.ToAdx(14).TakeLast(finalCacheSize).ToList());
            alligatorHub.Results.IsExactly(allQuotesWithRevisions.ToAlligator().TakeLast(finalCacheSize).ToList());
            almaHub.Results.IsExactly(allQuotesWithRevisions.ToAlma(10, 0.85, 6).TakeLast(finalCacheSize).ToList());
            aroonHub.Results.IsExactly(allQuotesWithRevisions.ToAroon(25).TakeLast(finalCacheSize).ToList());
            atrHub.Results.IsExactly(allQuotesWithRevisions.ToAtr(14).TakeLast(finalCacheSize).ToList());
            atrStopHub.Results.IsExactly(allQuotesWithRevisions.ToAtrStop(21).TakeLast(finalCacheSize).ToList());
            awesomeHub.Results.IsExactly(allQuotesWithRevisions.ToAwesome().TakeLast(finalCacheSize).ToList());
            bollingerBandsHub.Results.IsExactly(allQuotesWithRevisions.ToBollingerBands(20, 2).TakeLast(finalCacheSize).ToList());
            bopHub.Results.IsExactly(allQuotesWithRevisions.ToBop().TakeLast(finalCacheSize).ToList());
            cciHub.Results.IsExactly(allQuotesWithRevisions.ToCci(20).TakeLast(finalCacheSize).ToList());
            chaikinOscHub.Results.IsExactly(allQuotesWithRevisions.ToChaikinOsc().TakeLast(finalCacheSize).ToList());
            chandelierHub.Results.IsExactly(allQuotesWithRevisions.ToChandelier().TakeLast(finalCacheSize).ToList());
            chopHub.Results.IsExactly(allQuotesWithRevisions.ToChop(14).TakeLast(finalCacheSize).ToList());
            cmfHub.Results.IsExactly(allQuotesWithRevisions.ToCmf(20).TakeLast(finalCacheSize).ToList());
            cmoHub.Results.IsExactly(allQuotesWithRevisions.ToCmo(14).TakeLast(finalCacheSize).ToList());
            connorsRsiHub.Results.IsExactly(allQuotesWithRevisions.ToConnorsRsi().TakeLast(finalCacheSize).ToList());
            demaHub.Results.IsExactly(allQuotesWithRevisions.ToDema(20).TakeLast(finalCacheSize).ToList());
            dojiHub.Results.IsExactly(allQuotesWithRevisions.ToDoji().TakeLast(finalCacheSize).ToList());
            donchianHub.Results.IsExactly(allQuotesWithRevisions.ToDonchian(20).TakeLast(finalCacheSize).ToList());
            dpoHub.Results.IsExactly(allQuotesWithRevisions.ToDpo(14).TakeLast(finalCacheSize).ToList());
            dynamicHub.Results.IsExactly(allQuotesWithRevisions.ToDynamic(14).TakeLast(finalCacheSize).ToList());
            elderRayHub.Results.IsExactly(allQuotesWithRevisions.ToElderRay(13).TakeLast(finalCacheSize).ToList());
            emaHub.Results.IsExactly(allQuotesWithRevisions.ToEma(20).TakeLast(finalCacheSize).ToList());
            epmaHub.Results.IsExactly(allQuotesWithRevisions.ToEpma(20).TakeLast(finalCacheSize).ToList());
            fractalHub.Results.IsExactly(allQuotesWithRevisions.ToFractal(2).TakeLast(finalCacheSize).ToList());
            fcbHub.Results.IsExactly(allQuotesWithRevisions.ToFcb(2).TakeLast(finalCacheSize).ToList());
            fisherTransformHub.Results.IsExactly(allQuotesWithRevisions.ToFisherTransform(10).TakeLast(finalCacheSize).ToList());
            forceIndexHub.Results.IsExactly(allQuotesWithRevisions.ToForceIndex(13).TakeLast(finalCacheSize).ToList());
            gatorHub.Results.IsExactly(allQuotesWithRevisions.ToGator().TakeLast(finalCacheSize).ToList());
            heikinAshiHub.Results.IsExactly(allQuotesWithRevisions.ToHeikinAshi().TakeLast(finalCacheSize).ToList());
            hmaHub.Results.IsExactly(allQuotesWithRevisions.ToHma(20).TakeLast(finalCacheSize).ToList());
            htTrendlineHub.Results.IsExactly(allQuotesWithRevisions.ToHtTrendline().TakeLast(finalCacheSize).ToList());
            hurstHub.Results.IsExactly(allQuotesWithRevisions.ToHurst(20).TakeLast(finalCacheSize).ToList());
            ichimokuHub.Results.IsExactly(allQuotesWithRevisions.ToIchimoku().TakeLast(finalCacheSize).ToList());
            kamaHub.Results.IsExactly(allQuotesWithRevisions.ToKama(10, 2, 30).TakeLast(finalCacheSize).ToList());
            keltnerHub.Results.IsExactly(allQuotesWithRevisions.ToKeltner().TakeLast(finalCacheSize).ToList());
            kvoHub.Results.IsExactly(allQuotesWithRevisions.ToKvo().TakeLast(finalCacheSize).ToList());
            maEnvelopesHub.Results.IsExactly(allQuotesWithRevisions.ToMaEnvelopes(20, 2.5, MaType.SMA).TakeLast(finalCacheSize).ToList());
            macdHub.Results.IsExactly(allQuotesWithRevisions.ToMacd().TakeLast(finalCacheSize).ToList());
            mamaHub.Results.IsExactly(allQuotesWithRevisions.ToMama().TakeLast(finalCacheSize).ToList());
            marubozuHub.Results.IsExactly(allQuotesWithRevisions.ToMarubozu().TakeLast(finalCacheSize).ToList());
            mfiHub.Results.IsExactly(allQuotesWithRevisions.ToMfi(14).TakeLast(finalCacheSize).ToList());
            obvHub.Results.IsExactly(allQuotesWithRevisions.ToObv().TakeLast(finalCacheSize).ToList());
            parabolicSarHub.Results.IsExactly(allQuotesWithRevisions.ToParabolicSar().TakeLast(finalCacheSize).ToList());
            pivotPointsHub.Results.IsExactly(allQuotesWithRevisions.ToPivotPoints(PeriodSize.Month, PivotPointType.Standard).TakeLast(finalCacheSize).ToList());
            pivotsHub.Results.IsExactly(allQuotesWithRevisions.ToPivots(11, 14).TakeLast(finalCacheSize).ToList());
            pmoHub.Results.IsExactly(allQuotesWithRevisions.ToPmo().TakeLast(finalCacheSize).ToList());
            pvoHub.Results.IsExactly(allQuotesWithRevisions.ToPvo().TakeLast(finalCacheSize).ToList());
            renkoHub.Results.IsExactly(allQuotesWithRevisions.ToRenko(2.5m).TakeLast(finalCacheSize).ToList());
            rocHub.Results.IsExactly(allQuotesWithRevisions.ToRoc(20).TakeLast(finalCacheSize).ToList());
            rocWbHub.Results.IsExactly(allQuotesWithRevisions.ToRocWb(14).TakeLast(finalCacheSize).ToList());
            rollingPivotsHub.Results.IsExactly(allQuotesWithRevisions.ToRollingPivots(20, 0).TakeLast(finalCacheSize).ToList());
            rsiHub.Results.IsExactly(allQuotesWithRevisions.ToRsi(14).TakeLast(finalCacheSize).ToList());
            slopeHub.Results.IsExactly(allQuotesWithRevisions.ToSlope(20).TakeLast(finalCacheSize).ToList());
            smaHub.Results.IsExactly(allQuotesWithRevisions.ToSma(20).TakeLast(finalCacheSize).ToList());
            smaAnalysisHub.Results.IsExactly(allQuotesWithRevisions.ToSmaAnalysis(10).TakeLast(finalCacheSize).ToList());
            smiHub.Results.IsExactly(allQuotesWithRevisions.ToSmi().TakeLast(finalCacheSize).ToList());
            smmaHub.Results.IsExactly(allQuotesWithRevisions.ToSmma(20).TakeLast(finalCacheSize).ToList());
            starcBandsHub.Results.IsExactly(allQuotesWithRevisions.ToStarcBands().TakeLast(finalCacheSize).ToList());
            stcHub.Results.IsExactly(allQuotesWithRevisions.ToStc().TakeLast(finalCacheSize).ToList());
            stdDevHub.Results.IsExactly(allQuotesWithRevisions.ToStdDev(10).TakeLast(finalCacheSize).ToList());
            stochHub.Results.IsExactly(allQuotesWithRevisions.ToStoch().TakeLast(finalCacheSize).ToList());
            stochRsiHub.Results.IsExactly(allQuotesWithRevisions.ToStochRsi(14).TakeLast(finalCacheSize).ToList());
            superTrendHub.Results.IsExactly(allQuotesWithRevisions.ToSuperTrend().TakeLast(finalCacheSize).ToList());
            t3Hub.Results.IsExactly(allQuotesWithRevisions.ToT3(5).TakeLast(finalCacheSize).ToList());
            temaHub.Results.IsExactly(allQuotesWithRevisions.ToTema(20).TakeLast(finalCacheSize).ToList());
            trHub.Results.IsExactly(allQuotesWithRevisions.ToTr().TakeLast(finalCacheSize).ToList());
            trixHub.Results.IsExactly(allQuotesWithRevisions.ToTrix(14).TakeLast(finalCacheSize).ToList());
            tsiHub.Results.IsExactly(allQuotesWithRevisions.ToTsi().TakeLast(finalCacheSize).ToList());
            ulcerIndexHub.Results.IsExactly(allQuotesWithRevisions.ToUlcerIndex(14).TakeLast(finalCacheSize).ToList());
            ultimateHub.Results.IsExactly(allQuotesWithRevisions.ToUltimate().TakeLast(finalCacheSize).ToList());
            volatilityStopHub.Results.IsExactly(allQuotesWithRevisions.ToVolatilityStop().TakeLast(finalCacheSize).ToList());
            vortexHub.Results.IsExactly(allQuotesWithRevisions.ToVortex(14).TakeLast(finalCacheSize).ToList());
            vwapHub.Results.IsExactly(allQuotesWithRevisions.ToVwap().TakeLast(finalCacheSize).ToList());
            vwmaHub.Results.IsExactly(allQuotesWithRevisions.ToVwma(10).TakeLast(finalCacheSize).ToList());
            williamsRHub.Results.IsExactly(allQuotesWithRevisions.ToWilliamsR(14).TakeLast(finalCacheSize).ToList());
            wmaHub.Results.IsExactly(allQuotesWithRevisions.ToWma(20).TakeLast(finalCacheSize).ToList());
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
}
