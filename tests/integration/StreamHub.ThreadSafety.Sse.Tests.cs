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

        try
        {
            // Wait for server to be ready
            bool serverReady = await WaitForServerReady(StcTestPort, cts).ConfigureAwait(true);
            if (!serverReady)
            {
                Assert.Fail("SSE server did not become ready within timeout period");
                return;
            }

            // Setup: QuoteHub with StcHub
            QuoteHub quoteHub = new() { MaxCacheSize = MaxCacheSize };
            StcHub stcHub = quoteHub.ToStcHub();

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

            // Get final quote cache and compute series
            IReadOnlyList<IQuote> finalCache = quoteHub.Results;
            List<IQuote> finalQuotes = finalCache.ToList();

            // Compare: StcHub results should match series on final cache
            stcHub.Results.IsExactly(finalQuotes.ToStc());

            quoteHub.EndTransmission();
        }
        finally
        {
            if (!serverProcess.HasExited)
            {
                serverProcess.Kill();
                await serverProcess.WaitForExitAsync(cts).ConfigureAwait(true);
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

        try
        {
            // Wait for server to be ready (poll until it responds or timeout)
            bool serverReady = await WaitForServerReady(AllHubsTestPort, cts).ConfigureAwait(true);
            if (!serverReady)
            {
                Assert.Fail("SSE server did not become ready within timeout period");
                return;
            }

            // Setup: Create one primary QuoteHub
            QuoteHub quoteHub = new() { MaxCacheSize = MaxCacheSize };

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

            // TODO: Renko produces variable-length output (bricks, not 1:1 with quotes), has rollback bug
            // RenkoHub renkoHub = quoteHub.ToRenkoHub(2.5m);

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
            quoteHubResults.Should().HaveCount(1498, "rollback operations modify final cache count");

            // Verify pruning actually occurred (we sent more quotes than cache size)
            int actualPruned = TargetQuoteCount - quoteHubResults.Count;
            actualPruned.Should().Be(502, "exactly 502 quotes should have been pruned after rollback operations");

            // Convert QuoteHub final cache to Quote list for series comparison
            List<IQuote> finalQuotes = quoteHubResults.ToList();

            // Verify ALL indicator hubs match their equivalent series calculations.
            // Series is computed on the FINAL QuoteHub cache (after all rollbacks, inserts, replacements).
            // This validates that hubs correctly handle pruning, late arrivals, and state rollbacks.
            adlHub.Results.IsExactly(finalQuotes.ToAdl());
            adxHub.Results.IsExactly(finalQuotes.ToAdx(14));
            alligatorHub.Results.IsExactly(finalQuotes.ToAlligator());
            almaHub.Results.IsExactly(finalQuotes.ToAlma(10, 0.85, 6));
            aroonHub.Results.IsExactly(finalQuotes.ToAroon(25));
            atrHub.Results.IsExactly(finalQuotes.ToAtr(14));
            atrStopHub.Results.IsExactly(finalQuotes.ToAtrStop(21));
            awesomeHub.Results.IsExactly(finalQuotes.ToAwesome());
            bollingerBandsHub.Results.IsExactly(finalQuotes.ToBollingerBands(20, 2));
            bopHub.Results.IsExactly(finalQuotes.ToBop());
            cciHub.Results.IsExactly(finalQuotes.ToCci(20));
            chaikinOscHub.Results.IsExactly(finalQuotes.ToChaikinOsc());
            chandelierHub.Results.IsExactly(finalQuotes.ToChandelier());
            chopHub.Results.IsExactly(finalQuotes.ToChop(14));
            cmfHub.Results.IsExactly(finalQuotes.ToCmf(20));
            cmoHub.Results.IsExactly(finalQuotes.ToCmo(14));
            connorsRsiHub.Results.IsExactly(finalQuotes.ToConnorsRsi());
            demaHub.Results.IsExactly(finalQuotes.ToDema(20));
            dojiHub.Results.IsExactly(finalQuotes.ToDoji());
            donchianHub.Results.IsExactly(finalQuotes.ToDonchian(20));
            dpoHub.Results.IsExactly(finalQuotes.ToDpo(14));
            dynamicHub.Results.IsExactly(finalQuotes.ToDynamic(14));
            elderRayHub.Results.IsExactly(finalQuotes.ToElderRay(13));
            emaHub.Results.IsExactly(finalQuotes.ToEma(20));

            // TODO: EPMA has floating-point precision differences after rollbacks (~1e-14)
            // epmaHub.Results.IsExactly(finalQuotes.ToEpma(20));

            fractalHub.Results.IsExactly(finalQuotes.ToFractal(2));
            fcbHub.Results.IsExactly(finalQuotes.ToFcb(2));
            fisherTransformHub.Results.IsExactly(finalQuotes.ToFisherTransform(10));

            // TODO: ForceIndex rollback bug - incorrect values after rollback
            // forceIndexHub.Results.IsExactly(finalQuotes.ToForceIndex(13));

            gatorHub.Results.IsExactly(finalQuotes.ToGator());
            heikinAshiHub.Results.IsExactly(finalQuotes.ToHeikinAshi());
            hmaHub.Results.IsExactly(finalQuotes.ToHma(20));
            htTrendlineHub.Results.IsExactly(finalQuotes.ToHtTrendline());
            hurstHub.Results.IsExactly(finalQuotes.ToHurst(20));
            ichimokuHub.Results.IsExactly(finalQuotes.ToIchimoku());
            kamaHub.Results.IsExactly(finalQuotes.ToKama(10, 2, 30));
            keltnerHub.Results.IsExactly(finalQuotes.ToKeltner());
            kvoHub.Results.IsExactly(finalQuotes.ToKvo());
            maEnvelopesHub.Results.IsExactly(finalQuotes.ToMaEnvelopes(20, 2.5, MaType.SMA));
            macdHub.Results.IsExactly(finalQuotes.ToMacd());
            mamaHub.Results.IsExactly(finalQuotes.ToMama());
            marubozuHub.Results.IsExactly(finalQuotes.ToMarubozu());
            mfiHub.Results.IsExactly(finalQuotes.ToMfi(14));
            obvHub.Results.IsExactly(finalQuotes.ToObv());
            parabolicSarHub.Results.IsExactly(finalQuotes.ToParabolicSar());
            pivotPointsHub.Results.IsExactly(finalQuotes.ToPivotPoints(PeriodSize.Month, PivotPointType.Standard));
            pivotsHub.Results.IsExactly(finalQuotes.ToPivots(11, 14));
            pmoHub.Results.IsExactly(finalQuotes.ToPmo());
            pvoHub.Results.IsExactly(finalQuotes.ToPvo());

            // TODO: Renko produces variable-length output (bricks, not 1:1 with quotes)
            // renkoHub.Results.IsExactly(finalQuotes.ToRenko(2.5m));

            rocHub.Results.IsExactly(finalQuotes.ToRoc(20));
            rocWbHub.Results.IsExactly(finalQuotes.ToRocWb(14));
            rollingPivotsHub.Results.IsExactly(finalQuotes.ToRollingPivots(20, 0));
            rsiHub.Results.IsExactly(finalQuotes.ToRsi(14));

            // TODO: Slope rollback bug - incorrect Intercept values after rollback
            // slopeHub.Results.IsExactly(finalQuotes.ToSlope(20));

            smaHub.Results.IsExactly(finalQuotes.ToSma(20));
            smaAnalysisHub.Results.IsExactly(finalQuotes.ToSmaAnalysis(10));
            smiHub.Results.IsExactly(finalQuotes.ToSmi());
            smmaHub.Results.IsExactly(finalQuotes.ToSmma(20));
            starcBandsHub.Results.IsExactly(finalQuotes.ToStarcBands());
            stcHub.Results.IsExactly(finalQuotes.ToStc());
            stdDevHub.Results.IsExactly(finalQuotes.ToStdDev(10));
            stochHub.Results.IsExactly(finalQuotes.ToStoch());
            stochRsiHub.Results.IsExactly(finalQuotes.ToStochRsi(14));
            superTrendHub.Results.IsExactly(finalQuotes.ToSuperTrend());
            t3Hub.Results.IsExactly(finalQuotes.ToT3(5));
            temaHub.Results.IsExactly(finalQuotes.ToTema(20));
            trHub.Results.IsExactly(finalQuotes.ToTr());
            trixHub.Results.IsExactly(finalQuotes.ToTrix(14));
            tsiHub.Results.IsExactly(finalQuotes.ToTsi());
            ulcerIndexHub.Results.IsExactly(finalQuotes.ToUlcerIndex(14));
            ultimateHub.Results.IsExactly(finalQuotes.ToUltimate());
            volatilityStopHub.Results.IsExactly(finalQuotes.ToVolatilityStop());
            vortexHub.Results.IsExactly(finalQuotes.ToVortex(14));
            vwapHub.Results.IsExactly(finalQuotes.ToVwap());
            vwmaHub.Results.IsExactly(finalQuotes.ToVwma(10));
            williamsRHub.Results.IsExactly(finalQuotes.ToWilliamsR(14));
            wmaHub.Results.IsExactly(finalQuotes.ToWma(20));
            // Cleanup
            quoteHub.EndTransmission();
        }
        finally
        {
            // Stop SSE server
            if (!serverProcess.HasExited)
            {
                serverProcess.Kill();
                await serverProcess.WaitForExitAsync(cts).ConfigureAwait(true);
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

            string serverProjectPath = Path.Combine(repoRoot, "tools", "sse-server");

            ProcessStartInfo startInfo = new() {
                FileName = "dotnet",
                Arguments = $"run --project \"{serverProjectPath}\" -- --urls http://localhost:{port}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Process? process = Process.Start(startInfo);
            if (process is not null)
            {
                process.OutputDataReceived += (_, __) => { };
                process.ErrorDataReceived += (_, __) => { };
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
